using System;
using System.Collections.Generic;
using BestHTTP.Authentication;
using BestHTTP.Core;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using BestHTTP.Timings;
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

                            if (HttpManager.Logger.Level == Logger.Loglevels.All)
                                HttpManager.Logger.Verbose("HTTPConnection",
                                    string.Format("[{0}] - Redirected to Location: '{1}' redirectUri: '{1}'", context,
                                        location, redirectUri), loggingContext1, loggingContext2, loggingContext3);

                            if (redirectUri == request.CurrentUri)
                            {
                                HttpManager.Logger.Information("HTTPConnection",
                                    string.Format("[{0}] - Redirected to the same location!", context), loggingContext1,
                                    loggingContext2, loggingContext3);
                                goto default;
                            }

                            // Let the user to take some control over the redirection
                            if (!request.CallOnBeforeRedirection(redirectUri))
                            {
                                HttpManager.Logger.Information("HTTPConnection",
                                    string.Format("[{0}] OnBeforeRedirection returned False", context), loggingContext1,
                                    loggingContext2, loggingContext3);
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
                            throw new Exception(string.Format(
                                "[{0}] Got redirect status({1}) without 'location' header!", context,
                                request.Response.StatusCode.ToString()));

                        goto default;
                    }

#if !BESTHTTP_DISABLE_CACHING
                    case 304:
                        if (request.DisableCache)
                            break;

                        if (ConnectionHelper.LoadFromCache(context, request, loggingContext1, loggingContext2,
                                loggingContext3))
                        {
                            request.Timing.Add(TimingEventNames.Loading_From_Cache);
                            HttpManager.Logger.Verbose("HTTPConnection",
                                string.Format("[{0}] - HandleResponse - Loaded from cache successfully!", context),
                                loggingContext1, loggingContext2, loggingContext3);

                            // Update any caching value
                            HttpCacheService.SetUpCachingValues(request.CurrentUri, request.Response);
                        }
                        else
                        {
                            HttpManager.Logger.Verbose("HTTPConnection",
                                string.Format("[{0}] - HandleResponse - Loaded from cache failed!", context),
                                loggingContext1, loggingContext2, loggingContext3);
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

                // Closing the stream is done manually?
                if (request.Response != null && !request.Response.IsClosedManually)
                {
                    // If we have a response and the server telling us that it closed the connection after the message sent to us, then
                    //  we will close the connection too.
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
                            if (keepAlive == null)
                                keepAlive = new KeepAliveHeader();
                            keepAlive.Parse(keepAliveheaderValues);
                        }
                    }
                }

                // Null out the response here instead of the redirected cases (301, 302, 307, 308)
                //  because response might have a Connection: Close header that we would miss to process.
                // If Connection: Close is present, the server is closing the connection and we would
                // reuse that closed connection.
                if (resendRequest)
                {
                    // Discard the redirect response, we don't need it any more
                    request.Response = null;

                    if (proposedConnectionState == HttpConnectionStates.Closed)
                        proposedConnectionState = HttpConnectionStates.ClosedResendRequest;
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
                // Sometimes the server sends back only the path and query component of the new uri
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
                    result = new Uri(baseURL + location.Substring(1));
                else if (!endsWithSlash && !startsWithSlash)
                    result = new Uri(baseURL + '/' + location);
                else
                    result = new Uri(baseURL + location);
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
                    return true;
                else
                {
                    Caching.HttpCacheService.DeleteEntity(request.RedirectUri);
                }
            }

            bool loaded = LoadFromCache(context, request, request.Uri, loggingContext1, loggingContext2,
                loggingContext3);
            if (!loaded)
                Caching.HttpCacheService.DeleteEntity(request.Uri);

            return loaded;
        }

        private static bool LoadFromCache(string context, HttpRequest request, Uri uri,
            LoggingContext loggingContext1 = null, LoggingContext loggingContext2 = null,
            LoggingContext loggingContext3 = null)
        {
            if (HttpManager.Logger.Level == Logger.Loglevels.All)
                HttpManager.Logger.Verbose("HTTPConnection",
                    string.Format("[{0}] - LoadFromCache for Uri: {1}", context, uri.ToString()), loggingContext1,
                    loggingContext2, loggingContext3);

            var cacheEntity = HttpCacheService.GetEntity(uri);
            if (cacheEntity == null)
            {
                HttpManager.Logger.Warning("HTTPConnection",
                    string.Format("[{0}] - LoadFromCache for Uri: {1} - Cached entity not found!", context,
                        uri.ToString()), loggingContext1, loggingContext2, loggingContext3);
                return false;
            }

            request.Response.CacheFileInfo = cacheEntity;

            try
            {
                int bodyLength;
                using (var cacheStream = cacheEntity.GetBodyStream(out bodyLength))
                {
                    if (cacheStream == null)
                        return false;

                    if (!request.Response.HasHeader("content-length"))
                        request.Response.AddHeader("content-length", bodyLength.ToString());
                    request.Response.IsFromCache = true;

                    if (!request.CacheOnly)
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
            // We will try to read the response from the cache, but if something happens we will fallback to the normal way.
            try
            {
                //Unless specifically constrained by a cache-control (section 14.9) directive, a caching system MAY always store a successful response (see section 13.8) as a cache entity,
                //  MAY return it without validation if it is fresh, and MAY    return it after successful validation.
                // MAY return it without validation if it is fresh!
                if (HttpManager.Logger.Level == Logger.Loglevels.All)
                    HttpManager.Logger.Verbose("ConnectionHelper",
                        $"[{context}] - TryLoadAllFromCache - whole response loading from cache", loggingContext1,
                        loggingContext2, loggingContext3);

                request.Response = HttpCacheService.GetFullResponse(request);

                if (request.Response != null)
                    return true;
            }
            catch
            {
                HttpManager.Logger.Verbose("ConnectionHelper",
                    $"[{context}] - TryLoadAllFromCache - failed to load content!", loggingContext1, loggingContext2,
                    loggingContext3);
                HttpCacheService.DeleteEntity(request.CurrentUri);
            }

            return false;
        }

        public static void TryStoreInCache(HttpRequest request)
        {
            // if UseStreaming && !DisableCache then we already wrote the response to the cache
            if (!request.UseStreaming &&
                !request.DisableCache &&
                request.Response != null &&
                HttpCacheService.IsSupported &&
                HttpCacheService.IsCacheable(request.CurrentUri, request.MethodType, request.Response))
            {
                if (request.IsRedirected)
                    HttpCacheService.Store(request.Uri, request.MethodType, request.Response);
                else
                    HttpCacheService.Store(request.CurrentUri, request.MethodType, request.Response);
                request.Timing.Add(TimingEventNames.Writing_To_Cache);

                PluginEventHelper.EnqueuePluginEvent(new PluginEventInfo(PluginEvents.SaveCacheLibrary));
            }
        }
#endif
    }
}