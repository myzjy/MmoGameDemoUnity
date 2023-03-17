using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BestHTTP.Connections;
using BestHTTP.Core;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.Memory;
#if !BESTHTTP_DISABLE_CACHING
using BestHTTP.Caching;
#endif

#if !BESTHTTP_DISABLE_COOKIES
using BestHTTP.Cookies;
#endif

// ReSharper disable once CheckNamespace
namespace BestHTTP
{
    public enum ShutdownTypes
    {
        Running,
        Gentle,
        Immediate
    }

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
    public delegate Connections.TLS.AbstractTls13Client TlsClientFactoryDelegate(HttpRequest request,
        List<SecureProtocol.Org.BouncyCastle.Tls.ProtocolName> protocols);
#endif

    public delegate X509Certificate ClientCertificateSelector(
        HttpRequest request, string targetHost,
        X509CertificateCollection localCertificates,
        X509Certificate remoteCertificate, string[] acceptableIssuers);

    /// <summary>
    ///
    /// </summary>
    [PlatformSupport.IL2CPP.Il2CppEagerStaticClassConstructionAttribute]
    public static class HttpManager
    {
#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2
        /// <summary>
        /// HTTP/2 settings
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        public
            static readonly
            Connections.HTTP2.Http2PluginSettings Http2Settings =
                new Connections.HTTP2.Http2PluginSettings();
#endif

        private
            static
            bool _isSetupCalled;


        // 静态构造函数。设置默认值
        static HttpManager()
        {
            MaxConnectionPerServer = 6;
            KeepAliveDefaultValue = true;
            MaxPathLength = 255;
            MaxConnectionIdleTime = TimeSpan.FromSeconds(20);

#if !BESTHTTP_DISABLE_COOKIES
#if UNITY_WEBGL && !UNITY_EDITOR
            // 在webgl下，当IsCookiesEnabled为true时，它将为XmlHTTPRequest设置withCredentials标志
            //  这与默认行为不同。
            // https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest/withCredentials
            IsCookiesEnabled = false;
#else
            IsCookiesEnabled = true;
#endif
#endif

            CookieJarSize = 10 * 1024 * 1024;
            EnablePrivateBrowsing = false;
            ConnectTimeout = TimeSpan.FromSeconds(20);
            RequestTimeout = TimeSpan.FromSeconds(60);

            // Set the default logger mechanism
            _logger = new ThreadedLogger();

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
            UseAlternateSSLDefaultValue = true;
#endif

#if NETFX_CORE
            IOService = new PlatformSupport.FileSystem.NETFXCOREIOService();
#else
            IOService = new PlatformSupport.FileSystem.DefaultIOService();
#endif
        }

        /// <summary>
        /// 客户端将维持到服务器的最大活动TCP连接。缺省值为6。最小值为1。
        /// </summary>
        public static byte MaxConnectionPerServer
        {
            get => _maxConnectionPerServer;
            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException($"MaxConnectionPerServer 必须大于0!");
                }

                var isGrowing = value > _maxConnectionPerServer;
                _maxConnectionPerServer = value;

                // 如果每台服务器允许的连接正在增长，则遍历所有主机并尝试发送排队的请求。
                if (isGrowing)
                {
                    HostManager.TryToSendQueuedRequests();
                }
            }
        }

        private
            static
            byte _maxConnectionPerServer;

        /// <summary>
        /// HTTP请求的IsKeepAlive的缺省值。默认值为true。如果你很少向服务器请求，它应该被更改为false。
        /// </summary>
        public
            static
            bool KeepAliveDefaultValue { get; set; }

#if !BESTHTTP_DISABLE_CACHING
        /// <summary>
        /// 如果禁止缓存，则设置为true。
        /// </summary>
        public
            static
            bool IsCachingDisabled => false;
#endif

        /// <summary>
        /// 在连接完成最后一个请求后，必须传递多少时间来销毁该连接。缺省值为20秒。
        /// </summary>
        public
            static
            TimeSpan MaxConnectionIdleTime { get; set; }

#if !BESTHTTP_DISABLE_COOKIES
        /// <summary>
        /// 设置为false禁用所有Cookie。它的默认值是true。
        /// </summary>
        public
            static
            bool IsCookiesEnabled { get; set; }
#endif

        /// <summary>
        /// Cookie Jar的大小(以字节为单位)。默认值是10485760 (10 MB)。
        /// </summary>
        public
            static
            uint CookieJarSize { get; set; }

        /// <summary>
        /// 如果此属性设置为true，则新的cookie将被视为会话cookie，并且这些cookie不会保存到磁盘。默认值为false;
        /// </summary>
        public static bool EnablePrivateBrowsing { get; set; }

        /// <summary>
        /// 全局的，HTTPRequest的ConnectTimeout属性的默认值。如果设置为TimeSpan。0或更低，则不执行连接超时逻辑。缺省值是20秒。
        /// </summary>
        public static TimeSpan ConnectTimeout { get; set; }

        /// <summary>
        /// 全局的，HTTPRequest超时属性的默认值。缺省值为60秒。
        /// </summary>
        public static TimeSpan RequestTimeout { get; set; }

        /// <summary>
        /// <p>默认情况下，插件会将所有缓存和cookie数据保存在Application.persistentDataPath返回的路径下。</p>
        /// <p>你可以给这个委托分配一个函数来返回一个自定义的根路径来定义一个新的路径。</p>
        /// <remarks>此委托将在非Unity线程上调用!</remarks>
        /// </summary>
        private static Func<string> RootCacheFolderProvider => null;

#if !BESTHTTP_DISABLE_PROXY
        /// <summary>
        /// 所有httpRequest的全局默认代理。HTTPRequest的代理仍然可以在每个请求中更改。默认值为空。
        /// </summary>
        public static Proxy Proxy => null;
#endif

        /// <summary>
        /// Heartbeat管理器在插件中使用更少的线程。从OnUpdate函数调用心跳更新。
        /// </summary>
        public static HeartbeatManager Heartbeats
        {
            get { return _heartbeats ??= new HeartbeatManager(); }
        }

        private static HeartbeatManager _heartbeats;

        /// <summary>
        ///一个基本的BestHTTP.Logger.ILogger实现，能够智能地记录插件内部机制的附加信息。
        /// </summary>
        public static ILogger Logger
        {
            get
            {
                // 确保它有一个有效的记录器实例。
                if (_logger == null)
                {
                    _logger = new ThreadedLogger();
                    _logger.Level = Loglevels.None;
                }

                return _logger;
            }

            set => _logger = value;
        }

        private static ILogger _logger;

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

#pragma warning disable CS0169
        public
            static
            TlsClientFactoryDelegate TlsClientFactory { get; set; }
#pragma warning restore CS0169

        public static Connections.TLS.AbstractTls13Client DefaultTlsClientFactory(HttpRequest request,
            List<SecureProtocol.Org.BouncyCastle.Tls.ProtocolName> protocols)
        {
            // http://tools.ietf.org/html/rfc3546#section-3.1
            // -当客户端通过支持的名称类型定位服务器时，建议客户端在客户端hello中包含类型为"server_name"的扩展名。
            // -“HostName”中不允许输入IPv4和IPv6地址。
            // 用户自定义列表优先级更高
            List<SecureProtocol.Org.BouncyCastle.Tls.ServerName> hostNames = null;

            // 如果没有用户自定义IP地址，且主机不是IP地址，则添加默认IP地址
            if (request.CurrentUri.IsHostIsAnIPAddress())
            {
                return new Connections.TLS.DefaultTls13Client(request, null, protocols);
            }

            var serverName = new SecureProtocol.Org.BouncyCastle.Tls.ServerName(
                nameType: 0,
                nameData: Encoding.UTF8.GetBytes(request.CurrentUri.Host));
            hostNames = new List<SecureProtocol.Org.BouncyCastle.Tls.ServerName>(1)
            {
                serverName
            };

            return new Connections.TLS.DefaultTls13Client(request, hostNames, protocols);
        }

        /// <summary>
        /// HTTPRequest的UseAlternateSSL属性的默认值。
        /// </summary>
        public
            static
            bool UseAlternateSSLDefaultValue { get; set; }
#endif

#if !NETFX_CORE
#pragma warning disable CS0169

        public
            static
            Func<HttpRequest,
                X509Certificate,
                X509Chain,
                SslPolicyErrors,
                bool>
            DefaultCertificationValidator => null;
#pragma warning restore CS0169
#pragma warning disable CS0169
        public
            static
            ClientCertificateSelector
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            ClientCertificationProvider { get; set; }
#pragma warning restore CS0169
#endif

        /// <summary>
        /// TCP客户端的发送缓冲区大小。
        /// </summary>
#pragma warning disable CS0169
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnassignedField.Global
        public static int? SendBufferSize;
#pragma warning restore CS0169

        /// <summary>
        /// TCP客户端接收缓冲区大小。
        /// </summary>
#pragma warning disable CS0169
        // ReSharper disable once UnassignedField.Global
        public static int? ReceiveBufferSize;
#pragma warning restore CS0169

        /// <summary>
        /// 处理文件系统操作的IIOService实现。
        /// </summary>
        public static readonly PlatformSupport.FileSystem.IIOService IOService;

        /// <summary>
        /// 在大多数系统上，路径的最大长度大约是255个字符。如果缓存实体的路径比这个值长，它就不会被缓存。没有平台独立的API来查询当前系统上的确切值，但它是
        /// 暴露在这里，可以覆盖。缺省值为255。
        /// </summary>
        internal static int MaxPathLength { get; set; }

        /// <summary>
        /// 将随每个请求一起发送的用户代理字符串。
        /// </summary>
        public const string UserAgent = "BestHTTP/2 v2.6.3";

        /// <summary>
        /// 如果应用程序正在退出，插件正在关闭自己，这是正确的。
        /// </summary>
        public static bool IsQuitting
        {
            get => _isQuitting;
            private set => _isQuitting = value;
        }

        private static volatile bool _isQuitting;


        public static void Setup()
        {
            if (_isSetupCalled)
            {
                return;
            }

            _isSetupCalled = true;
            IsQuitting = false;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                StringBuilder sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}]");
                sb.Append($"{sf.GetMethod().Name}");
                sb.Append($"Line:{sf.GetFileLineNumber()}");
                sb.Append($"[msg]Setup called! UserAgent: {UserAgent}");
                Debug.Log($"{sb}");
            }
#endif
            HttpUpdateDelegator.CheckInstance();

#if !BESTHTTP_DISABLE_CACHING
            HttpCacheService.CheckSetup();
#endif

#if !BESTHTTP_DISABLE_COOKIES
            CookieJar.SetupFolder();
            CookieJar.Load();
#endif

            HostManager.Load();
        }

        public
            static
            HttpRequest SendRequest(string url, OnRequestFinishedDelegate callback)
        {
            var urlData = new Uri(url);
            var httpRequest = new HttpRequest(
                uri: urlData,
                methodType: HttpMethods.Get,
                callback: callback);
            return SendRequest(httpRequest);
        }

        public static HttpRequest SendRequest(string url, HttpMethods methodType, OnRequestFinishedDelegate callback)
        {
            var urlData = new Uri(url);
            var httpRequest = new HttpRequest(
                uri: urlData,
                methodType: methodType,
                callback: callback);
            return SendRequest(httpRequest);
        }

        public
            static
            HttpRequest SendRequest(string url, HttpMethods methodType, bool isKeepAlive,
                OnRequestFinishedDelegate callback)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                Debug.Log(
                    $"[{sf.GetFileName()}] [method:{sf.GetMethod().Name}] {sf.GetMethod().Name} Line:{sf.GetFileLineNumber()}");
            }
#endif
            var urlData = new Uri(url);
            var httpRequest = new HttpRequest(
                uri: urlData,
                methodType: methodType,
                isKeepAlive: isKeepAlive,
                callback: callback);
            return SendRequest(httpRequest);
        }

        public
            static
            HttpRequest SendRequest(
                string url,
                HttpMethods methodType,
                bool isKeepAlive,
                bool disableCache,
                OnRequestFinishedDelegate callback)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                Debug.Log(
                    $"[{sf.GetFileName()}] [method:{sf.GetMethod().Name}] {sf.GetMethod().Name} Line:{sf.GetFileLineNumber()}");
            }
#endif
            var urlData = new Uri(url);
            var httpRequest = new HttpRequest(
                uri: urlData,
                methodType: methodType,
                isKeepAlive: isKeepAlive,
                disableCache: disableCache,
                callback: callback);
            return SendRequest(httpRequest);
        }

        public static HttpRequest SendRequest(HttpRequest request)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                Debug.Log(
                    $"[{sf.GetFileName()}] [method:{sf.GetMethod().Name}] {sf.GetMethod().Name} Line:{sf.GetFileLineNumber()}");
            }
#endif
            if (!_isSetupCalled)
            {
                Setup();
            }

            if (request.IsCancellationRequested || IsQuitting)
            {
                return request;
            }

#if !BESTHTTP_DISABLE_CACHING
            // 如果可能的话，从缓存中加载完整的响应。
            if (HttpCacheService.IsCachedEntityExpiresInTheFuture(request))
            {
                var started = DateTime.Now;
                PlatformSupport.Threading.ThreadedRunner.RunShortLiving((req) =>
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    {
                        var st = new StackTrace(new StackFrame(true));
                        var sf = st.GetFrame(0);
                        Debug.Log(
                            $"[{sf.GetFileName()}] [method:{sf.GetMethod().Name}] {sf.GetMethod().Name} Line:{sf.GetFileLineNumber()}");
                    }
#endif
                    if (ConnectionHelper.TryLoadAllFromCache("HTTPManager", req, req.Context))
                    {
                        req.Timing.Add("Full Cache Load", DateTime.Now - started);
                        req.State = HttpRequestStates.Finished;
                    }
                    else
                    {
                        // 如果由于某种原因它不能加载，我们将请求放回队列。

                        request.State = HttpRequestStates.Queued;
                        var requestEvent = new RequestEventInfo(
                            request: request,
                            @event: RequestEvents.Resend);
                        RequestEventHelper.EnqueueRequestEvent(@event: requestEvent);
                    }
                }, request);
            }
            else
#endif
            {
                request.State = HttpRequestStates.Queued;
                var requestEvent = new RequestEventInfo(
                    request: request,
                    @event: RequestEvents.Resend);
                RequestEventHelper.EnqueueRequestEvent(@event: requestEvent);
            }

            return request;
        }

        /// <summary>
        /// 将返回应该保存各种缓存的位置。
        /// </summary>
        public static string GetRootCacheFolder()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($"将返回应该保存各种缓存的位置。");
                Debug.Log($"{sb}");
            }
#endif
            try
            {
                if (RootCacheFolderProvider != null)
                    return RootCacheFolderProvider();
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}] ");
                    sb.Append($"{sf.GetMethod().Name} ");
                    sb.Append($"Line:{sf.GetFileLineNumber()} ");
                    sb.Append($"[msg{sf.GetMethod().Name}]");
                    sb.Append($"[Exception] {ex}");
                    Debug.LogError($"{sb}");
                }
#endif
            }

#if NETFX_CORE
            return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
            return UnityEngine.Application.persistentDataPath;
#endif
        }

#if UNITY_EDITOR
#if UNITY_2019_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        public static void ResetSetup()
        {
            _isSetupCalled = false;
            BufferedReadNetworkStream.ResetNetworkStats();
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[HTTPManager] [method:ResetSetup] [msg] Reset called!");
#endif
        }
#endif

        /// <summary>
        /// 应该从Unity事件中定期调用的更新函数(Update, LateUpdate)。从这个函数分派回调函数。
        /// </summary>
        public static void OnUpdate()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($"OnUpdate");
                Debug.Log($"{sb}");
            }
#endif
            RequestEventHelper.ProcessQueue();
            ConnectionEventHelper.ProcessQueue();
            ProtocolEventHelper.ProcessQueue();
            PluginEventHelper.ProcessQueue();

            Timer.Process();

            _heartbeats?.Update();

            BufferPool.Maintain();
        }

        public static void OnQuit()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($" OnQuit called!");
                Debug.Log($"{sb}");
            }
#endif
            IsQuitting = true;

            AbortAll();

#if !BESTHTTP_DISABLE_CACHING
            HttpCacheService.SaveLibrary();
#endif

#if !BESTHTTP_DISABLE_COOKIES
            CookieJar.Persist();
#endif

            OnUpdate();

            HostManager.Clear();

            Heartbeats.Clear();
        }

        private static void AbortAll()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}]");
                sb.Append($"AbortAll called!");
                Debug.Log($"{sb}");
            }
#endif
            // 这是一个立即关闭的请求!

            RequestEventHelper.Clear();
            ConnectionEventHelper.Clear();
            PluginEventHelper.Clear();
            ProtocolEventHelper.Clear();

            HostManager.Shutdown();

            ProtocolEventHelper.CancelActiveProtocols();
        }
    }
}