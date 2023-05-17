using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BestHTTP.Core;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.Timings;
using UnityEngine;
#if !NETFX_CORE || UNITY_EDITOR
using System.Net.Sockets;
#endif

// ReSharper disable once CheckNamespace
namespace BestHTTP
{
#if !BESTHTTP_DISABLE_CACHING
    using Caching;
#endif

#if !BESTHTTP_DISABLE_COOKIES
    using Cookies;
#endif

    public class HttpResponse : IDisposable
    {
        internal const byte CR = 13;
        internal const byte LF = 10;

        /// <summary>
        ///读取缓冲区的最小大小。
        /// </summary>
        private const int MinReadBufferSize = 16 * 1024;

        public int VersionMajor { get; protected set; }

        public int VersionMinor { get; protected set; }

        /// <summary>
        /// 从服务器发送的状态码。
        /// </summary>
        public int StatusCode { get; protected set; }

        /// <summary>
        /// 如果状态码在[200..]范围内，返回true。300[或304(未经修改)
        /// </summary>
        public bool IsSuccess => this.StatusCode is >= 200 and < 300 || this.StatusCode == 304;

        /// <summary>
        /// 与StatusCode一起从服务器发送的消息。您可以从服务器上检查它的错误。
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// 如果是流响应则为True。
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private bool IsStreamed { get; set; }

#if !BESTHTTP_DISABLE_CACHING
        /// <summary>
        /// 指示从缓存中读取响应体。
        /// </summary>
        public bool IsFromCache { get; internal set; }

        /// <summary>
        /// 提供有关用于缓存请求的文件的信息。
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public HttpCacheFileInfo CacheFileInfo { get; internal set; }

        /// <summary>
        /// 确定此响应是否仅存储到缓存。
        /// 如果IsCacheOnly和isStreaming都为true, OnStreamingData不会被调用。
        /// </summary>
        private bool IsCacheOnly { get; set; }
#endif

        /// <summary>
        /// 如果这是HTTPProxy请求的响应，则为True。
        /// </summary>
        private bool IsProxyResponse { get; set; }

        /// <summary>
        /// 从服务器发送的报头。
        /// </summary>
        public Dictionary<string, List<string>> Headers { get; private set; }

        /// <summary>
        /// 从服务器上下载的数据。所有传输和内容编码解码(如有)。Chunked, gzip, deflate)。
        /// </summary>
        public byte[] Data { get; internal set; }

        /// <summary>
        /// 正常HTTP协议升级为其他HTTP协议。
        /// </summary>
        public bool IsUpgraded { get; private set; }

#if !BESTHTTP_DISABLE_COOKIES
        /// <summary>
        /// 服务器发送给客户端的cookie。
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public List<Cookie> Cookies { get; internal set; }
#endif

        /// <summary>
        /// 缓存、转换的数据。
        /// </summary>
        private string _dataAsString;

        /// <summary>
        /// 转换为UTF8字符串的数据。
        /// </summary>
        public string DataAsText
        {
            get
            {
                if (Data == null)
                    return string.Empty;

                if (!string.IsNullOrEmpty(_dataAsString))
                    return _dataAsString;

                return _dataAsString = Encoding.UTF8.GetString(Data, 0, Data.Length);
            }
        }

        /// <summary>
        /// 缓存转换的数据。
        /// </summary>
        private Texture2D _texture;

        /// <summary>
        /// 加载到Texture2D的数据.
        /// </summary>
        public Texture2D DataAsTexture2D
        {
            get
            {
                if (Data == null)
                    return null;

                if (_texture != null)
                    return _texture;

                _texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
                _texture.LoadImage(Data, true);

                return _texture;
            }
        }

        /// <summary>
        /// 如果连接流将手动关闭，则为。用于自定义协议(WebSocket, EventSource).
        /// </summary>
        public bool IsClosedManually { get; protected set; }

        /// <summary>
        /// IProtocol。LoggingContext实现。
        /// </summary>
        protected LoggingContext Context { get; private set; }

        /// <summary>
        /// HTTPManager请求事件队列中的流数据片段的计数。
        /// </summary>
#if UNITY_EDITOR
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        internal long UnprocessedFragments;

        internal readonly HttpRequest BaseRequest;

        protected Stream Stream;

        private byte[] _fragmentBuffer;
        private int _fragmentBufferDataLength;
#if !BESTHTTP_DISABLE_CACHING
        private Stream _cacheStream;
#endif
        private int _allFragmentSize;


        protected HttpResponse(HttpRequest request, bool isFromCache)
        {
            this.BaseRequest = request;
#if !BESTHTTP_DISABLE_CACHING
            this.IsFromCache = isFromCache;
#endif
            this.Context = new LoggingContext(this);
            this.Context.Add("BaseRequest", request.Context);
            this.Context.Add("IsFromCache", isFromCache);
        }

        public HttpResponse(
            HttpRequest request,
            Stream stream,
            bool isStreamed,
            bool isFromCache,
            bool isProxyResponse = false)
        {
            this.BaseRequest = request;
            this.Stream = stream;
            this.IsStreamed = isStreamed;

#if !BESTHTTP_DISABLE_CACHING
            this.IsFromCache = isFromCache;
            this.IsCacheOnly = request.CacheOnly;
#endif

            this.IsProxyResponse = isProxyResponse;

            this.IsClosedManually = false;

            this.Context = new LoggingContext(this);
            this.Context.Add("BaseRequest", request.GetHashCode());
            this.Context.Add("IsStreamed", isStreamed);
            this.Context.Add("IsFromCache", isFromCache);
        }

        public bool Receive(
            long forceReadRawContentLength = -1,
            bool readPayloadData = true,
            bool sendUpgradedEvent = true)
        {
            /*
             * 如果在此请求上调用Abort()则为True。
             */
            if (this.BaseRequest.IsCancellationRequested)
            {
                return false;
            }

            string statusLine;
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append(
                    $"Receive.forceReadRawContentLength: '{forceReadRawContentLength:N0}', readPayloadData: '{readPayloadData}'");
                Debug.Log($"{sb}");
            }
#endif

            // 在WP平台上，我们不能确定tcp连接是否关闭。
            //  因此，如果我们在这里得到一个异常，我们需要重新创建连接。
            try
            {
                // 从“HTTP/1.1 {StatusCode} {Message}”中读出“HTTP/1.1”
                statusLine = ReadTo(
                    stream: Stream,
                    blocker: (byte)' ');
            }
            catch
            {
                /*
                 * 如果在此请求上调用Abort()则为True。
                 */
                if (BaseRequest.IsCancellationRequested)
                {
                    return false;
                }

                if (BaseRequest.Retries >= BaseRequest.MaxRetries)
                {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                    {
                        var st = new StackTrace(new StackFrame(true));
                        var sf = st.GetFrame(0);
                        var sb = new StringBuilder(6);
                        sb.Append($"[{sf.GetFileName()}]");
                        sb.Append($"[method:{sf.GetMethod().Name}] ");
                        sb.Append($"{sf.GetMethod().Name} ");
                        sb.Append($"Line:{sf.GetFileLineNumber()} ");
                        sb.Append($"[msg{sf.GetMethod().Name}]");
                        sb.Append($"读取状态行失败!启用Retry，返回false.");
                        Debug.Log($"{sb}");
                    }
#endif
                    return false;
                }
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}] ");
                    sb.Append($"{sf.GetMethod().Name} ");
                    sb.Append($"Line:{sf.GetFileLineNumber()} ");
                    sb.Append($"[msg{sf.GetMethod().Name}]");
                    sb.Append($"取状态行失败!禁用重试，重新抛出异常。");
                    Debug.LogError($"{sb}");
                }
#endif
                throw;
            }
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($"Status Line: '{statusLine}'");
                Debug.Log($"{sb}");
            }
#endif

            if (string.IsNullOrEmpty(statusLine))
            {
                if (BaseRequest.Retries >= BaseRequest.MaxRetries)
                    return false;

                throw new Exception("网络错误!TCP连接在收到任何数据之前被关闭!");
            }

            if (!this.IsProxyResponse)
                BaseRequest.Timing.Add(TimingEventNames.WaitingTTFB);

            string[] versions = statusLine.Split(new char[] { '/', '.' });
            this.VersionMajor = int.Parse(versions[1]);
            this.VersionMinor = int.Parse(versions[2]);
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($"HTTP Version: '{this.VersionMajor.ToString()}.{this.VersionMinor.ToString()}'");
                Debug.Log($"{sb}");
            }
#endif
            int statusCode;
            string statusCodeStr = NoTrimReadTo(Stream, (byte)' ', LF);
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($"Status Code: '{statusCodeStr}'");
                Debug.Log($"{sb}");
            }
#endif
            if (BaseRequest.Retries >= BaseRequest.MaxRetries)
            {
                statusCode = int.Parse(statusCodeStr);
            }
            else if (!int.TryParse(statusCodeStr, out statusCode))
            {
                return false;
            }

            this.StatusCode = statusCode;

            if (statusCodeStr.Length > 0 && (byte)statusCodeStr[^1] != LF &&
                (byte)statusCodeStr[^1] != CR)
            {
                this.Message = ReadTo(Stream, LF);
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}] ");
                    sb.Append($"{sf.GetMethod().Name} ");
                    sb.Append($"Line:{sf.GetFileLineNumber()} ");
                    sb.Append($"[msg{sf.GetMethod().Name}]");
                    sb.Append($"Status Message: '{this.Message}'");
                    Debug.Log($"{sb}");
                }
#endif
            }
            else
            {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}] ");
                    sb.Append($"{sf.GetMethod().Name} ");
                    sb.Append($"Line:{sf.GetFileLineNumber()} ");
                    sb.Append($"[msg{sf.GetMethod().Name}]");
                    sb.Append($"Skipping Status Message reading!");
                    Debug.Log($"{sb}");
                }
#endif
                this.Message = string.Empty;
            }

            //Read Headers
            ReadHeaders(Stream);

            if (!this.IsProxyResponse)
            {
                BaseRequest.Timing.Add(name: TimingEventNames.Headers);
            }

            IsUpgraded = StatusCode == 101
                         && (HasHeaderWithValue(
                                 headerName: "connection",
                                 value: "upgrade")
                             || HasHeader(headerName: "upgrade"));
            /*
             * 正常HTTP协议升级为其他HTTP协议。
             */
            if (IsUpgraded)
            {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}] ");
                    sb.Append($"{sf.GetMethod().Name} ");
                    sb.Append($"Line:{sf.GetFileLineNumber()} ");
                    sb.Append($"[msg{sf.GetMethod().Name}]");
                    sb.Append($"请求升级!");
                    Debug.Log($"{sb}");
                }
#endif
                var requestEvent = new RequestEventInfo(
                    request: this.BaseRequest,
                    @event: RequestEvents.Upgraded);
                RequestEventHelper.EnqueueRequestEvent(@event: requestEvent);
            }

            if (!readPayloadData)
            {
                return true;
            }

            if (this.StatusCode == 200 && this.IsProxyResponse)
            {
                return true;
            }

            return ReadPayload(forceReadRawContentLength: forceReadRawContentLength);
        }

        private bool ReadPayload(long forceReadRawContentLength)
        {
            // 从已经打开的流中阅读 (eq. 从文件缓存或webgl下的所有响应)
            if (forceReadRawContentLength != -1)
            {
                ReadRaw(Stream, forceReadRawContentLength);
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}] ");
                    sb.Append($"{sf.GetMethod().Name} ");
                    sb.Append($"Line:{sf.GetFileLineNumber()} ");
                    sb.Append($"[msg{sf.GetMethod().Name}]");
                    sb.Append($"ReadPayload完成!");
                    Debug.Log($"{sb}");
                }
#endif
                return true;
            }

            //  http://www.w3.org/Protocols/rfc2616/rfc2616-sec4.html#sec4.4
            // 1。任何“MUST NOT”包含消息体的响应消息(例如1xx、204和304响应以及对HEAD请求的任何响应)
            //总是以报头字段之后的第一个空行结束，而不管消息中出现的实体报头字段。
            if (StatusCode is >= 100 and < 200 ||
                StatusCode == 204 ||
                StatusCode == 304 ||
                BaseRequest.MethodType == HttpMethods.Head)
            {
                return true;
            }

#if (!UNITY_WEBGL || UNITY_EDITOR)
            if (HasHeaderWithValue(
                    headerName: "transfer-encoding",
                    value: "chunked"))
            {
                ReadChunked(stream: Stream);
            }
            else
#endif
            {
                //  http://www.w3.org/Protocols/rfc2616/rfc2616-sec4.html#sec4.4
                //      Case 3 in the above link.
                var contentLengthHeaders = GetHeaderValues(name: "content-length");
                var contentRangeHeaders = GetHeaderValues(name: "content-range");
                if (contentLengthHeaders != null && contentRangeHeaders == null)
                {
                    ReadRaw(
                        stream: Stream,
                        contentLength: long.Parse(contentLengthHeaders[0]));
                }
                else if (contentRangeHeaders != null)
                {
                    if (contentLengthHeaders != null)
                    {
                        ReadRaw(
                            stream: Stream,
                            contentLength: long.Parse(contentLengthHeaders[0]));
                    }
                    else
                    {
                        var range = GetRange();
                        ReadRaw(
                            stream: Stream,
                            contentLength: (range.LastBytePos - range.FirstBytePos) + 1);
                    }
                }
                else
                {
                    ReadUnknownSize(stream: Stream);
                }
            }
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($"ReadPayload完成!");
                Debug.Log($"{sb}");
            }
#endif
            return true;
        }


        private void ReadHeaders(Stream stream)
        {
            var newHeaders = this.BaseRequest.OnHeadersReceived != null ? new Dictionary<string, List<string>>() : null;

            var headerName = ReadTo(
                stream: stream,
                blocker1: (byte)':',
                blocker2: LF) /*.Trim()*/;
            while (headerName != string.Empty)
            {
                var value = ReadTo(
                    stream: stream,
                    blocker: LF);
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}] ");
                    sb.Append($"{sf.GetMethod().Name} ");
                    sb.Append($"Line:{sf.GetFileLineNumber()} ");
                    sb.Append($"[msg{sf.GetMethod().Name}]");
                    sb.Append($"Header - '{headerName}': '{value}'");
                    Debug.Log($"{sb}");
                }
#endif

                AddHeader(
                    name: headerName,
                    value: value);

                if (newHeaders != null)
                {
                    if (!newHeaders.TryGetValue(headerName, out var values))
                    {
                        newHeaders.Add(headerName, values = new List<string>(1));
                    }

                    values.Add(value);
                }

                headerName = ReadTo(
                    stream: stream,
                    blocker1: (byte)':',
                    blocker2: LF);
            }

            if (this.BaseRequest.OnHeadersReceived != null)
            {
                var requestEvent = new RequestEventInfo(
                    request: this.BaseRequest,
                    headers: newHeaders);
                RequestEventHelper.EnqueueRequestEvent(@event: requestEvent);
            }
        }

        public void AddHeader(string name, string value)
        {
            name = name.ToLower();

            Headers ??= new Dictionary<string, List<string>>();

            if (!Headers.TryGetValue(name, out var values))
            {
                Headers.Add(name, values = new List<string>(1));
            }

            values.Add(value);

#if !BESTHTTP_DISABLE_CACHING
            var isFromCache = this.IsFromCache;
#endif
            if (!isFromCache && name.Equals("alt-svc", StringComparison.Ordinal))
            {
                PluginEventHelper.EnqueuePluginEvent(new PluginEventInfo(PluginEvents.AltSvcHeader,
                    new AltSvcEventInfo(this.BaseRequest.CurrentUri.Host, this)));
            }
        }

        /// <summary>
        /// 返回从服务器接收到的给定头名称的值列表。
        /// <remarks>备注:读取响应时所有头转换为小写。</remarks>
        /// </summary>
        /// <param name="name">标头名称</param>
        /// <returns>如果没有找到带有给定名称的头文件，或者列表中没有值(例如。Count == 0)返回null</returns>
        public List<string> GetHeaderValues(string name)
        {
            if (Headers == null)
            {
                return null;
            }

            name = name.ToLower();

            if (!Headers.TryGetValue(name, out var values) || values.Count == 0)
            {
                return null;
            }

            return values;
        }

        /// <summary>
        /// 返回头列表中的第一个值，如果没有头或值，则返回null。
        /// </summary>
        /// <param name="name">标头名称</param>
        /// <returns>如果没有找到带有给定名称的头文件，或者列表中没有值(例如。Count == 0)返回null。</returns>
        public string GetFirstHeaderValue(string name)
        {
            if (Headers == null)
            {
                return null;
            }

            name = name.ToLower();

            if (!Headers.TryGetValue(name, out var values) || values.Count == 0)
            {
                return null;
            }

            return values[0];
        }

        /// <summary>
        /// 检查是否存在具有给定名称和值的头文件。
        /// </summary>
        /// <param name="headerName">标头名称.</param>
        /// <param name="value"></param>
        /// <returns>如果存在具有给定名称和值的头文件，则返回true</returns>
        public bool HasHeaderWithValue(string headerName, string value)
        {
            var values = GetHeaderValues(headerName);
            return values != null
                   && values.Any(t => string.Compare(t, value, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// 检查是否有具有给定名称的头文件。
        /// </summary>
        /// <param name="headerName">标头名称.</param>
        /// <returns>如果存在具有给定名称的头文件，则返回true.</returns>
        public bool HasHeader(string headerName)
        {
            var values = GetHeaderValues(headerName);
            return values != null;
        }

        /// <summary>
        /// 解析“Content-Range”报头的值并返回一个HTTPRange对象。
        /// </summary>
        /// <remarks>如果服务器忽略了一个字节范围规格，因为它是语法无效的，服务器应该处理请求，如果无效的范围报头字段不存在。
        /// (通常，这意味着返回一个包含完整实体的200响应)。在这种情况下，因为没有'Content-Range'头，这个函数将返回null!</remarks>
        /// <returns>如果没有找到'Content-Range'头，则返回null.</returns>
        private HttpRange GetRange()
        {
            var rangeHeaders = GetHeaderValues("content-range");
            if (rangeHeaders == null)
            {
                return null;
            }

            //最后一个字节的pos值小于第一个字节的pos值的byte-content-range-spec
            //实例长度小于或等于其last-byte-pos值的实例长度无效。
            //接收到一个无效的byte-content-range- spec的人必须忽略它以及与它一起传输的任何内容。

            // 一个有效的内容范围示例:"bytes 500-1233/1234"
            var ranges = rangeHeaders[0].Split(new[] { ' ', '-', '/' }, StringSplitOptions.RemoveEmptyEntries);

            //服务器发送一个状态码为416的响应(请求的范围不能满足)，应该包含一个Content-Range字段，字节范围- respe -spec为"*"。
            // instance-length指定当前资源的长度。
            // "bytes */1234"
            if (ranges[1] == "*")
            {
                return new HttpRange(contentLength: int.Parse(ranges[2]));
            }

            return new HttpRange(
                firstBytePosition: int.Parse(ranges[1]),
                lastBytePosition: int.Parse(ranges[2]),
                contentLength: ranges[3] != "*" ? int.Parse(ranges[3]) : -1);
        }

        private static string ReadTo(Stream stream, byte blocker)
        {
            var readBuf = BufferPool.Get(
                size: 1024,
                canBeLarger: true);
            try
            {
                var buffoons = 0;

                var ch = stream.ReadByte();
                while (ch != blocker && ch != -1)
                {
                    if (ch > 0x7f) //replaces ostracising
                    {
                        ch = '?';
                    }

                    //如果太短，请增大缓冲区
                    if (readBuf.Length <= buffoons)
                    {
                        BufferPool.Resize(
                            buffer: ref readBuf,
                            newSize: readBuf.Length * 2,
                            canBeLarger: true,
                            clear: false);
                    }

                    if (buffoons > 0 || !char.IsWhiteSpace((char)ch)) //trims tart
                    {
                        readBuf[buffoons++] = (byte)ch;
                    }

                    ch = stream.ReadByte();
                }

                while (buffoons > 0 && char.IsWhiteSpace((char)readBuf[buffoons - 1]))
                {
                    buffoons--;
                }

                return Encoding.UTF8.GetString(
                    bytes: readBuf,
                    index: 0,
                    count: buffoons);
            }
            finally
            {
                BufferPool.Release(readBuf);
            }
        }

        private static string ReadTo(Stream stream, byte blocker1, byte blocker2)
        {
            byte[] readBuf = BufferPool.Get(
                size: 1024,
                canBeLarger: true);
            try
            {
                int buffoons = 0;

                int ch = stream.ReadByte();
                while (ch != blocker1 && ch != blocker2 && ch != -1)
                {
                    // ReSharper disable once CommentTypo
                    //replaces asciitostring
                    if (ch > 0x7f)
                    {
                        ch = '?';
                    }

                    //如果太短，请增大缓冲区
                    if (readBuf.Length <= buffoons)
                    {
                        BufferPool.Resize(ref readBuf, readBuf.Length * 2, true, true);
                    }

                    // ReSharper disable once CommentTypo
                    //trimstart
                    if (buffoons > 0 || !char.IsWhiteSpace((char)ch))
                    {
                        readBuf[buffoons++] = (byte)ch;
                    }

                    ch = stream.ReadByte();
                }

                while (buffoons > 0 && char.IsWhiteSpace((char)readBuf[buffoons - 1]))
                {
                    buffoons--;
                }

                return Encoding.UTF8.GetString(
                    bytes: readBuf,
                    index: 0,
                    count: buffoons);
            }
            finally
            {
                BufferPool.Release(buffer: readBuf);
            }
        }

        private static string NoTrimReadTo(Stream stream, byte blocker1, byte blocker2)
        {
            var readBuf = BufferPool.Get(
                size: 1024,
                canBeLarger: true);
            try
            {
                int buffoons = 0;

                var ch = stream.ReadByte();
                while (ch != blocker1 && ch != blocker2 && ch != -1)
                {
                    // ReSharper disable once CommentTypo
                    //replaces asciitostring
                    if (ch > 0x7f)
                    {
                        ch = '?';
                    }

                    //如果太短，请增大缓冲区
                    if (readBuf.Length <= buffoons)
                    {
                        BufferPool.Resize(
                            buffer: ref readBuf,
                            newSize: readBuf.Length * 2,
                            canBeLarger: true,
                            clear: true);
                    }

                    // ReSharper disable once CommentTypo
                    //trimstart
                    if (buffoons > 0 || !char.IsWhiteSpace((char)ch))
                    {
                        readBuf[buffoons++] = (byte)ch;
                    }

                    ch = stream.ReadByte();
                }

                return Encoding.UTF8.GetString(readBuf, 0, buffoons);
            }
            finally
            {
                BufferPool.Release(buffer: readBuf);
            }
        }


        private int ReadChunkLength(Stream stream)
        {
            // 到行尾，然后分割字符串，这样我们将丢弃任何可选的块扩展
            string line = ReadTo(stream, LF);
            string[] splits = line.Split(';');
            string num = splits[0];

            if (int.TryParse(num, System.Globalization.NumberStyles.AllowHexSpecifier, null, out var result))
            {
                return result;
            }

            throw new Exception($"无法将“{num}”解析为十六进制数!");
        }

        // http://www.w3.org/Protocols/rfc2616/rfc2616-sec3.html#sec3.6.1
        private void ReadChunked(Stream stream)
        {
            BeginReceiveStreamFragments();

            var contentLengthHeader = GetFirstHeaderValue("Content-Length");
            var hasContentLengthHeader = !string.IsNullOrEmpty(contentLengthHeader);
            var realLength = 0;
            if (hasContentLengthHeader)
            {
                hasContentLengthHeader = int.TryParse(contentLengthHeader, out realLength);
            }
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($"ReadChunked - hasContentLengthHeader: {hasContentLengthHeader.ToString()},");
                sb.Append($"contentLengthHeader: {contentLengthHeader} realLength: {realLength:N0}");
                Debug.Log($"{sb}");
            }
#endif
            using var output = new BufferPoolMemoryStream();
            int chunkLength = ReadChunkLength(stream);
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($"chunkLength: {chunkLength:N0}");
                Debug.Log($"{sb}");
            }
#endif

            byte[] buffer = BaseRequest.ReadBufferSizeOverride > 0
                ? BufferPool.Get(BaseRequest.ReadBufferSizeOverride, false)
                : BufferPool.Get(MinReadBufferSize, true);

            // Progress report:
            long downloaded = 0;
            long downloadLength = hasContentLengthHeader ? realLength : chunkLength;
            bool sendProgressChanged = this.BaseRequest.OnDownloadProgress != null && (this.IsSuccess
#if !BESTHTTP_DISABLE_CACHING
                    || this.IsFromCache
#endif
                );

            if (sendProgressChanged)
            {
                var requestEvent = new RequestEventInfo(
                    request: this.BaseRequest,
                    @event: RequestEvents.DownloadProgress,
                    progress: downloaded,
                    progressLength: downloadLength);
                RequestEventHelper.EnqueueRequestEvent(@event: requestEvent);
            }

            string encoding =
#if !BESTHTTP_DISABLE_CACHING
                IsFromCache
                    ? null
                    :
#endif
                    GetFirstHeaderValue("content-encoding");
            bool gzipped = !string.IsNullOrEmpty(encoding) && encoding == "gzip";

            var decompressor = gzipped ? new Decompression.GZipDecompressor(minLengthToDecompress: 256) : null;

            while (chunkLength != 0)
            {
                if (this.BaseRequest.IsCancellationRequested)
                {
                    return;
                }

                var totalBytes = 0;
                // Fill up the buffer
                do
                {
                    var tryToReadCount = Math.Min(chunkLength - totalBytes, buffer.Length);

                    var bytes = stream.Read(buffer: buffer, offset: 0, count: tryToReadCount);
                    if (bytes <= 0)
                    {
                        throw ExceptionHelper.ServerClosedTCPStream();
                    }

                    // Progress report:
                    // 将报告放在这个周期内将更频繁地报告进度
                    downloaded += bytes;

                    if (sendProgressChanged)
                    {
                        var requestEvent = new RequestEventInfo(
                            request: this.BaseRequest,
                            @event: RequestEvents.DownloadProgress,
                            progress: downloaded,
                            progressLength: downloadLength);
                        RequestEventHelper.EnqueueRequestEvent(@event: requestEvent);
                    }

                    if (BaseRequest.UseStreaming)
                    {
                        if (gzipped)
                        {
                            var decompressed = decompressor.Decompress(buffer, 0, bytes, false, true);
                            if (decompressed.Data != null)
                            {
                                FeedStreamFragment(
                                    buffer: decompressed.Data,
                                    pos: 0,
                                    length: decompressed.Length);
                            }
                        }
                        else
                        {
                            FeedStreamFragment(
                                buffer: buffer,
                                pos: 0,
                                length: bytes);
                        }
                    }
                    else
                    {
                        output.Write(
                            buffer: buffer,
                            offset: 0,
                            count: bytes);
                    }

                    totalBytes += bytes;
                } while (totalBytes < chunkLength);

                // 每个块数据都有一个尾随的CRLF
                ReadTo(stream: stream, blocker: LF);

                // 读取下一个块的长度
                chunkLength = ReadChunkLength(stream: stream);

                if (!hasContentLengthHeader)
                {
                    downloadLength += chunkLength;
                }
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}] ");
                    sb.Append($"{sf.GetMethod().Name} ");
                    sb.Append($"Line:{sf.GetFileLineNumber()} ");
                    sb.Append($"[msg{sf.GetMethod().Name}]");
                    sb.Append($"chunkLength: {chunkLength:N0}");
                    Debug.Log($"{sb}");
                }
#endif
            }

            BufferPool.Release(buffer);

            if (BaseRequest.UseStreaming)
            {
                if (gzipped)
                {
                    var decompressed = decompressor.Decompress(null, 0, 0, true, true);
                    if (decompressed.Data != null)
                    {
                        FeedStreamFragment(decompressed.Data, 0, decompressed.Length);
                    }
                }

                FlushRemainingFragmentBuffer();
            }

            // Read the trailing headers or the CRLF
            ReadHeaders(stream);

            // HTTP服务器有时会使用压缩(gzip)或deflate方法来优化传输。
            // chunked和gzip编码的交互方式由HTTP的两阶段编码决定:
            //首先将内容流编码为(content - encoding: gzip)，然后将产生的字节流编码为使用另一个编码器(transfer - encoding: chunked)传输。
            //这意味着在同时启用压缩和分块编码的情况下，块编码本身不会被压缩，并且每个块中的数据不应该被单独压缩。
            //远程终端可以解码传入流，首先使用Transfer-Encoding解码，然后使用指定的Content-Encoding解码。
            //当数据块被实时解码时，这将是一个更好的实现。因为现在必须下载整个流，然后解码。它需要更多的内存。
            if (!BaseRequest.UseStreaming)
            {
                this.Data = DecodeStream(output);
            }

            decompressor?.Dispose();
        }

        // No transfer-encoding just raw bytes.
        internal void ReadRaw(Stream stream, long contentLength)
        {
            BeginReceiveStreamFragments();

            // Progress report:
            long downloaded = 0;
            long downloadLength = contentLength;
            bool sendProgressChanged = this.BaseRequest.OnDownloadProgress != null && (this.IsSuccess
#if !BESTHTTP_DISABLE_CACHING
                    || this.IsFromCache
#endif
                );

            if (sendProgressChanged)
            {
                var requestEvent = new RequestEventInfo(
                    request: this.BaseRequest,
                    @event: RequestEvents.DownloadProgress,
                    progress: downloaded,
                    progressLength: downloadLength);
                RequestEventHelper.EnqueueRequestEvent(@event: requestEvent);
                
            }
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($"ReadRaw - contentLength: {contentLength:N0}");
                Debug.Log($"{sb}");
            }
#endif

            string encoding =
#if !BESTHTTP_DISABLE_CACHING
                IsFromCache
                    ? null
                    :
#endif
                    GetFirstHeaderValue("content-encoding");
            bool gzipped = !string.IsNullOrEmpty(encoding) && encoding == "gzip";
            Decompression.GZipDecompressor decompressor = gzipped ? new Decompression.GZipDecompressor(256) : null;

            if (!BaseRequest.UseStreaming && contentLength > 2147483646)
            {
                throw new OverflowException("You have to use STREAMING to download files bigger than 2GB!");
            }

            using (var output = new BufferPoolMemoryStream(BaseRequest.UseStreaming ? 0 : (int)contentLength))
            {
                //由于最后一个参数，缓冲区的大小可以比请求的大，但没有理由使用
                //如果池中有一个更大的可用池，则返回一个精确的大小。稍后我们将使用整个缓冲区。
                byte[] buffer = BaseRequest.ReadBufferSizeOverride > 0
                    ? BufferPool.Get(BaseRequest.ReadBufferSizeOverride, false)
                    : BufferPool.Get(MinReadBufferSize, true);

                while (contentLength > 0)
                {
                    if (this.BaseRequest.IsCancellationRequested)
                    {
                        return;
                    }

                    var readBytes = 0;

                    do
                    {
                        // tryToReadCount包含我们一次要读入多少字节。我们试着把缓冲区全部读入一次，
                        //但是要限制剩余的contentLength。
                        int tryToReadCount = (int)Math.Min(
                            Math.Min(int.MaxValue, contentLength),
                            buffer.Length - readBytes);

                        int bytes = stream.Read(buffer, readBytes, tryToReadCount);

                        if (bytes <= 0)
                        {
                            throw ExceptionHelper.ServerClosedTCPStream();
                        }

                        readBytes += bytes;
                        contentLength -= bytes;

                        // Progress report:
                        if (!sendProgressChanged) continue;
                        downloaded += bytes;
                        var requestEvent = new RequestEventInfo(
                            request: this.BaseRequest,
                            @event: RequestEvents.DownloadProgress,
                            progress: downloaded,
                            progressLength: downloadLength);
                        RequestEventHelper.EnqueueRequestEvent(@event: requestEvent);
                        
                    } while (readBytes < buffer.Length && contentLength > 0);

                    if (BaseRequest.UseStreaming)
                    {
                        if (gzipped)
                        {
                            var decompressed = decompressor.Decompress(buffer, 0, readBytes, false, true);
                            if (decompressed.Data != null)
                            {
                                FeedStreamFragment(decompressed.Data, 0, decompressed.Length);
                            }
                        }
                        else
                        {
                            FeedStreamFragment(buffer, 0, readBytes);
                        }
                    }
                    else
                    {
                        output.Write(buffer, 0, readBytes);
                    }
                }


                BufferPool.Release(buffer);

                if (BaseRequest.UseStreaming)
                {
                    if (gzipped)
                    {
                        var decompressed = decompressor.Decompress(null, 0, 0, true, true);
                        if (decompressed.Data != null)
                        {
                            FeedStreamFragment(decompressed.Data, 0, decompressed.Length);
                        }
                    }

                    FlushRemainingFragmentBuffer();
                }

                if (!BaseRequest.UseStreaming)
                {
                    this.Data = DecodeStream(output);
                }
            }

            decompressor?.Dispose();
        }

        private void ReadUnknownSize(Stream stream)
        {
            // Progress report:
            long downloaded = 0;
            long downloadLength = 0;
            bool sendProgressChanged = this.BaseRequest.OnDownloadProgress != null
                                       && (this.IsSuccess
#if !BESTHTTP_DISABLE_CACHING
                                           || this.IsFromCache
#endif
                                       );

            if (sendProgressChanged)
            {
                var requestEvent = new RequestEventInfo(
                    request: this.BaseRequest,
                    @event: RequestEvents.DownloadProgress,
                    progress: downloaded,
                    progressLength: downloadLength);
                RequestEventHelper.EnqueueRequestEvent(@event: requestEvent);
            }

            string encoding =
#if !BESTHTTP_DISABLE_CACHING
                IsFromCache
                    ? null
                    :
#endif
                    GetFirstHeaderValue("content-encoding");
            bool gzipped = !string.IsNullOrEmpty(encoding) && encoding == "gzip";
            Decompression.GZipDecompressor decompressor = gzipped ? new Decompression.GZipDecompressor(256) : null;

            using (var output = new BufferPoolMemoryStream())
            {
                byte[] buffer = BaseRequest.ReadBufferSizeOverride > 0
                    ? BufferPool.Get(BaseRequest.ReadBufferSizeOverride, false)
                    : BufferPool.Get(MinReadBufferSize, true);
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}] ");
                    sb.Append($"{sf.GetMethod().Name} ");
                    sb.Append($"Line:{sf.GetFileLineNumber()} ");
                    sb.Append($"[msg{sf.GetMethod().Name}]");
                    sb.Append($"ReadUnknownSize - buffer size: {buffer.Length:N0}");
                    Debug.Log($"{sb}");
                }
#endif
                int bytes;
                do
                {
                    var readBytes = 0;

                    do
                    {
                        if (this.BaseRequest.IsCancellationRequested)
                        {
                            return;
                        }

                        bytes = 0;

#if !NETFX_CORE || UNITY_EDITOR
                        // 如果我们有好的旧的NetworkStream，那么我们可以使用DataAvailable属性。在WP8平台上，这些都被省略了…… :/
                        if (stream is NetworkStream networkStream
                            && BaseRequest.EnableSafeReadOnUnknownContentLength)
                        {
                            for (var i = readBytes; i < buffer.Length && networkStream.DataAvailable; ++i)
                            {
                                var read = stream.ReadByte();
                                if (read >= 0)
                                {
                                    buffer[i] = (byte)read;
                                    bytes++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else // 不管怎样这都很好，只是有点慢。
#endif
                        {
                            bytes = stream.Read(buffer, readBytes, buffer.Length - readBytes);
                        }

                        readBytes += bytes;

                        // Progress report:
                        downloaded += bytes;
                        downloadLength = downloaded;

                        if (sendProgressChanged)
                        {
                            var requestEvent = new RequestEventInfo(
                                request: this.BaseRequest,
                                @event: RequestEvents.DownloadProgress,
                                progress: downloaded,
                                progressLength: downloadLength);
                            RequestEventHelper.EnqueueRequestEvent(@event: requestEvent);
                        }
                    } while (readBytes < buffer.Length && bytes > 0);

                    if (BaseRequest.UseStreaming)
                    {
                        if (gzipped)
                        {
                            var decompressed = decompressor.Decompress(
                                data: buffer,
                                offset: 0,
                                count: readBytes,
                                forceDecompress: false,
                                dataCanBeLarger: true);
                            if (decompressed.Data != null)
                            {
                                FeedStreamFragment(
                                    buffer: decompressed.Data,
                                    pos: 0,
                                    length: decompressed.Length);
                            }
                        }
                        else
                        {
                            FeedStreamFragment(
                                buffer: buffer,
                                pos: 0,
                                length: readBytes);
                        }
                    }
                    else if (readBytes > 0)
                    {
                        output.Write(
                            buffer: buffer,
                            offset: 0,
                            count: readBytes);
                    }
                } while (bytes > 0);

                BufferPool.Release(buffer: buffer);

                if (BaseRequest.UseStreaming)
                {
                    if (gzipped)
                    {
                        var decompressed = decompressor.Decompress(
                            data: null,
                            offset: 0,
                            count: 0,
                            forceDecompress: true,
                            dataCanBeLarger: true);
                        if (decompressed.Data != null)
                        {
                            FeedStreamFragment(
                                buffer: decompressed.Data,
                                pos: 0,
                                length: decompressed.Length);
                        }
                    }

                    FlushRemainingFragmentBuffer();
                }

                if (!BaseRequest.UseStreaming)
                {
                    this.Data = DecodeStream(streamToDecode: output);
                }
            }

            decompressor?.Dispose();
        }

        private byte[] DecodeStream(BufferPoolMemoryStream streamToDecode)
        {
            streamToDecode.Seek(0, SeekOrigin.Begin);

            // 缓存存储解码后的数据
            var encoding =
#if !BESTHTTP_DISABLE_CACHING
                IsFromCache
                    ? null
                    :
#endif
                    GetHeaderValues("content-encoding");

#if !UNITY_WEBGL || UNITY_EDITOR
            Stream decoderStream;
#endif

            // 如果没有使用编码，则提早返回。
            if (encoding == null)
            {
                return streamToDecode.ToArray();
            }
            else
            {
                switch (encoding[0])
                {
#if !UNITY_WEBGL || UNITY_EDITOR
                    case "gzip":
                    {
                        decoderStream = new Decompression.Zlib.GZipStream(
                            stream: streamToDecode,
                            mode: Decompression.Zlib.CompressionMode.Decompress);
                    }
                        break;
                    case "deflate":
                    {
                        decoderStream = new Decompression.Zlib.DeflateStream(
                            stream: streamToDecode,
                            mode: Decompression.Zlib.CompressionMode.Decompress);
                    }
                        break;
#endif
                    //identity, utf-8, etc.
                    default:
                    {
                        // 不复制从一个流到另一个，只是返回与原始字节
                        return streamToDecode.ToArray();
                    }
                }
            }

#if !UNITY_WEBGL || UNITY_EDITOR
            using var ms = new BufferPoolMemoryStream(capacity: (int)streamToDecode.Length);
            var buf = BufferPool.Get(
                size: 1024,
                canBeLarger: true);
            int byteCount;

            while ((byteCount = decoderStream.Read(
                       buffer: buf,
                       offset: 0,
                       count: buf.Length)) > 0)
            {
                ms.Write(
                    buffer: buf,
                    offset: 0,
                    count: byteCount);
            }

            BufferPool.Release(buffer: buf);

            decoderStream.Dispose();
            return ms.ToArray();
#endif
        }

        protected void BeginReceiveStreamFragments()
        {
#if !BESTHTTP_DISABLE_CACHING
            if (
                /*
                 * 使用此属性，可以在每个请求的基础上启用/禁用缓存。
                 * !true 代表需要禁用
                 * ！false 启用
                 */
                !BaseRequest.DisableCache
                /*
                * 如果它为真，每次如果我们可以发送至少一个片段，Callback将被调用。
                */
                && BaseRequest.UseStreaming)
            {
                // 如果缓存被启用，并且响应不是来自缓存，它是可缓存的，我们将缓存下载的数据。
                if (!IsFromCache && HttpCacheService.IsCacheable(
                        uri: BaseRequest.CurrentUri,
                        method: BaseRequest.MethodType,
                        response: this))
                {
                    _cacheStream = HttpCacheService.PrepareStreamed(
                        uri: BaseRequest.CurrentUri,
                        response: this);
                }
            }
#endif
            _allFragmentSize = 0;
        }

        /// <summary>
        /// 向片段列表中添加数据。
        /// </summary>
        /// <param name="buffer">要添加的缓冲区.</param>
        /// <param name="pos">我们开始复制数据的位置.</param>
        /// <param name="length">我们要复制多少数据.</param>
        protected void FeedStreamFragment(byte[] buffer, int pos, int length)
        {
            /*
             * 发送过来的是否有数据
             */
            if (buffer == null || length == 0)
            {
                return;
            }

            // 如果从缓存中读取，我们不想读取太多的数据到内存中。因此，我们将等待加载的片段处理完毕。
#if !UNITY_WEBGL || UNITY_EDITOR
#if CSHARP_7_3_OR_NEWER
            SpinWait spinWait = new SpinWait();
#endif

            while (!this.BaseRequest.IsCancellationRequested &&
                   this.BaseRequest.State == HttpRequestStates.Processing &&
                   BaseRequest.UseStreaming &&
                   FragmentQueueIsFull())
            {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}] ");
                    sb.Append($"{sf.GetMethod().Name} ");
                    sb.Append($"Line:{sf.GetFileLineNumber()} ");
                    sb.Append($"[msg{sf.GetMethod().Name}]");
                    sb.Append($"WaitWhileFragmentQueueIsFull");
                    Debug.Log($"{sb}");
                }
#endif

#if CSHARP_7_3_OR_NEWER
                spinWait.SpinOnce();
#elif !NETFX_CORE
                System.Threading.Thread.Sleep(1);
#endif
            }
#endif

            if (_fragmentBuffer == null)
            {
                _fragmentBuffer = BufferPool.Get(
                    size: BaseRequest.StreamFragmentSize,
                    canBeLarger: true);
                _fragmentBufferDataLength = 0;
            }

            if (_fragmentBufferDataLength + length <= _fragmentBuffer.Length)
            {
                Array.Copy(
                    sourceArray: buffer,
                    sourceIndex: pos,
                    destinationArray: _fragmentBuffer,
                    destinationIndex: _fragmentBufferDataLength,
                    length: length);
                _fragmentBufferDataLength += length;

                if (_fragmentBufferDataLength != _fragmentBuffer.Length
                    && !BaseRequest.StreamChunksImmediately)
                {
                    return;
                }

                AddStreamedFragment(
                    buffer: _fragmentBuffer,
                    bufferLength: _fragmentBufferDataLength);
                _fragmentBuffer = null;
                _fragmentBufferDataLength = 0;
            }
            else
            {
                var remaining = _fragmentBuffer.Length - _fragmentBufferDataLength;

                FeedStreamFragment(
                    buffer: buffer,
                    pos: pos,
                    length: remaining);
                // ReSharper disable once TailRecursiveCall
                FeedStreamFragment(
                    buffer: buffer,
                    pos: (pos + remaining),
                    length: (length - remaining));
            }
        }

        protected void FlushRemainingFragmentBuffer()
        {
            if (_fragmentBuffer != null)
            {
                AddStreamedFragment(
                    buffer: _fragmentBuffer,
                    bufferLength: _fragmentBufferDataLength);
                _fragmentBuffer = null;
                _fragmentBufferDataLength = 0;
            }

#if !BESTHTTP_DISABLE_CACHING
            if (_cacheStream == null) return;
            _cacheStream.Dispose();
            _cacheStream = null;

            HttpCacheService.SetBodyLength(
                uri: BaseRequest.CurrentUri,
                bodyLength: _allFragmentSize);
#endif
        }

#if NET_STANDARD_2_0 || NETFX_CORE
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private void AddStreamedFragment(byte[] buffer, int bufferLength)
        {
#if !BESTHTTP_DISABLE_CACHING
            if (!IsCacheOnly)
#endif
            {
                if (this.BaseRequest.UseStreaming
                    && buffer != null
                    && bufferLength > 0)
                {
                    var requestEvent = new RequestEventInfo(
                        request: this.BaseRequest,
                        data: buffer,
                        dataLength: bufferLength);
                    RequestEventHelper.EnqueueRequestEvent(@event: requestEvent);
                    Interlocked.Increment(ref this.UnprocessedFragments);
                }
            }
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($"AddStreamedFragment缓冲区长度: {bufferLength:N0}");
                sb.Append($" UnprocessedFragments: {Interlocked.Read(ref this.UnprocessedFragments):N0}");
                Debug.Log($"{sb}");
            }
#endif

#if !BESTHTTP_DISABLE_CACHING
            if (_cacheStream == null) return;
            if (buffer != null)
            {
                _cacheStream.Write(
                    buffer: buffer,
                    offset: 0,
                    count: bufferLength);
            }

            _allFragmentSize += bufferLength;
#endif
        }

#if NET_STANDARD_2_0 || NETFX_CORE
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool FragmentQueueIsFull()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            var unprocessedFragments = Interlocked.Read(ref UnprocessedFragments);

            var result = unprocessedFragments >= BaseRequest.MaxFragmentQueueLength;
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($"FragmentQueueIsFull - {unprocessedFragments}");
                sb.Append($" / {BaseRequest.MaxFragmentQueueLength}");
                Debug.Log($"{sb}");
            }
#endif

            return result;
#else
            return false;
#endif
        }

        /// <summary>
        /// IDisposable实现。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            // 释放资源，如果我们正在使用ReadOnlyBufferedStream，它将不会关闭它的内部流。
            // 否则，关闭(内部)流是连接的责任
            if (Stream is ReadOnlyBufferedStream _)
            {
                ((IDisposable)Stream).Dispose();
            }

            Stream = null;

#if !BESTHTTP_DISABLE_CACHING
            if (_cacheStream == null) return;
            _cacheStream.Dispose();
            _cacheStream = null;
#endif
        }
    }
}