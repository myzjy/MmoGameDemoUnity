using System;
using System.Collections.Generic;
using BestHTTP.Authentication;
using BestHTTP.Core;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using BestHTTP.Timings;
using FrostEngine;
#if !BESTHTTP_DISABLE_CACHING
using BestHTTP.Caching;
#endif
#if !BESTHTTP_DISABLE_COOKIES
using BestHTTP.Cookies;
#endif

// ReSharper disable once CheckNamespace
namespace BestHTTP.Connections
{
    /// <summary>
    /// https://tools.ietf.org/html/draft-thomson-hybi-http-timeout-03
    /// Test servers: http://tools.ietf.org/ http://nginx.org/
    /// </summary>
    public sealed class KeepAliveHeader
    {
        /// <summary>
        /// A host sets the value of the "timeout" parameter to the time that the host will allow an idle connection to remain open before it is closed. A connection is idle if no data is sent or received by a host.
        /// </summary>
        public TimeSpan TimeOut { get; private set; }

        /// <summary>
        /// The "max" parameter has been used to indicate the maximum number of requests that would be made on the connection.This parameter is deprecated.Any limit on requests can be enforced by sending "Connection: close" and closing the connection.
        /// </summary>
        public int MaxRequests { get; private set; }

        public void Parse(List<string> headerValues)
        {
            HeaderParser parser = new HeaderParser(headerValues[0]);
            HeaderValue value;
            if (parser.TryGet("timeout", out value) && value.HasValue)
            {
                int intValue = 0;
                if (int.TryParse(value.Value, out intValue) && intValue > 1)
                    this.TimeOut = TimeSpan.FromSeconds(intValue - 1);
                else
                    this.TimeOut = TimeSpan.MaxValue;
            }

            if (parser.TryGet("max", out value) && value.HasValue)
            {
                int intValue = 0;
                if (int.TryParse("max", out intValue))
                    this.MaxRequests = intValue;
                else
                    this.MaxRequests = int.MaxValue;
            }
        }
    }

    public static class ConnectionHelper
    {
        public static void HandleResponse(string context, HttpRequest request, out bool resendRequest,
            out HttpConnectionStates proposedConnectionState, ref KeepAliveHeader keepAlive,
            LoggingContext loggingContext1 = null, LoggingContext loggingContext2 = null,
            LoggingContext loggingContext3 = null)
        {
            resendRequest = false;
            proposedConnectionState = HttpConnectionStates.Processing;

            if (request.Response != null)
            {
#if !BESTHTTP_DISABLE_COOKIES
                // Try to store cookies before we do anything else, as we may remove the response deleting the cookies as well.
                if (request.IsCookiesEnabled)
                    CookieJar.Set(request.Response);
#endif

                switch (request.Response.StatusCode)
                {
                    // Not authorized
                    // http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html#sec10.4.2
                    case 401:
                    {
                        string authHeader = DigestStore.FindBest(request.Response.GetHeaderValues("www-authenticate"));
                        if (!string.IsNullOrEmpty(authHeader))
                        {
                            var digest = DigestStore.GetOrCreate(request.CurrentUri);
                            digest.ParseChallange(authHeader);

                            if (request.Credentials != null && digest.IsUriProtected(request.CurrentUri) &&
                                (!request.HasHeader("Authorization") || digest.Stale))
                                resendRequest = true;
                        }

                        goto default;
                    }

                    case 407:
                    {
                        if (request.Proxy == null)
                            goto default;

                        resendRequest = request.Proxy.SetupRequest(request);

                        goto default;
                    }

                    // Redirected
                    case 301: // http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html#sec10.3.2
                    case 302: // http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html#sec10.3.3
                    case 303: // "See Other"
                    case 307: // http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html#sec10.3.8
                    case 308: // http://tools.ietf.org/html/rfc7238
                    {
                        if (request.RedirectCount >= request.MaxRedirects)
                            goto default;
                        request.RedirectCount++;

                        string location = request.Response.GetFirstHeaderValue("location");
                        if (!string.IsNullOrEmpty(location))
                        {
                            Uri redirectUri = ConnectionHelper.GetRedirectUri(request, location);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.Log(
                                $"[HTTPConnection][method:HandleResponse] [msg] [{context}] - Redirected to Location: '{location}' redirectUri: '{redirectUri}'");
#endif
                            if (redirectUri == request.CurrentUri)
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.Log($"[HTTPConnection][method:HandleResponse] [msg] [{context}] - 重定向到相同的位置!");
#endif
                                goto default;
                            }

                            // Let the user to take some control over the redirection
                            if (!request.CallOnBeforeRedirection(redirectUri))
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.Log(
                                    $"[HTTPConnection][method:HandleResponse] [msg] [{context}] OnBeforeRedirection返回False");
#endif
                                goto default;
                            }

                            // Remove the previously set Host header.
                            request.RemoveHeader("Host");

                            // Set the Referer header to the last Uri.
                            request.SetHeader("Referer", request.CurrentUri.ToString());

                            // Set the new Uri, the CurrentUri will return this while the IsRedirected property is true
                            request.RedirectUri = redirectUri;

                            request.IsRedirected = true;

                            resendRequest = true;
                        }
                        else
                            throw new Exception(
                                $"[{context}] Got redirect status({request.Response.StatusCode.ToString()}) without 'location' header!");

                        goto default;
                    }

#if !BESTHTTP_DISABLE_CACHING
                    case 304:
                        if (request.DisableCache)
                            break;

                        if (ConnectionHelper.LoadFromCache(context, request, loggingContext1, loggingContext2,
                                loggingContext3))
                        {
                            request.Timing.Add(TimingEventNames.LoadingFromCache);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.Log(
                                $"[HTTPConnection][method:HandleResponse] [msg] [{context}] - HandleResponse - 成功从缓存加载!");
#endif
                            // Update any caching value
                            HttpCacheService.SetUpCachingValues(request.CurrentUri, request.Response);
                        }
                        else
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.Log(
                                $"[HTTPConnection][method:HandleResponse] [msg] [{context}] - HandleResponse - 从缓存加载失败!");
#endif
                            resendRequest = true;
                        }

                        break;
#endif

                    default:
#if !BESTHTTP_DISABLE_CACHING
                        ConnectionHelper.TryStoreInCache(request);
#endif
                        break;
                }

                // 关闭流是手动完成的?
                if (request.Response != null && !request.Response.IsClosedManually)
                {
                    // 如果我们有一个响应，并且服务器在消息发送给我们之后告诉我们它关闭了连接，那么我们也将关闭连接。
                    bool closeByServer = request.Response.HasHeaderWithValue("connection", "close");
                    bool closeByClient = !request.IsKeepAlive;

                    if (closeByServer || closeByClient)
                    {
                        proposedConnectionState = HttpConnectionStates.Closed;
                    }
                    else if (request.Response != null)
                    {
                        var keepAliveheaderValues = request.Response.GetHeaderValues("keep-alive");
                        if (keepAliveheaderValues != null && keepAliveheaderValues.Count > 0)
                        {
                            keepAlive ??= new KeepAliveHeader();

                            keepAlive.Parse(keepAliveheaderValues);
                        }
                    }
                }

                // 在这里取消响应，而不是重定向的情况(301,302,307,308)，因为响应可能有一个Connection: Close报头，我们会错过处理。
                // 如果Connection: Close存在，则服务器正在关闭连接，我们将重用已关闭的连接。
                if (resendRequest)
                {
                    // 丢弃重定向响应，我们不再需要它了
                    request.Response = null;

                    if (proposedConnectionState == HttpConnectionStates.Closed)
                    {
                        proposedConnectionState = HttpConnectionStates.ClosedResendRequest;
                    }
                }
            }
        }

        public static Uri GetRedirectUri(HttpRequest request, string location)
        {
            Uri result;
            try
            {
                result = new Uri(location);

                if (result.IsFile || result.AbsolutePath == location)
                    result = null;
            }
            catch
            {
                // 有时服务器只发回新uri的路径和查询组件
                result = null;
            }

            if (result == null)
            {
                var baseURL = request.CurrentUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);

                if (!location.StartsWith("/"))
                {
                    var segments = request.CurrentUri.Segments;
                    segments[segments.Length - 1] = location;

                    location = String.Join(string.Empty, segments);
                    if (location.StartsWith("//"))
                        location = location.Substring(1);
                }

                bool endsWithSlash = baseURL[baseURL.Length - 1] == '/';
                bool startsWithSlash = location[0] == '/';
                if (endsWithSlash && startsWithSlash)
                {
                    result = new Uri($"{baseURL}{location.Substring(1)}");
                }
                else if (!endsWithSlash && !startsWithSlash)
                {
                    result = new Uri($"{baseURL}/{location}");
                }
                else
                {
                    result = new Uri($"{baseURL}{location}");
                }
            }

            return result;
        }

#if !BESTHTTP_DISABLE_CACHING
        public static bool LoadFromCache(string context, HttpRequest request, LoggingContext loggingContext1 = null,
            LoggingContext loggingContext2 = null, LoggingContext loggingContext3 = null)
        {
            if (request.IsRedirected)
            {
                if (LoadFromCache(context, request, request.RedirectUri, loggingContext1, loggingContext2,
                        loggingContext3))
                {
                    return true;
                }
                else
                {
                    Caching.HttpCacheService.DeleteEntity(request.RedirectUri);
                }
            }

            bool loaded = LoadFromCache(context, request, request.Uri, loggingContext1, loggingContext2,
                loggingContext3);
            if (!loaded)
            {
                Caching.HttpCacheService.DeleteEntity(request.Uri);
            }

            return loaded;
        }

        private static bool LoadFromCache(string context, HttpRequest request, Uri uri,
            LoggingContext loggingContext1 = null, LoggingContext loggingContext2 = null,
            LoggingContext loggingContext3 = null)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[HTTPConnection][method:LoadFromCache] [msg] [{context}] - LoadFromCache for Uri: {uri.ToString()}");
#endif
            var cacheEntity = HttpCacheService.GetEntity(uri);
            if (cacheEntity == null)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[HTTPConnection] [method:LoadFromCache] [msg] [{context}] - LoadFromCache for Uri: {uri.ToString()} - 缓存实体未找到!");
#endif
                return false;
            }

            request.Response.CacheFileInfo = cacheEntity;

            try
            {
                using var cacheStream = cacheEntity.GetBodyStream(out var bodyLength);
                if (cacheStream == null)
                    return false;

                if (!request.Response.HasHeader("content-length"))
                {
                    request.Response.AddHeader("content-length", bodyLength.ToString());
                }

                request.Response.IsFromCache = true;

                if (!request.CacheOnly)
                {
                    request.Response.ReadRaw(cacheStream, bodyLength);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool TryLoadAllFromCache(string context, HttpRequest request,
            LoggingContext loggingContext1 = null, LoggingContext loggingContext2 = null,
            LoggingContext loggingContext3 = null)
        {
            // 我们将尝试从缓存中读取响应，但如果发生了什么事情，我们将退回到正常的方式。
            try
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                //除非被缓存控制指令(第14.9节)特别约束，缓存系统可能总是将一个成功的响应(见第13.8节)存储为缓存实体，
                //如果它是新鲜的，可以不经过验证就返回，并且在验证成功后可以返回。
                //如果它是新鲜的，可能不需要验证就返回它!
                Debug.Log(
                    $"[ConnectionHelper] [method:TryLoadAllFromCache] [msg] [{context}] - TryLoadAllFromCache - 从缓存加载整个响应");
#endif

                request.Response = HttpCacheService.GetFullResponse(request);
                if (request.Response != null)
                {
                    return true;
                }
            }
            catch
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    "[ConnectionHelper] [method:TryLoadAllFromCache] [msg] [{context}] - TryLoadAllFromCache - 加载内容失败!");
#endif
                HttpCacheService.DeleteEntity(request.CurrentUri);
            }

            return false;
        }

        public static void TryStoreInCache(HttpRequest request)
        {
            // 如果UseStreaming && !DisableCache，那么我们已经写了对缓存的响应
            if (!request.UseStreaming &&
                !request.DisableCache &&
                request.Response != null &&
                HttpCacheService.IsSupported &&
                HttpCacheService.IsCacheable(request.CurrentUri, request.MethodType, request.Response))
            {
                if (request.IsRedirected)
                {
                    HttpCacheService.Store(request.Uri, request.MethodType, request.Response);
                }
                else
                {
                    HttpCacheService.Store(request.CurrentUri, request.MethodType, request.Response);
                }

                request.Timing.Add(TimingEventNames.WritingToCache);

                PluginEventHelper.EnqueuePluginEvent(new PluginEventInfo(PluginEvents.SaveCacheLibrary));
            }
        }
#endif
    }
}