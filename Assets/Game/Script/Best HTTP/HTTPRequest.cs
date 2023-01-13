using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BestHTTP.Authentication;
using BestHTTP.Connections;
using BestHTTP.Core;
using BestHTTP.Extensions;
using BestHTTP.Forms;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.Timings;

namespace BestHTTP
{
#if !BESTHTTP_DISABLE_COOKIES
    using Cookies;
#endif

    /// <summary>
    /// HttpRequest对象可能的逻辑状态。
    /// </summary>
    public enum HttpRequestStates
    {
        /// <summary>
        /// 请求的初始状态。在此状态下不会调用任何回调函数。
        /// </summary>
        Initial,

        /// <summary>
        /// 请求排队等待处理。
        /// </summary>
        Queued,

        /// <summary>
        ///开始处理请求。在这种状态下，客户端将发送请求并解析响应。在此状态下不会调用任何回调函数。
        /// </summary>
        Processing,

        /// <summary>
        ///请求完成。解析响应完成后，可以使用结果。用户定义的回调将使用有效的响应对象调用。请求的Exception属性将为空。
        /// </summary>
        Finished,

        /// <summary>
        /// 请求结束时出现意外错误。用户定义的回调将使用空响应对象调用。请求的Exception属性可能包含更多关于错误的信息，但它可以为空。
        /// </summary>
        Error,

        /// <summary>
        /// 请求被客户端中止(HTTPRequest的Abort()函数)。用户定义的回调将以空响应被调用。请求的Exception属性将为空。
        /// </summary>
        Aborted,

        /// <summary>
        /// 连接服务器超时。处理步骤用户定义的回调将以空响应被调用。请求的Exception属性将为空。
        /// </summary>
        ConnectionTimedOut,

        /// <summary>
        /// 请求没有在规定的时间内完成。用户定义的回调将以空响应被调用。请求的Exception属性将为空。
        /// </summary>
        TimedOut
    }

    public delegate void OnRequestFinishedDelegate(HttpRequest originalRequest, HttpResponse response);

    public delegate void OnDownloadProgressDelegate(HttpRequest originalRequest, long downloaded, long downloadLength);

    public delegate void OnUploadProgressDelegate(HttpRequest originalRequest, long uploaded, long uploadLength);

    public delegate bool OnBeforeRedirectionDelegate(HttpRequest originalRequest, HttpResponse response,
        Uri redirectUri);

    public delegate void OnHeaderEnumerationDelegate(string header, List<string> values);

    public delegate void OnBeforeHeaderSendDelegate(HttpRequest req);

    public delegate void OnHeadersReceivedDelegate(HttpRequest originalRequest, HttpResponse response,
        Dictionary<string, List<string>> headers);

    /// <summary>
    /// 调用从服务器下载的每个数据片段。它的返回值表示插件是否可以重用dataFragment数组。
    /// </summary>
    /// <param name="request">父HTTPRequest对象</param>
    /// <param name="response">HTTPResponse对象.</param>
    /// <param name="dataFragment">下载的数据。字节[]可以比实际负载更大!它可以使用的有效长度在dataFragmentLength参数中.</param>
    /// <param name="dataFragmentLength">下载数据的长度。</param>
    public delegate bool OnStreamingDataDelegate(HttpRequest request, HttpResponse response, byte[] dataFragment,
        int dataFragmentLength);

    public sealed class HttpRequest : IEnumerator, IEnumerator<HttpRequest>
    {
        #region 静态属性

        public static readonly byte[] Eol = { HttpResponse.CR, HttpResponse.LF };

        /// <summary>
        /// 缓存大写值以节省cpu周期和每个请求的GC分配。
        /// </summary>
        public static readonly string[] MethodNames =
        {
            HttpMethods.Get.ToString().ToUpper(),
            HttpMethods.Head.ToString().ToUpper(),
            HttpMethods.Post.ToString().ToUpper(),
            HttpMethods.Put.ToString().ToUpper(),
            HttpMethods.Delete.ToString().ToUpper(),
            HttpMethods.Patch.ToString().ToUpper(),
            HttpMethods.Merge.ToString().ToUpper(),
            HttpMethods.Options.ToString().ToUpper(),
            HttpMethods.Connect.ToString().ToUpper(),
        };

        /// <summary>
        /// 内部缓冲区大小，上传过程中会触发此大小的数据发送到线路。默认值为4kib。
        /// </summary>
        public static readonly int UploadChunkSize = 4 * 1024;

        #endregion

        #region 属性

        /// <summary>
        /// 原始请求的Uri。
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// 该方法表示我们希望如何处理服务器请求。
        /// </summary>
        public HttpMethods MethodType { get; set; }

        /// <summary>
        /// 在POST请求中发送的原始数据。如果设置了，添加到此请求的所有其他字段将被忽略。
        /// </summary>
        public byte[] RawData { get; set; }

        /// <summary>
        /// 插件用来获取数据并发送到服务器的流。当设置此属性时，将不使用任何表单或RawData属性
        /// </summary>
        public Stream UploadStream { get; set; }

        /// <summary>
        /// 当设置为true(它的默认值)时，插件将在上传完数据后调用UploadStream的Dispose()函数。默认值为true
        /// </summary>
        public bool DisposeUploadStream { get; set; }

        /// <summary>
        /// 如果为真，插件将使用流的长度属性。否则插件将发送数据分块。默认值为true。
        /// </summary>
        public bool UseUploadStreamLength { get; set; }

        /// <summary>
        /// 在数据发送到线路后调用。
        /// </summary>
        public OnUploadProgressDelegate OnUploadProgress;

        /// <summary>
        /// 指示在接收到响应后应打开连接。如果为真，那么内部TCP连接将被重用(如果可能的话)。默认值为true。
        /// 缺省值可以在HTTPManager类中更改。如果你很少向服务器发出请求，它应该被更改为false。
        /// </summary>
        public bool IsKeepAlive
        {
            get { return _isKeepAlive; }
            set
            {
                if (State == HttpRequestStates.Processing)
                    throw new NotSupportedException(
                        "Changing the IsKeepAlive property while processing the request is not supported.");
                _isKeepAlive = value;
            }
        }

#if !BESTHTTP_DISABLE_CACHING
        /// <summary>
        /// With this property caching can be enabled/disabled on a per-request basis.
        /// </summary>
        public bool DisableCache
        {
            get { return _disableCache; }
            set
            {
                if (State == HttpRequestStates.Processing)
                    throw new NotSupportedException(
                        "Changing the DisableCache property while processing the request is not supported.");
                _disableCache = value;
            }
        }

        /// <summary>
        /// It can be used with streaming. When set to true, no OnStreamingData event is called, the streamed content will be saved straight to the cache if all requirements are met(caching is enabled and there's a caching headers).
        /// </summary>
        public bool CacheOnly
        {
            get { return _cacheOnly; }
            set
            {
                if (State == HttpRequestStates.Processing)
                    throw new NotSupportedException(
                        "Changing the CacheOnly property while processing the request is not supported.");
                _cacheOnly = value;
            }
        }
#endif

        /// <summary>
        /// Maximum size of a data chunk that we want to receive when streaming is set. Its default value is 1 MB.
        /// </summary>
        public int StreamFragmentSize
        {
            get { return _streamFragmentSize; }
            set
            {
                if (State == HttpRequestStates.Processing)
                    throw new NotSupportedException(
                        "Changing the StreamFragmentSize property while processing the request is not supported.");

                if (value < 1)
                    throw new System.ArgumentException("StreamFragmentSize must be at least 1.");

                _streamFragmentSize = value;
            }
        }

        /// <summary>
        /// When set to true, StreamFragmentSize will be ignored and downloaded chunks will be sent immediately.
        /// </summary>
        public bool StreamChunksImmediately { get; set; }

        /// <summary>
        /// This property can be used to force the HTTPRequest to use an exact sized read buffer.
        /// </summary>
        public int ReadBufferSizeOverride { get; set; }

        /// <summary>
        /// Maximum unprocessed fragments allowed to queue up. 
        /// </summary>
        public int MaxFragmentQueueLength { get; set; }

        /// <summary>
        /// The callback function that will be called when a request is fully processed or when any downloaded fragment is available if UseStreaming is true. Can be null for fire-and-forget requests.
        /// </summary>
        public OnRequestFinishedDelegate Callback { get; set; }

        /// <summary>
        /// When the request is queued for processing.
        /// </summary>
        public DateTime QueuedAt { get; internal set; }

        public bool IsConnectTimedOut
        {
            get { return this.QueuedAt != DateTime.MinValue && DateTime.UtcNow - this.QueuedAt > this.ConnectTimeout; }
        }

        /// <summary>
        /// When the processing of the request started
        /// </summary>
        public DateTime ProcessingStarted { get; internal set; }

        /// <summary>
        /// Returns true if the time passed the Timeout setting since processing started.
        /// </summary>
        public bool IsTimedOut
        {
            get
            {
                DateTime now = DateTime.UtcNow;

                return (!this.UseStreaming || (this.UseStreaming && this.EnableTimoutForStreaming)) &&
                       ((this.ProcessingStarted != DateTime.MinValue && now - this.ProcessingStarted > this.Timeout) ||
                        this.IsConnectTimedOut);
            }
        }

        /// <summary>
        /// Called for every fragment of data downloaded from the server. Return true if dataFrament is processed and the plugin can recycle the byte[].
        /// </summary>
        public OnStreamingDataDelegate OnStreamingData;

        /// <summary>
        /// This event is called when the plugin received and parsed all headers.
        /// </summary>
        public OnHeadersReceivedDelegate OnHeadersReceived;

        /// <summary>
        /// Number of times that the plugin retried the request.
        /// </summary>
        public int Retries { get; internal set; }

        /// <summary>
        /// Maximum number of tries allowed. To disable it set to 0. Its default value is 1 for GET requests, otherwise 0.
        /// </summary>
        public int MaxRetries { get; set; }

        /// <summary>
        /// True if Abort() is called on this request.
        /// </summary>
        public bool IsCancellationRequested { get; internal set; }

        /// <summary>
        /// Called when new data downloaded from the server.
        /// The first parameter is the original HTTTPRequest object itself, the second parameter is the downloaded bytes while the third parameter is the content length.
        /// <remarks>There are download modes where we can't figure out the exact length of the final content. In these cases we just guarantee that the third parameter will be at least the size of the second one.</remarks>
        /// </summary>
        public OnDownloadProgressDelegate OnDownloadProgress;

        /// <summary>
        /// Indicates that the request is redirected. If a request is redirected, the connection that served it will be closed regardless of the value of IsKeepAlive.
        /// </summary>
        public bool IsRedirected { get; internal set; }

        /// <summary>
        /// The Uri that the request redirected to.
        /// </summary>
        public Uri RedirectUri { get; internal set; }

        /// <summary>
        /// If redirected it contains the RedirectUri.
        /// </summary>
        public Uri CurrentUri
        {
            get { return IsRedirected ? RedirectUri : Uri; }
        }

        /// <summary>
        /// The response to the query.
        /// <remarks>If an exception occurred during reading of the response stream or can't connect to the server, this will be null!</remarks>
        /// </summary>
        public HttpResponse Response { get; internal set; }

#if !BESTHTTP_DISABLE_PROXY
        /// <summary>
        /// Response from the Proxy server. It's null with transparent proxies.
        /// </summary>
        public HttpResponse ProxyResponse { get; internal set; }
#endif

        /// <summary>
        /// It there is an exception while processing the request or response the Response property will be null, and the Exception will be stored in this property.
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// Any object can be passed with the request with this property. (eq. it can be identified, etc.)
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// The UserName, Password pair that the plugin will use to authenticate to the remote server.
        /// </summary>
        public Credentials Credentials { get; set; }

#if !BESTHTTP_DISABLE_PROXY
        /// <summary>
        /// True, if there is a Proxy object.
        /// </summary>
        public bool HasProxy
        {
            get { return Proxy != null && Proxy.UseProxyForAddress(this.CurrentUri); }
        }

        /// <summary>
        /// A web proxy's properties where the request must pass through.
        /// </summary>
        public Proxy Proxy { get; set; }
#endif

        /// <summary>
        /// How many redirection supported for this request. The default is 10. 0 or a negative value means no redirection supported.
        /// </summary>
        public int MaxRedirects { get; set; }

#if !BESTHTTP_DISABLE_COOKIES

        /// <summary>
        /// If true cookies will be added to the headers (if any), and parsed from the response. If false, all cookie operations will be ignored. It's default value is HTTPManager's IsCookiesEnabled.
        /// </summary>
        public bool IsCookiesEnabled { get; set; }

        /// <summary>
        /// Cookies that are added to this list will be sent to the server alongside withe the server sent ones. If cookies are disabled only these cookies will be sent.
        /// </summary>
        public List<Cookie> Cookies
        {
            get
            {
                if (_customCookies == null)
                    _customCookies = new List<Cookie>();
                return _customCookies;
            }
            set { _customCookies = value; }
        }

        private List<Cookie> _customCookies;
#endif

        /// <summary>
        /// What form should used. Its default value is Automatic.
        /// </summary>
        public HttpFormUsage FormUsage { get; set; }

        /// <summary>
        /// Current state of this request.
        /// </summary>
        public HttpRequestStates State
        {
            get { return this._state; }
            internal set
            {
                lock (this)
                {
                    if (this._state != value)
                    {
                        //if (this._state >= HTTPRequestStates.Finished && value >= HTTPRequestStates.Finished)
                        //    return;

                        this._state = value;

                        RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this, this._state));
                    }
                }
            }
        }

        private volatile HttpRequestStates _state;

        /// <summary>
        /// How many times redirected.
        /// </summary>
        public int RedirectCount { get; internal set; }

        /// <summary>
        /// Maximum time we wait to establish the connection to the target server. If set to TimeSpan.Zero or lower, no connect timeout logic is executed. Default value is 20 seconds.
        /// </summary>
        public TimeSpan ConnectTimeout { get; set; }

        /// <summary>
        /// Maximum time we want to wait to the request to finish after the connection is established. Default value is 60 seconds.
        /// <remarks>It's disabled for streaming requests! See <see cref="EnableTimoutForStreaming"/>.</remarks>
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Set to true to enable Timeouts on streaming request. Default value is false.
        /// </summary>
        public bool EnableTimoutForStreaming { get; set; }

        /// <summary>
        /// Enables safe read method when the response's length of the content is unknown. Its default value is enabled (true).
        /// </summary>
        public bool EnableSafeReadOnUnknownContentLength { get; set; }

        /// <summary>
        /// It's called before the plugin will do a new request to the new uri. The return value of this function will control the redirection: if it's false the redirection is aborted.
        /// This function is called on a thread other than the main Unity thread!
        /// </summary>
        public event OnBeforeRedirectionDelegate OnBeforeRedirection
        {
            add { _onBeforeRedirection += value; }
            remove { _onBeforeRedirection -= value; }
        }

        private OnBeforeRedirectionDelegate _onBeforeRedirection;

        /// <summary>
        /// This event will be fired before the plugin will write headers to the wire. New headers can be added in this callback. This event is called on a non-Unity thread!
        /// </summary>
        public event OnBeforeHeaderSendDelegate OnBeforeHeaderSend
        {
            add { _onBeforeHeaderSend += value; }
            remove { _onBeforeHeaderSend -= value; }
        }

        private OnBeforeHeaderSendDelegate _onBeforeHeaderSend;

        /// <summary>
        /// Logging context of the request.
        /// </summary>
        public LoggingContext Context { get; private set; }

        /// <summary>
        /// Timing information.
        /// </summary>
        public TimingCollector Timing { get; private set; }

#if UNITY_WEBGL
        /// <summary>
        /// Its value will be set to the XmlHTTPRequest's withCredentials field. Its default value is HTTPManager.IsCookiesEnabled's value.
        /// </summary>
        public bool WithCredentials { get; set; }
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
        /// <summary>
        /// Called when the current protocol is upgraded to an other. (HTTP => WebSocket for example)
        /// </summary>
        internal OnRequestFinishedDelegate OnUpgraded;
#endif

        #region Internal Properties For Progress Report Support

        /// <summary>
        /// If it's true, the Callback will be called every time if we can send out at least one fragment.
        /// </summary>
        internal bool UseStreaming
        {
            get { return this.OnStreamingData != null; }
        }

        /// <summary>
        /// Will return the length of the UploadStream, or -1 if it's not supported.
        /// </summary>
        internal long UploadStreamLength
        {
            get
            {
                if (UploadStream == null || !UseUploadStreamLength)
                    return -1;

                try
                {
                    // This may will throw a NotSupportedException
                    return UploadStream.Length;
                }
                catch
                {
                    // We will fall back to chunked
                    return -1;
                }
            }
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        /// <summary>
        /// This action is called when a user calls the Abort function. Do not use it outside of the plugin!
        /// </summary>
        internal Action<HttpRequest> OnCancellationRequested;
#endif

        #endregion

        #endregion

        #region 私有属性

        private bool _isKeepAlive;
#if !BESTHTTP_DISABLE_CACHING
        private bool _disableCache;
        private bool _cacheOnly;
#endif
        private int _streamFragmentSize;

        private Dictionary<string, List<string>> Headers { get; set; }

        /// <summary>
        /// We will collect the fields and values to the FieldCollector through the AddField and AddBinaryData functions.
        /// </summary>
        private HttpFormBase _fieldCollector;

        /// <summary>
        /// When the request about to send the request we will create a specialized form implementation(url-encoded, multipart, or the legacy WWWForm based).
        /// And we will use this instance to create the data that we will send to the server.
        /// </summary>
        private HttpFormBase _formImpl;

        #endregion

        #region Constructors

        #region Default Get Constructors

        public HttpRequest(Uri uri)
            : this(uri, HttpMethods.Get, HttpManager.KeepAliveDefaultValue,
#if !BESTHTTP_DISABLE_CACHING
                HttpManager.IsCachingDisabled
#else
            true
#endif
                , null)
        {
        }

        public HttpRequest(Uri uri, OnRequestFinishedDelegate callback)
            : this(uri, HttpMethods.Get, HttpManager.KeepAliveDefaultValue,
#if !BESTHTTP_DISABLE_CACHING
                HttpManager.IsCachingDisabled
#else
            true
#endif
                , callback)
        {
        }

        public HttpRequest(Uri uri, bool isKeepAlive, OnRequestFinishedDelegate callback)
            : this(uri, HttpMethods.Get, isKeepAlive,
#if !BESTHTTP_DISABLE_CACHING
                HttpManager.IsCachingDisabled
#else
            true
#endif

                , callback)
        {
        }

        public HttpRequest(Uri uri, bool isKeepAlive, bool disableCache, OnRequestFinishedDelegate callback)
            : this(uri, HttpMethods.Get, isKeepAlive, disableCache, callback)
        {
        }

        #endregion

        public HttpRequest(Uri uri, HttpMethods methodType)
            : this(uri, methodType, HttpManager.KeepAliveDefaultValue,
#if !BESTHTTP_DISABLE_CACHING
                HttpManager.IsCachingDisabled || methodType != HttpMethods.Get
#else
            true
#endif
                , null)
        {
        }

        public HttpRequest(Uri uri, HttpMethods methodType, OnRequestFinishedDelegate callback)
            : this(uri, methodType, HttpManager.KeepAliveDefaultValue,
#if !BESTHTTP_DISABLE_CACHING
                HttpManager.IsCachingDisabled || methodType != HttpMethods.Get
#else
            true
#endif
                , callback)
        {
        }

        public HttpRequest(Uri uri, HttpMethods methodType, bool isKeepAlive, OnRequestFinishedDelegate callback)
            : this(uri, methodType, isKeepAlive,
#if !BESTHTTP_DISABLE_CACHING
                HttpManager.IsCachingDisabled || methodType != HttpMethods.Get
#else
            true
#endif
                , callback)
        {
        }

        public HttpRequest(Uri uri, HttpMethods methodType, bool isKeepAlive, bool disableCache,
            OnRequestFinishedDelegate callback)
        {
            this.Uri = uri;
            this.MethodType = methodType;
            this.IsKeepAlive = isKeepAlive;
#if !BESTHTTP_DISABLE_CACHING
            this.DisableCache = disableCache;
#endif
            this.Callback = callback;
            this.StreamFragmentSize = 1024 * 1024;
            this.MaxFragmentQueueLength = 10;

            this.MaxRetries = methodType == HttpMethods.Get ? 1 : 0;
            this.MaxRedirects = 10;
            this.RedirectCount = 0;
#if !BESTHTTP_DISABLE_COOKIES
            this.IsCookiesEnabled = HttpManager.IsCookiesEnabled;
#endif

            this.State = HttpRequestStates.Initial;

            this.ConnectTimeout = HttpManager.ConnectTimeout;
            this.Timeout = HttpManager.RequestTimeout;
            this.EnableTimoutForStreaming = false;

            this.EnableSafeReadOnUnknownContentLength = true;

#if !BESTHTTP_DISABLE_PROXY
            this.Proxy = HttpManager.Proxy;
#endif

            this.UseUploadStreamLength = true;
            this.DisposeUploadStream = true;

#if UNITY_WEBGL && !BESTHTTP_DISABLE_COOKIES
            this.WithCredentials = this.IsCookiesEnabled;
#endif

            this.Context = new LoggingContext(this);
            this.Timing = new TimingCollector(this);
        }

        #endregion

        #region Public Field Functions

        /// <summary>
        /// Add a field with a given string value.
        /// </summary>
        public void AddField(string fieldName, string value)
        {
            AddField(fieldName, value, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Add a field with a given string value.
        /// </summary>
        public void AddField(string fieldName, string value, System.Text.Encoding e)
        {
            if (_fieldCollector == null)
                _fieldCollector = new HttpFormBase();

            _fieldCollector.AddField(fieldName, value, e);
        }

        /// <summary>
        /// Add a field with binary content to the form.
        /// </summary>
        public void AddBinaryData(string fieldName, byte[] content)
        {
            AddBinaryData(fieldName, content, null, null);
        }

        /// <summary>
        /// Add a field with binary content to the form.
        /// </summary>
        public void AddBinaryData(string fieldName, byte[] content, string fileName)
        {
            AddBinaryData(fieldName, content, fileName, null);
        }

        /// <summary>
        /// Add a field with binary content to the form.
        /// </summary>
        public void AddBinaryData(string fieldName, byte[] content, string fileName, string mimeType)
        {
            if (_fieldCollector == null)
                _fieldCollector = new HttpFormBase();

            _fieldCollector.AddBinaryData(fieldName, content, fileName, mimeType);
        }

        /// <summary>
        /// Manually set a HTTP Form.
        /// </summary>
        public void SetForm(HttpFormBase form)
        {
            _formImpl = form;
        }

        /// <summary>
        /// Returns with the added form-fields or null if no one added.
        /// </summary>
        public List<HttpFieldData> GetFormFields()
        {
            if (this._fieldCollector == null || this._fieldCollector.IsEmpty)
                return null;

            return new List<HttpFieldData>(this._fieldCollector.Fields);
        }

        /// <summary>
        /// Clears all data from the form.
        /// </summary>
        public void ClearForm()
        {
            _formImpl = null;
            _fieldCollector = null;
        }

        /// <summary>
        /// Will create the form implementation based on the value of the FormUsage property.
        /// </summary>
        private HttpFormBase SelectFormImplementation()
        {
            // Our form already created with a previous
            if (_formImpl != null)
                return _formImpl;

            // No field added to this request yet
            if (_fieldCollector == null)
                return null;

            switch (FormUsage)
            {
                case HttpFormUsage.Automatic:
                    // A really simple decision making: if there are at least one field with binary data, or a 'long' string value then we will choose a Multipart form.
                    //  Otherwise Url Encoded form will be used.
                    if (_fieldCollector.HasBinary || _fieldCollector.HasLongValue)
                        goto case HttpFormUsage.Multipart;
                    else
                        goto case HttpFormUsage.UrlEncoded;

                case HttpFormUsage.UrlEncoded:
                    _formImpl = new HttpUrlEncodedForm();
                    break;
                case HttpFormUsage.Multipart:
                    _formImpl = new HttpMultiPartForm();
                    break;
            }

            // Copy the fields, and other properties to the new implementation
            _formImpl.CopyFrom(_fieldCollector);

            return _formImpl;
        }

        #endregion

        #region Header Management

        #region General Management

        /// <summary>
        /// Adds a header and value pair to the Headers. Use it to add custom headers to the request.
        /// </summary>
        /// <example>AddHeader("User-Agent', "FooBar 1.0")</example>
        public void AddHeader(string name, string value)
        {
            if (Headers == null)
                Headers = new Dictionary<string, List<string>>();

            List<string> values;
            if (!Headers.TryGetValue(name, out values))
                Headers.Add(name, values = new List<string>(1));

            values.Add(value);
        }

        /// <summary>
        /// Removes any previously added values, and sets the given one.
        /// </summary>
        public void SetHeader(string name, string value)
        {
            if (Headers == null)
                Headers = new Dictionary<string, List<string>>();

            List<string> values;
            if (!Headers.TryGetValue(name, out values))
                Headers.Add(name, values = new List<string>(1));

            values.Clear();
            values.Add(value);
        }

        /// <summary>
        /// Removes the specified header. Returns true, if the header found and succesfully removed.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveHeader(string name)
        {
            if (Headers == null)
                return false;

            return Headers.Remove(name);
        }

        /// <summary>
        /// Returns true if the given head name is already in the Headers.
        /// </summary>
        public bool HasHeader(string name)
        {
            return Headers != null && Headers.ContainsKey(name);
        }

        /// <summary>
        /// Returns the first header or null for the given header name.
        /// </summary>
        public string GetFirstHeaderValue(string name)
        {
            if (Headers == null)
                return null;

            List<string> headers = null;
            if (Headers.TryGetValue(name, out headers) && headers.Count > 0)
                return headers[0];

            return null;
        }

        /// <summary>
        /// Returns all header values for the given header or null.
        /// </summary>
        public List<string> GetHeaderValues(string name)
        {
            if (Headers == null)
                return null;

            List<string> headers = null;
            if (Headers.TryGetValue(name, out headers) && headers.Count > 0)
                return headers;

            return null;
        }

        /// <summary>
        /// Removes all headers.
        /// </summary>
        public void RemoveHeaders()
        {
            if (Headers == null)
                return;

            Headers.Clear();
        }

        #endregion

        #region Range Headers

        /// <summary>
        /// Sets the Range header to download the content from the given byte position. See http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.35
        /// </summary>
        /// <param name="firstBytePos">Start position of the download.</param>
        public void SetRangeHeader(long firstBytePos)
        {
            SetHeader("Range", string.Format("bytes={0}-", firstBytePos));
        }

        /// <summary>
        /// Sets the Range header to download the content from the given byte position to the given last position. See http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.35
        /// </summary>
        /// <param name="firstBytePos">Start position of the download.</param>
        /// <param name="lastBytePos">The end position of the download.</param>
        public void SetRangeHeader(long firstBytePos, long lastBytePos)
        {
            SetHeader("Range", string.Format("bytes={0}-{1}", firstBytePos, lastBytePos));
        }

        #endregion

        public void EnumerateHeaders(OnHeaderEnumerationDelegate callback)
        {
            EnumerateHeaders(callback, false);
        }

        public void EnumerateHeaders(OnHeaderEnumerationDelegate callback, bool callBeforeSendCallback)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (!HasHeader("Host"))
            {
                if (CurrentUri.Port == 80 || CurrentUri.Port == 443)
                    SetHeader("Host", CurrentUri.Host);
                else
                    SetHeader("Host", CurrentUri.Authority);
            }

            if (IsRedirected && !HasHeader("Referer"))
                AddHeader("Referer", Uri.ToString());

            if (!HasHeader("Accept-Encoding"))
#if BESTHTTP_DISABLE_GZIP
              AddHeader("Accept-Encoding", "identity");
#else
                AddHeader("Accept-Encoding", "gzip, identity");
#endif

#if !BESTHTTP_DISABLE_PROXY
            if (!HttpProtocolFactory.IsSecureProtocol(this.CurrentUri) && HasProxy && !HasHeader("Proxy-Connection"))
                AddHeader("Proxy-Connection", IsKeepAlive ? "Keep-Alive" : "Close");
#endif

            if (!HasHeader("Connection"))
                AddHeader("Connection", IsKeepAlive ? "Keep-Alive, TE" : "Close, TE");

            if (IsKeepAlive && !HasHeader("Keep-Alive"))
            {
                // Send the server a slightly larger value to make sure it's not going to close sooner than the client
                int seconds = (int)Math.Ceiling(HttpManager.MaxConnectionIdleTime.TotalSeconds + 1);

                AddHeader("Keep-Alive", "timeout=" + seconds);
            }

            if (!HasHeader("TE"))
                AddHeader("TE", "identity");

            if (!string.IsNullOrEmpty(HttpManager.UserAgent) && !HasHeader("User-Agent"))
                AddHeader("User-Agent", HttpManager.UserAgent);
#endif
            long contentLength = -1;

            if (UploadStream == null)
            {
                byte[] entityBody = GetEntityBody();
                contentLength = entityBody != null ? entityBody.Length : 0;

                if (RawData == null && (_formImpl != null || (_fieldCollector != null && !_fieldCollector.IsEmpty)))
                {
                    SelectFormImplementation();
                    if (_formImpl != null)
                        _formImpl.PrepareRequest(this);
                }
            }
            else
            {
                contentLength = UploadStreamLength;

                if (contentLength == -1)
                    SetHeader("Transfer-Encoding", "Chunked");

                if (!HasHeader("Content-Type"))
                    SetHeader("Content-Type", "application/octet-stream");
            }

            // Always set the Content-Length header if possible
            // http://tools.ietf.org/html/rfc2616#section-4.4 : For compatibility with HTTP/1.0 applications, HTTP/1.1 requests containing a message-body MUST include a valid Content-Length header field unless the server is known to be HTTP/1.1 compliant.
            // 2018.06.03: Changed the condition so that content-length header will be included for zero length too.
            // 2022.05.25: Don't send a Content-Length (: 0) header if there's an Upgrade header. Upgrade is set for websocket, and it might be not true that the client doesn't send any bytes.
            if (
#if !UNITY_WEBGL || UNITY_EDITOR
                contentLength >= 0
#else
                contentLength != -1
#endif
                && !HasHeader("Content-Length")
                && !HasHeader("Upgrade"))
                SetHeader("Content-Length", contentLength.ToString());

#if !UNITY_WEBGL || UNITY_EDITOR
#if !BESTHTTP_DISABLE_PROXY
            // Proxy Authentication
            if (!HttpProtocolFactory.IsSecureProtocol(this.CurrentUri) && HasProxy && Proxy.Credentials != null)
            {
                switch (Proxy.Credentials.Type)
                {
                    case AuthenticationTypes.Basic:
                        // With Basic authentication we don't want to wait for a challenge, we will send the hash with the first request
                        SetHeader("Proxy-Authorization",
                            string.Concat("Basic ",
                                Convert.ToBase64String(Encoding.UTF8.GetBytes(Proxy.Credentials.UserName + ":" +
                                                                              Proxy.Credentials.Password))));
                        break;

                    case AuthenticationTypes.Unknown:
                    case AuthenticationTypes.Digest:
                        var digest = DigestStore.Get(Proxy.Address);
                        if (digest != null)
                        {
                            string authentication = digest.GenerateResponseHeader(this, Proxy.Credentials);
                            if (!string.IsNullOrEmpty(authentication))
                                SetHeader("Proxy-Authorization", authentication);
                        }

                        break;
                }
            }
#endif

#endif

            // Server authentication
            if (Credentials != null)
            {
                switch (Credentials.Type)
                {
                    case AuthenticationTypes.Basic:
                        // With Basic authentication we don't want to wait for a challenge, we will send the hash with the first request
                        SetHeader("Authorization",
                            string.Concat("Basic ",
                                Convert.ToBase64String(
                                    Encoding.UTF8.GetBytes(Credentials.UserName + ":" + Credentials.Password))));
                        break;

                    case AuthenticationTypes.Unknown:
                    case AuthenticationTypes.Digest:
                        var digest = DigestStore.Get(this.CurrentUri);
                        if (digest != null)
                        {
                            string authentication = digest.GenerateResponseHeader(this, Credentials);
                            if (!string.IsNullOrEmpty(authentication))
                                SetHeader("Authorization", authentication);
                        }

                        break;
                }
            }

            // Cookies.
#if !BESTHTTP_DISABLE_COOKIES
            // User added cookies are sent even when IsCookiesEnabled is set to false
            List<Cookie> cookies = IsCookiesEnabled ? CookieJar.Get(CurrentUri) : null;

            // Merge server sent cookies with user-set cookies
            if (cookies == null || cookies.Count == 0)
                cookies = this._customCookies;
            else if (this._customCookies != null)
            {
                // Merge
                int idx = 0;
                while (idx < this._customCookies.Count)
                {
                    Cookie customCookie = _customCookies[idx];

                    int foundIdx = cookies.FindIndex(c => c.Name.Equals(customCookie.Name));
                    if (foundIdx >= 0)
                        cookies[foundIdx] = customCookie;
                    else
                        cookies.Add(customCookie);

                    idx++;
                }
            }

            // http://tools.ietf.org/html/rfc6265#section-5.4
            //  -When the user agent generates an HTTP request, the user agent MUST NOT attach more than one Cookie header field.
            if (cookies != null && cookies.Count > 0)
            {
                // Room for improvement:
                //   2. The user agent SHOULD sort the cookie-list in the following order:
                //      *  Cookies with longer paths are listed before cookies with shorter paths.
                //      *  Among cookies that have equal-length path fields, cookies with earlier creation-times are listed before cookies with later creation-times.

                bool first = true;
                string cookieStr = string.Empty;

                bool isSecureProtocolInUse = HttpProtocolFactory.IsSecureProtocol(CurrentUri);

                foreach (var cookie in cookies)
                    if (!cookie.IsSecure || (cookie.IsSecure && isSecureProtocolInUse))
                    {
                        if (!first)
                            cookieStr += "; ";
                        else
                            first = false;

                        cookieStr += cookie.ToString();

                        // 3. Update the last-access-time of each cookie in the cookie-list to the current date and time.
                        cookie.LastAccess = DateTime.UtcNow;
                    }

                if (!string.IsNullOrEmpty(cookieStr))
                    SetHeader("Cookie", cookieStr);
            }
#endif

            if (callBeforeSendCallback && _onBeforeHeaderSend != null)
            {
                try
                {
                    _onBeforeHeaderSend(this);
                }
                catch (Exception ex)
                {
                    HttpManager.Logger.Exception("HTTPRequest", "OnBeforeHeaderSend", ex, this.Context);
                }
            }

            // Write out the headers to the stream
            if (callback != null && Headers != null)
                foreach (var kvp in Headers)
                    callback(kvp.Key, kvp.Value);
        }

        /// <summary>
        /// Writes out the Headers to the stream.
        /// </summary>
        private void SendHeaders(Stream stream)
        {
            EnumerateHeaders((header, values) =>
            {
                if (string.IsNullOrEmpty(header) || values == null)
                    return;

                byte[] headerName = string.Concat(header, ": ").GetASCIIBytes();

                for (int i = 0; i < values.Count; ++i)
                {
                    if (string.IsNullOrEmpty(values[i]))
                    {
                        HttpManager.Logger.Warning("HTTPRequest",
                            string.Format("Null/empty value for header: {0}", header), this.Context);
                        continue;
                    }

                    if (HttpManager.Logger.Level <= Logger.Loglevels.Information)
                        VerboseLogging("Header - '" + header + "': '" + values[i] + "'");

                    byte[] valueBytes = values[i].GetASCIIBytes();

                    stream.WriteArray(headerName);
                    stream.WriteArray(valueBytes);
                    stream.WriteArray(Eol);

                    BufferPool.Release(valueBytes);
                }

                BufferPool.Release(headerName);
            }, /*callBeforeSendCallback:*/ true);
        }

        /// <summary>
        /// Returns a string representation of the headers.
        /// </summary>
        public string DumpHeaders()
        {
            using (var ms = new BufferPoolMemoryStream(5 * 1024))
            {
                SendHeaders(ms);
                return ms.ToArray().AsciiToString();
            }
        }

        /// <summary>
        /// Returns with the bytes that will be sent to the server as the request's payload.
        /// </summary>
        /// <remarks>Call this only after all form-fields are added!</remarks>
        public byte[] GetEntityBody()
        {
            if (RawData != null)
                return RawData;

            if (_formImpl != null || (_fieldCollector != null && !_fieldCollector.IsEmpty))
            {
                SelectFormImplementation();
                if (_formImpl != null)
                    return _formImpl.GetData();
            }

            return null;
        }

        internal struct UploadStreamInfo
        {
            public readonly Stream Stream;
            public readonly long Length;

            public UploadStreamInfo(Stream stream, long length)
            {
                this.Stream = stream;
                this.Length = length;
            }
        }

        internal UploadStreamInfo GetUpStream()
        {
            byte[] data = RawData;

            // We are sending forms? Then convert the form to a byte array
            if (data == null && _formImpl != null)
                data = _formImpl.GetData();

            if (data != null || UploadStream != null)
            {
                // Make a new reference, as we will check the UploadStream property in the HTTPManager
                Stream uploadStream = UploadStream;

                long uploadLength = 0;

                if (uploadStream == null)
                {
                    // Make stream from the data. A BufferPoolMemoryStream could be used here,
                    // but because data comes from outside, we don't have control on its lifetime
                    // and might be gets reused without our knowledge.
                    uploadStream = new MemoryStream(data, 0, data.Length);

                    // Initialize progress report variable
                    uploadLength = data.Length;
                }
                else
                    uploadLength = UseUploadStreamLength ? UploadStreamLength : -1;

                return new UploadStreamInfo(uploadStream, uploadLength);
            }

            return new UploadStreamInfo(null, 0);
        }

        #endregion

        #region Internal Helper Functions

        internal void SendOutTo(Stream stream)
        {
            // Under WEBGL EnumerateHeaders and GetEntityBody are used instead of this function.
#if !UNITY_WEBGL || UNITY_EDITOR
            string requestPathAndQuery =
#if !BESTHTTP_DISABLE_PROXY
                HasProxy
                    ? this.Proxy.GetRequestPath(CurrentUri)
                    :
#endif
                    CurrentUri.GetRequestPathAndQueryURL();

            string requestLine = string.Format("{0} {1} HTTP/1.1", MethodNames[(byte)MethodType], requestPathAndQuery);

            if (HttpManager.Logger.Level <= Logger.Loglevels.Information)
                HttpManager.Logger.Information("HTTPRequest", string.Format("Sending request: '{0}'", requestLine),
                    this.Context);

            // Create a buffer stream that will not close 'stream' when disposed or closed.
            // buffersize should be larger than UploadChunkSize as it might be used for uploading user data and
            //  it should have enough room for UploadChunkSize data and additional chunk information.
            using (WriteOnlyBufferedStream bufferStream =
                   new WriteOnlyBufferedStream(stream, (int)(UploadChunkSize * 1.5f)))
            {
                byte[] requestLineBytes = requestLine.GetASCIIBytes();
                bufferStream.WriteArray(requestLineBytes);
                bufferStream.WriteArray(Eol);

                BufferPool.Release(requestLineBytes);

                // Write headers to the buffer
                SendHeaders(bufferStream);
                bufferStream.WriteArray(Eol);

                // Send remaining data to the wire
                bufferStream.Flush();

                byte[] data = RawData;

                // We are sending forms? Then convert the form to a byte array
                if (data == null && _formImpl != null)
                    data = _formImpl.GetData();

                if (data != null || UploadStream != null)
                {
                    // Make a new reference, as we will check the UploadStream property in the HTTPManager
                    Stream uploadStream = UploadStream;

                    long uploadLength = 0;

                    if (uploadStream == null)
                    {
                        // Make stream from the data. A BufferPoolMemoryStream could be used here,
                        // but because data comes from outside, we don't have control on it's lifetime
                        // and might be gets reused without our knowledge.
                        uploadStream = new MemoryStream(data, 0, data.Length);

                        // Initialize progress report variable
                        uploadLength = data.Length;
                    }
                    else
                        uploadLength = UseUploadStreamLength ? UploadStreamLength : -1;

                    // Initialize the progress report variables
                    long uploaded = 0;

                    // Upload buffer. First we will read the data into this buffer from the UploadStream, then write this buffer to our outStream
                    byte[] buffer = BufferPool.Get(UploadChunkSize, true);

                    // How many bytes was read from the UploadStream
                    int count = 0;
                    while ((count = uploadStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        // If we don't know the size, send as chunked
                        if (!UseUploadStreamLength)
                        {
                            byte[] countBytes = count.ToString("X").GetASCIIBytes();
                            bufferStream.WriteArray(countBytes);
                            bufferStream.WriteArray(Eol);

                            BufferPool.Release(countBytes);
                        }

                        // write out the buffer to the wire
                        bufferStream.Write(buffer, 0, count);

                        // chunk trailing EOL
                        if (!UseUploadStreamLength)
                            bufferStream.WriteArray(Eol);

                        // update how many bytes are uploaded
                        uploaded += count;

                        // Write to the wire
                        bufferStream.Flush();

                        if (this.OnUploadProgress != null)
                            RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this,
                                RequestEvents.UploadProgress, uploaded, uploadLength));

                        if (this.IsCancellationRequested)
                            return;
                    }

                    BufferPool.Release(buffer);

                    // All data from the stream are sent, write the 'end' chunk if necessary
                    if (!UseUploadStreamLength)
                    {
                        byte[] noMoreChunkBytes = BufferPool.Get(1, true);
                        noMoreChunkBytes[0] = (byte)'0';
                        bufferStream.Write(noMoreChunkBytes, 0, 1);
                        bufferStream.WriteArray(Eol);
                        bufferStream.WriteArray(Eol);

                        BufferPool.Release(noMoreChunkBytes);
                    }

                    // Make sure all remaining data will be on the wire
                    bufferStream.Flush();

                    // Dispose the MemoryStream
                    if (UploadStream == null && uploadStream != null)
                        uploadStream.Dispose();
                }
                else
                    bufferStream.Flush();
            } // bufferStream.Dispose

            HttpManager.Logger.Information("HTTPRequest", "'" + requestLine + "' sent out", this.Context);
#endif
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        internal void UpgradeCallback()
        {
            if (Response == null || !Response.IsUpgraded)
                return;

            try
            {
                if (OnUpgraded != null)
                    OnUpgraded(this, Response);
            }
            catch (Exception ex)
            {
                HttpManager.Logger.Exception("HTTPRequest", "UpgradeCallback", ex, this.Context);
            }
        }
#endif

        internal bool CallOnBeforeRedirection(Uri redirectUri)
        {
            if (_onBeforeRedirection != null)
                return _onBeforeRedirection(this, this.Response, redirectUri);

            return true;
        }

        /// <summary>
        /// Called on Unity's main thread just before processing it.
        /// </summary>
        internal void Prepare()
        {
        }

        #endregion

        /// <summary>
        /// Starts processing the request.
        /// </summary>
        public HttpRequest Send()
        {
            this.IsCancellationRequested = false;
            this.Exception = null;

            return HttpManager.SendRequest(this);
        }

        /// <summary>
        /// Aborts an already established connection, so no further download or upload are done.
        /// </summary>
        public void Abort()
        {
            VerboseLogging("Abort request!");

            lock (this)
            {
                if (this.State >= HttpRequestStates.Finished)
                    return;

                this.IsCancellationRequested = true;

                // If the response is an IProtocol implementation, call the protocol's cancellation.
                IProtocol protocol = this.Response as IProtocol;
                if (protocol != null)
                    protocol.CancellationRequested();

                // There's a race-condition here, another thread might set it too.
                this.Response = null;

                // There's a race-condition here too, another thread might set it too.
                //  In this case, both state going to be queued up that we have to handle in RequestEvents.cs.
                if (this.IsTimedOut)
                {
                    this.State = this.IsConnectTimedOut
                        ? HttpRequestStates.ConnectionTimedOut
                        : HttpRequestStates.TimedOut;
                }
                else
                    this.State = HttpRequestStates.Aborted;

#if !UNITY_WEBGL || UNITY_EDITOR
                if (this.OnCancellationRequested != null)
                {
                    try
                    {
                        this.OnCancellationRequested(this);
                    }
                    catch
                    {
                    }
                }
#endif
            }
        }

        /// <summary>
        /// Resets the request for a state where switching MethodType is possible.
        /// </summary>
        public void Clear()
        {
            ClearForm();
            RemoveHeaders();

            this.IsRedirected = false;
            this.RedirectCount = 0;
            this.Exception = null;
        }

        private void VerboseLogging(string str)
        {
            HttpManager.Logger.Verbose("HTTPRequest", str, this.Context);
        }

        #region System.Collections.IEnumerator implementation

        public object Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            return this.State < HttpRequestStates.Finished;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion

        HttpRequest IEnumerator<HttpRequest>.Current
        {
            get { return this; }
        }

        public void Dispose()
        {
            if (UploadStream != null && DisposeUploadStream)
            {
                UploadStream.Dispose();
                UploadStream = null;
            }

            if (Response != null)
                Response.Dispose();
        }
    }
}