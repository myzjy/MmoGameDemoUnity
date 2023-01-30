using System;
using System.Collections.Generic;
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

        #region Public Properties

        public int VersionMajor { get; protected set; }

        public int VersionMinor { get; protected set; }

        /// <summary>
        /// 从服务器发送的状态码。
        /// </summary>
        public int StatusCode { get; protected set; }

        /// <summary>
        /// 如果状态码在[200..]范围内，返回true。300[或304(未经修改)
        /// </summary>
        public bool IsSuccess => (this.StatusCode >= 200 && this.StatusCode < 300) || this.StatusCode == 304;

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
        /// 如果IsCacheOnly和isstreaming都为true, OnStreamingData不会被调用。
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
        public LoggingContext Context { get; private set; }

        /// <summary>
        /// HTTPManager请求事件队列中的流数据片段的计数。
        /// </summary>
#if UNITY_EDITOR
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        internal long UnprocessedFragments;

        #endregion

        #region 内部字段

        internal readonly HttpRequest BaseRequest;

        #endregion

        #region 受保护属性和字段

        protected Stream Stream;

        private byte[] _fragmentBuffer;
        private int _fragmentBufferDataLength;
#if !BESTHTTP_DISABLE_CACHING
        private Stream _cacheStream;
#endif
        private int _allFragmentSize;

        #endregion

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

        public HttpResponse(HttpRequest request, Stream stream, bool isStreamed, bool isFromCache,
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

        public bool Receive(long forceReadRawContentLength = -1, bool readPayloadData = true,
            bool sendUpgradedEvent = true)
        {
            if (this.BaseRequest.IsCancellationRequested)
                return false;

            string statusLine;

            if (HttpManager.Logger.Level == Loglevels.All)
                VerboseLogging(
                    $"Receive. forceReadRawContentLength: '{forceReadRawContentLength:N0}', readPayloadData: '{readPayloadData}'");

            // 在WP平台上，我们不能确定tcp连接是否关闭。
            //  因此，如果我们在这里得到一个异常，我们需要重新创建连接。
            try
            {
                // Read out 'HTTP/1.1' from the "HTTP/1.1 {StatusCode} {Message}"
                statusLine = ReadTo(Stream, (byte)' ');
            }
            catch
            {
                if (BaseRequest.IsCancellationRequested)
                {
                    return false;
                }

                if (BaseRequest.Retries >= BaseRequest.MaxRetries)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.Log("[HTTPResponse] [msg:读取状态行失败!启用Retry，返回false.] []");
#endif
                    // HttpManager.Logger.Warning("HTTPResponse",
                    //     "读取状态行失败!启用Retry，返回false.", this.Context,
                    //     this.BaseRequest.Context);
                    return false;
                }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log("[HTTPResponse] [msg:取状态行失败!禁用重试，重新抛出异常。] []");
#endif
                // HttpManager.Logger.Warning("HTTPResponse",
                //     "读取状态行失败!禁用重试，重新抛出异常。", this.Context,
                //     this.BaseRequest.Context);
                throw;
            }

            if (HttpManager.Logger.Level == Loglevels.All)
                VerboseLogging($"Status Line: '{statusLine}'");

            if (string.IsNullOrEmpty(statusLine))
            {
                if (BaseRequest.Retries >= BaseRequest.MaxRetries)
                    return false;

                throw new Exception("Network error! TCP Connection got closed before receiving any data!");
            }

            if (!this.IsProxyResponse)
                BaseRequest.Timing.Add(TimingEventNames.Waiting_TTFB);

            string[] versions = statusLine.Split(new char[] { '/', '.' });
            this.VersionMajor = int.Parse(versions[1]);
            this.VersionMinor = int.Parse(versions[2]);

            if (HttpManager.Logger.Level == Loglevels.All)
                VerboseLogging(string.Format("HTTP Version: '{0}.{1}'", this.VersionMajor.ToString(),
                    this.VersionMinor.ToString()));

            int statusCode;
            string statusCodeStr = NoTrimReadTo(Stream, (byte)' ', LF);

            if (HttpManager.Logger.Level == Loglevels.All)
                VerboseLogging($"Status Code: '{statusCodeStr}'");

            if (BaseRequest.Retries >= BaseRequest.MaxRetries)
                statusCode = int.Parse(statusCodeStr);
            else if (!int.TryParse(statusCodeStr, out statusCode))
                return false;

            this.StatusCode = statusCode;

            if (statusCodeStr.Length > 0 && (byte)statusCodeStr[statusCodeStr.Length - 1] != LF &&
                (byte)statusCodeStr[statusCodeStr.Length - 1] != CR)
            {
                this.Message = ReadTo(Stream, LF);
                if (HttpManager.Logger.Level == Loglevels.All)
                    VerboseLogging($"Status Message: '{this.Message}'");
            }
            else
            {
                HttpManager.Logger.Warning("HTTPResponse", "Skipping Status Message reading!", this.Context,
                    this.BaseRequest.Context);

                this.Message = string.Empty;
            }

            //Read Headers
            ReadHeaders(Stream);

            if (!this.IsProxyResponse)
                BaseRequest.Timing.Add(TimingEventNames.Headers);

            IsUpgraded = StatusCode == 101 && (HasHeaderWithValue("connection", "upgrade") || HasHeader("upgrade"));

            if (IsUpgraded)
            {
                if (HttpManager.Logger.Level == Loglevels.All)
                    VerboseLogging("Request Upgraded!");

                RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.BaseRequest, RequestEvents.Upgraded));
            }

            if (!readPayloadData)
                return true;

            if (this.StatusCode == 200 && this.IsProxyResponse)
                return true;

            return ReadPayload(forceReadRawContentLength);
        }

        private bool ReadPayload(long forceReadRawContentLength)
        {
            // Reading from an already unpacked stream (eq. From a file cache or all responses under webgl)
            if (forceReadRawContentLength != -1)
            {
                ReadRaw(Stream, forceReadRawContentLength);

                if (HttpManager.Logger.Level == Loglevels.All)
                    VerboseLogging("ReadPayload Finished!");
                return true;
            }

            //  http://www.w3.org/Protocols/rfc2616/rfc2616-sec4.html#sec4.4
            //  1.Any response message which "MUST NOT" include a message-body (such as the 1xx, 204, and 304 responses and any response to a HEAD request)
            //      is always terminated by the first empty line after the header fields, regardless of the entity-header fields present in the message.
            if ((StatusCode >= 100 && StatusCode < 200) || StatusCode == 204 || StatusCode == 304 ||
                BaseRequest.MethodType == HttpMethods.Head)
                return true;

#if (!UNITY_WEBGL || UNITY_EDITOR)
            if (HasHeaderWithValue("transfer-encoding", "chunked"))
                ReadChunked(Stream);
            else
#endif
            {
                //  http://www.w3.org/Protocols/rfc2616/rfc2616-sec4.html#sec4.4
                //      Case 3 in the above link.
                List<string> contentLengthHeaders = GetHeaderValues("content-length");
                var contentRangeHeaders = GetHeaderValues("content-range");
                if (contentLengthHeaders != null && contentRangeHeaders == null)
                    ReadRaw(Stream, long.Parse(contentLengthHeaders[0]));
                else if (contentRangeHeaders != null)
                {
                    if (contentLengthHeaders != null)
                        ReadRaw(Stream, long.Parse(contentLengthHeaders[0]));
                    else
                    {
                        HttpRange range = GetRange();
                        ReadRaw(Stream, (range.LastBytePos - range.FirstBytePos) + 1);
                    }
                }
                else
                    ReadUnknownSize(Stream);
            }

            if (HttpManager.Logger.Level == Loglevels.All)
                VerboseLogging("ReadPayload Finished!");

            return true;
        }

        #region Header Management

        private void ReadHeaders(Stream stream)
        {
            var newHeaders = this.BaseRequest.OnHeadersReceived != null ? new Dictionary<string, List<string>>() : null;

            string headerName = ReadTo(stream, (byte)':', LF) /*.Trim()*/;
            while (headerName != string.Empty)
            {
                string value = ReadTo(stream, LF);

                if (HttpManager.Logger.Level == Loglevels.All)
                    VerboseLogging($"Header - '{headerName}': '{value}'");

                AddHeader(headerName, value);

                if (newHeaders != null)
                {
                    if (!newHeaders.TryGetValue(headerName, out var values))
                        newHeaders.Add(headerName, values = new List<string>(1));

                    values.Add(value);
                }

                headerName = ReadTo(stream, (byte)':', LF);
            }

            if (this.BaseRequest.OnHeadersReceived != null)
                RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.BaseRequest, newHeaders));
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
        /// Returns the list of values that received from the server for the given header name.
        /// <remarks>Remarks: All headers converted to lowercase while reading the response.</remarks>
        /// </summary>
        /// <param name="name">Name of the header</param>
        /// <returns>If no header found with the given name or there are no values in the list (eg. Count == 0) returns null.</returns>
        public List<string> GetHeaderValues(string name)
        {
            if (Headers == null)
                return null;

            name = name.ToLower();

            if (!Headers.TryGetValue(name, out var values) || values.Count == 0)
            {
                return null;
            }

            return values;
        }

        /// <summary>
        /// Returns the first value in the header list or null if there are no header or value.
        /// </summary>
        /// <param name="name">Name of the header</param>
        /// <returns>If no header found with the given name or there are no values in the list (eg. Count == 0) returns null.</returns>
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
        /// Checks if there is a header with the given name and value.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="value"></param>
        /// <returns>Returns true if there is a header with the given name and value.</returns>
        public bool HasHeaderWithValue(string headerName, string value)
        {
            var values = GetHeaderValues(headerName);
            if (values == null)
            {
                return false;
            }

            return values.Any(t => string.Compare(t, value, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Checks if there is a header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>Returns true if there is a header with the given name.</returns>
        public bool HasHeader(string headerName)
        {
            var values = GetHeaderValues(headerName);
            if (values == null)
                return false;

            return true;
        }

        /// <summary>
        /// Parses the 'Content-Range' header's value and returns a HTTPRange object.
        /// </summary>
        /// <remarks>If the server ignores a byte-range-spec because it is syntactically invalid, the server SHOULD treat the request as if the invalid Range header field did not exist.
        /// (Normally, this means return a 200 response containing the full entity). In this case because of there are no 'Content-Range' header, this function will return null!</remarks>
        /// <returns>Returns null if no 'Content-Range' header found.</returns>
        private HttpRange GetRange()
        {
            var rangeHeaders = GetHeaderValues("content-range");
            if (rangeHeaders == null)
            {
                return null;
            }

            // A byte-content-range-spec with a byte-range-resp-spec whose last- byte-pos value is less than its first-byte-pos value,
            //  or whose instance-length value is less than or equal to its last-byte-pos value, is invalid.
            // The recipient of an invalid byte-content-range- spec MUST ignore it and any content transferred along with it.

            // A valid content-range sample: "bytes 500-1233/1234"
            var ranges = rangeHeaders[0].Split(new[] { ' ', '-', '/' }, StringSplitOptions.RemoveEmptyEntries);

            // A server sending a response with status code 416 (Requested range not satisfiable) SHOULD include a Content-Range field with a byte-range-resp-spec of "*".
            // The instance-length specifies the current length of the selected resource.
            // "bytes */1234"
            if (ranges[1] == "*")
            {
                return new HttpRange(int.Parse(ranges[2]));
            }

            return new HttpRange(int.Parse(ranges[1]), int.Parse(ranges[2]),
                ranges[3] != "*" ? int.Parse(ranges[3]) : -1);
        }

        #endregion

        #region Static Stream Management Helper Functions

        private static string ReadTo(Stream stream, byte blocker)
        {
            byte[] readBuf = BufferPool.Get(1024, true);
            try
            {
                int bufpos = 0;

                int ch = stream.ReadByte();
                while (ch != blocker && ch != -1)
                {
                    if (ch > 0x7f) //replaces asciitostring
                        ch = '?';

                    //make buffer larger if too short
                    if (readBuf.Length <= bufpos)
                    {
                        BufferPool.Resize(ref readBuf, readBuf.Length * 2, true, false);
                    }

                    if (bufpos > 0 || !char.IsWhiteSpace((char)ch)) //trimstart
                    {
                        readBuf[bufpos++] = (byte)ch;
                    }

                    ch = stream.ReadByte();
                }

                while (bufpos > 0 && char.IsWhiteSpace((char)readBuf[bufpos - 1]))
                {
                    bufpos--;
                }

                return Encoding.UTF8.GetString(readBuf, 0, bufpos);
            }
            finally
            {
                BufferPool.Release(readBuf);
            }
        }

        private static string ReadTo(Stream stream, byte blocker1, byte blocker2)
        {
            byte[] readBuf = BufferPool.Get(1024, true);
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

                    //make buffer larger if too short
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
                    buffoons--;

                return Encoding.UTF8.GetString(readBuf, 0, buffoons);
            }
            finally
            {
                BufferPool.Release(readBuf);
            }
        }

        private static string NoTrimReadTo(Stream stream, byte blocker1, byte blocker2)
        {
            byte[] readBuf = BufferPool.Get(1024, true);
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

                    //make buffer larger if too short
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

                return Encoding.UTF8.GetString(readBuf, 0, buffoons);
            }
            finally
            {
                BufferPool.Release(readBuf);
            }
        }

        #endregion

        #region Read Chunked Body

        private int ReadChunkLength(Stream stream)
        {
            // Read until the end of line, then split the string so we will discard any optional chunk extensions
            string line = ReadTo(stream, LF);
            string[] splits = line.Split(';');
            string num = splits[0];

            if (int.TryParse(num, System.Globalization.NumberStyles.AllowHexSpecifier, null, out var result))
            {
                return result;
            }

            throw new Exception($"Can't parse '{num}' as a hex number!");
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

            if (HttpManager.Logger.Level == Loglevels.All)
            {
                VerboseLogging(string.Format(
                    "ReadChunked - hasContentLengthHeader: {0}, contentLengthHeader: {1} realLength: {2:N0}",
                    hasContentLengthHeader.ToString(), contentLengthHeader, realLength));
            }

            using var output = new BufferPoolMemoryStream();
            int chunkLength = ReadChunkLength(stream);

            if (HttpManager.Logger.Level == Loglevels.All)
            {
                VerboseLogging($"chunkLength: {chunkLength:N0}");
            }

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
                RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.BaseRequest,
                    RequestEvents.DownloadProgress, downloaded, downloadLength));

            string encoding =
#if !BESTHTTP_DISABLE_CACHING
                IsFromCache
                    ? null
                    :
#endif
                    GetFirstHeaderValue("content-encoding");
            bool gzipped = !string.IsNullOrEmpty(encoding) && encoding == "gzip";

            var decompressor = gzipped ? new Decompression.GZipDecompressor(256) : null;

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

                    var bytes = stream.Read(buffer, 0, tryToReadCount);
                    if (bytes <= 0)
                    {
                        throw ExceptionHelper.ServerClosedTCPStream();
                    }

                    // Progress report:
                    // Placing reporting inside this cycle will report progress much more frequent
                    downloaded += bytes;

                    if (sendProgressChanged)
                    {
                        RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.BaseRequest,
                            RequestEvents.DownloadProgress, downloaded, downloadLength));
                    }

                    if (BaseRequest.UseStreaming)
                    {
                        if (gzipped)
                        {
                            var decompressed = decompressor.Decompress(buffer, 0, bytes, false, true);
                            if (decompressed.Data != null)
                            {
                                FeedStreamFragment(decompressed.Data, 0, decompressed.Length);
                            }
                        }
                        else
                        {
                            FeedStreamFragment(buffer, 0, bytes);
                        }
                    }
                    else
                    {
                        output.Write(buffer, 0, bytes);
                    }

                    totalBytes += bytes;
                } while (totalBytes < chunkLength);

                // Every chunk data has a trailing CRLF
                ReadTo(stream, LF);

                // read the next chunk's length
                chunkLength = ReadChunkLength(stream);

                if (!hasContentLengthHeader)
                {
                    downloadLength += chunkLength;
                }

                if (HttpManager.Logger.Level == Loglevels.All)
                {
                    VerboseLogging($"chunkLength: {chunkLength:N0}");
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

            // Read the trailing headers or the CRLF
            ReadHeaders(stream);

            // HTTP servers sometimes use compression (gzip) or deflate methods to optimize transmission.
            // How both chunked and gzip encoding interact is dictated by the two-staged encoding of HTTP:
            //  first the content stream is encoded as (Content-Encoding: gzip), after which the resulting byte stream is encoded for transfer using another encoder (Transfer-Encoding: chunked).
            //  This means that in case both compression and chunked encoding are enabled, the chunk encoding itself is not compressed, and the data in each chunk should not be compressed individually.
            //  The remote endpoint can decode the incoming stream by first decoding it with the Transfer-Encoding, followed by the specified Content-Encoding.
            // It would be a better implementation when the chunk would be decododed on-the-fly. Becouse now the whole stream must be downloaded, and then decoded. It needs more memory.
            if (!BaseRequest.UseStreaming)
            {
                this.Data = DecodeStream(output);
            }

            if (decompressor != null)
            {
                decompressor.Dispose();
            }
        }

        #endregion

        #region Read Raw Body

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
                RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.BaseRequest,
                    RequestEvents.DownloadProgress, downloaded, downloadLength));
            }

            if (HttpManager.Logger.Level == Loglevels.All)
            {
                VerboseLogging($"ReadRaw - contentLength: {contentLength:N0}");
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

            if (!BaseRequest.UseStreaming && contentLength > 2147483646)
            {
                throw new OverflowException("You have to use STREAMING to download files bigger than 2GB!");
            }

            using (var output = new BufferPoolMemoryStream(BaseRequest.UseStreaming ? 0 : (int)contentLength))
            {
                // Because of the last parameter, buffer's size can be larger than the requested but there's no reason to use
                //  an exact sized one if there's an larger one available in the pool. Later we will use the whole buffer.
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
                        // tryToReadCount contain how much bytes we want to read in once. We try to read the buffer fully in once, 
                        //  but with a limit of the remaining contentLength.
                        int tryToReadCount = (int)Math.Min(Math.Min(int.MaxValue, contentLength),
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
                        RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.BaseRequest,
                            RequestEvents.DownloadProgress, downloaded, downloadLength));
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

        #endregion

        #region Read Unknown Size

        private void ReadUnknownSize(Stream stream)
        {
            // Progress report:
            long downloaded = 0;
            long downloadLength = 0;
            bool sendProgressChanged = this.BaseRequest.OnDownloadProgress != null && (this.IsSuccess
#if !BESTHTTP_DISABLE_CACHING
                    || this.IsFromCache
#endif
                );

            if (sendProgressChanged)
                RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.BaseRequest,
                    RequestEvents.DownloadProgress, downloaded, downloadLength));

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

                if (HttpManager.Logger.Level == Loglevels.All)
                {
                    VerboseLogging($"ReadUnknownSize - buffer size: {buffer.Length:N0}");
                }

                int bytes;
                do
                {
                    var readBytes = 0;

                    do
                    {
                        if (this.BaseRequest.IsCancellationRequested)
                            return;

                        bytes = 0;

#if !NETFX_CORE || UNITY_EDITOR
                        // If we have the good-old NetworkStream, than we can use the DataAvailable property. On WP8 platforms, these are omitted... :/
                        if (stream is NetworkStream networkStream && BaseRequest.EnableSafeReadOnUnknownContentLength)
                        {
                            for (int i = readBytes; i < buffer.Length && networkStream.DataAvailable; ++i)
                            {
                                int read = stream.ReadByte();
                                if (read >= 0)
                                {
                                    buffer[i] = (byte)read;
                                    bytes++;
                                }
                                else
                                    break;
                            }
                        }
                        else // This will be good anyway, but a little slower.
#endif
                        {
                            bytes = stream.Read(buffer, readBytes, buffer.Length - readBytes);
                        }

                        readBytes += bytes;

                        // Progress report:
                        downloaded += bytes;
                        downloadLength = downloaded;

                        if (sendProgressChanged)
                            RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.BaseRequest,
                                RequestEvents.DownloadProgress, downloaded, downloadLength));
                    } while (readBytes < buffer.Length && bytes > 0);

                    if (BaseRequest.UseStreaming)
                    {
                        if (gzipped)
                        {
                            var decompressed = decompressor.Decompress(buffer, 0, readBytes, false, true);
                            if (decompressed.Data != null)
                                FeedStreamFragment(decompressed.Data, 0, decompressed.Length);
                        }
                        else
                            FeedStreamFragment(buffer, 0, readBytes);
                    }
                    else if (readBytes > 0)
                        output.Write(buffer, 0, readBytes);
                } while (bytes > 0);

                BufferPool.Release(buffer);

                if (BaseRequest.UseStreaming)
                {
                    if (gzipped)
                    {
                        var decompressed = decompressor.Decompress(null, 0, 0, true, true);
                        if (decompressed.Data != null)
                            FeedStreamFragment(decompressed.Data, 0, decompressed.Length);
                    }

                    FlushRemainingFragmentBuffer();
                }

                if (!BaseRequest.UseStreaming)
                    this.Data = DecodeStream(output);
            }

            if (decompressor != null)
                decompressor.Dispose();
        }

        #endregion

        #region Stream Decoding

        private byte[] DecodeStream(BufferPoolMemoryStream streamToDecode)
        {
            streamToDecode.Seek(0, SeekOrigin.Begin);

            // The cache stores the decoded data
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

            // Return early if there are no encoding used.
            if (encoding == null)
                return streamToDecode.ToArray();
            else
            {
                switch (encoding[0])
                {
#if !UNITY_WEBGL || UNITY_EDITOR
                    case "gzip":
                        decoderStream = new Decompression.Zlib.GZipStream(streamToDecode,
                            Decompression.Zlib.CompressionMode.Decompress);
                        break;
                    case "deflate":
                        decoderStream = new Decompression.Zlib.DeflateStream(streamToDecode,
                            Decompression.Zlib.CompressionMode.Decompress);
                        break;
#endif
                    //identity, utf-8, etc.
                    default:
                        // Do not copy from one stream to an other, just return with the raw bytes
                        return streamToDecode.ToArray();
                }
            }

#if !UNITY_WEBGL || UNITY_EDITOR
            using var ms = new BufferPoolMemoryStream((int)streamToDecode.Length);
            var buf = BufferPool.Get(1024, true);
            int byteCount;

            while ((byteCount = decoderStream.Read(buf, 0, buf.Length)) > 0)
                ms.Write(buf, 0, byteCount);

            BufferPool.Release(buf);

            decoderStream.Dispose();
            return ms.ToArray();
#endif
        }

        #endregion

        #region 流片段支持

        protected void BeginReceiveStreamFragments()
        {
#if !BESTHTTP_DISABLE_CACHING
            if (!BaseRequest.DisableCache && BaseRequest.UseStreaming)
            {
                // 如果缓存被启用，并且响应不是来自缓存，它是可缓存的，我们将缓存下载的数据。
                if (!IsFromCache && HttpCacheService.IsCacheable(BaseRequest.CurrentUri, BaseRequest.MethodType, this))
                {
                    _cacheStream = HttpCacheService.PrepareStreamed(BaseRequest.CurrentUri, this);
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
            if (buffer == null || length == 0)
                return;

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
                VerboseLogging("WaitWhileFragmentQueueIsFull");

#if CSHARP_7_3_OR_NEWER
                spinWait.SpinOnce();
#elif !NETFX_CORE
                System.Threading.Thread.Sleep(1);
#endif
            }
#endif

            if (_fragmentBuffer == null)
            {
                _fragmentBuffer = BufferPool.Get(BaseRequest.StreamFragmentSize, true);
                _fragmentBufferDataLength = 0;
            }

            if (_fragmentBufferDataLength + length <= _fragmentBuffer.Length)
            {
                Array.Copy(buffer, pos, _fragmentBuffer, _fragmentBufferDataLength, length);
                _fragmentBufferDataLength += length;

                if (_fragmentBufferDataLength == _fragmentBuffer.Length || BaseRequest.StreamChunksImmediately)
                {
                    AddStreamedFragment(_fragmentBuffer, _fragmentBufferDataLength);
                    _fragmentBuffer = null;
                    _fragmentBufferDataLength = 0;
                }
            }
            else
            {
                int remaining = _fragmentBuffer.Length - _fragmentBufferDataLength;

                FeedStreamFragment(buffer, pos, remaining);
                FeedStreamFragment(buffer, pos + remaining, length - remaining);
            }
        }

        protected void FlushRemainingFragmentBuffer()
        {
            if (_fragmentBuffer != null)
            {
                AddStreamedFragment(_fragmentBuffer, _fragmentBufferDataLength);
                _fragmentBuffer = null;
                _fragmentBufferDataLength = 0;
            }

#if !BESTHTTP_DISABLE_CACHING
            if (_cacheStream != null)
            {
                _cacheStream.Dispose();
                _cacheStream = null;

                HttpCacheService.SetBodyLength(BaseRequest.CurrentUri, _allFragmentSize);
            }
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
                if (this.BaseRequest.UseStreaming && buffer != null && bufferLength > 0)
                {
                    RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.BaseRequest, buffer,
                        bufferLength));
                    Interlocked.Increment(ref this.UnprocessedFragments);
                }
            }

            if (HttpManager.Logger.Level == Loglevels.All && buffer != null)
            {
                VerboseLogging(
                    $"AddStreamedFragment缓冲区长度: {bufferLength:N0} UnprocessedFragments: {Interlocked.Read(ref this.UnprocessedFragments):N0}");
            }

#if !BESTHTTP_DISABLE_CACHING
            if (_cacheStream != null)
            {
                if (buffer != null)
                {
                    _cacheStream.Write(buffer, 0, bufferLength);
                }

                _allFragmentSize += bufferLength;
            }
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

            if (result && HttpManager.Logger.Level == Loglevels.All)
            {
                VerboseLogging($"FragmentQueueIsFull - {unprocessedFragments} / {BaseRequest.MaxFragmentQueueLength}");
            }

            return result;
#else
            return false;
#endif
        }

        #endregion

        void VerboseLogging(string str)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[HTTPResponse]  [message: {str}]");
#endif
            // if (HttpManager.Logger.Level == Loglevels.All)
            //     HttpManager.Logger.Verbose("HTTPResponse", str, this.Context, this.BaseRequest.Context);
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
            if (disposing)
            {
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
}