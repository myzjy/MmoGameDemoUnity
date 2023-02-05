using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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

    public delegate System.Security.Cryptography.X509Certificates.X509Certificate ClientCertificateSelector(
        HttpRequest request, string targetHost,
        System.Security.Cryptography.X509Certificates.X509CertificateCollection localCertificates,
        System.Security.Cryptography.X509Certificates.X509Certificate remoteCertificate, string[] acceptableIssuers);

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
        public static readonly Connections.HTTP2.Http2PluginSettings Http2Settings =
            new Connections.HTTP2.Http2PluginSettings();
#endif

        #region Manager variables

        private static bool _isSetupCalled;

        #endregion

        // Static constructor. Setup default values
        static HttpManager()
        {
            MaxConnectionPerServer = 6;
            KeepAliveDefaultValue = true;
            MaxPathLength = 255;
            MaxConnectionIdleTime = TimeSpan.FromSeconds(20);

#if !BESTHTTP_DISABLE_COOKIES
#if UNITY_WEBGL && !UNITY_EDITOR
            // Under webgl when IsCookiesEnabled is true, it will set the withCredentials flag for the XmlHTTPRequest
            //  and that's different from the default behavior.
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
//#elif UNITY_WEBGL && !UNITY_EDITOR
//            IOService = new PlatformSupport.FileSystem.WebGLIOService();
#else
            IOService = new PlatformSupport.FileSystem.DefaultIOService();
#endif
        }

        #region Global Options

        /// <summary>
        /// The maximum active TCP connections that the client will maintain to a server. Default value is 6. Minimum value is 1.
        /// </summary>
        public static byte MaxConnectionPerServer
        {
            get => _maxConnectionPerServer;
            private set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException($"MaxConnectionPerServer must be greater than 0!");

                var isGrowing = value > _maxConnectionPerServer;
                _maxConnectionPerServer = value;

                // If the allowed connections per server is growing, go through all hosts and try to send out queueud requests.
                if (isGrowing)
                    HostManager.TryToSendQueuedRequests();
            }
        }

        private static byte _maxConnectionPerServer;

        /// <summary>
        /// Default value of a HTTP request's IsKeepAlive value. Default value is true. If you make rare request to the server it should be changed to false.
        /// </summary>
        public static bool KeepAliveDefaultValue { get; set; }

#if !BESTHTTP_DISABLE_CACHING
        /// <summary>
        /// Set to true, if caching is prohibited.
        /// </summary>
        public static bool IsCachingDisabled => false;
#endif

        /// <summary>
        /// How many time must be passed to destroy that connection after a connection finished its last request. Its default value is 20 seconds.
        /// </summary>
        public static TimeSpan MaxConnectionIdleTime { get; set; }

#if !BESTHTTP_DISABLE_COOKIES
        /// <summary>
        /// Set to false to disable all Cookie. It's default value is true.
        /// </summary>
        public static bool IsCookiesEnabled { get; set; }
#endif

        /// <summary>
        /// Size of the Cookie Jar in bytes. It's default value is 10485760 (10 MB).
        /// </summary>
        public static uint CookieJarSize { get; set; }

        /// <summary>
        /// If this property is set to true, then new cookies treated as session cookies and these cookies are not saved to disk. Its default value is false;
        /// </summary>
        public static bool EnablePrivateBrowsing { get; set; }

        /// <summary>
        /// Global, default value of the HTTPRequest's ConnectTimeout property. If set to TimeSpan.Zero or lower, no connect timeout logic is executed. Default value is 20 seconds.
        /// </summary>
        public static TimeSpan ConnectTimeout { get; set; }

        /// <summary>
        /// Global, default value of the HTTPRequest's Timeout property. Default value is 60 seconds.
        /// </summary>
        public static TimeSpan RequestTimeout { get; set; }

        /// <summary>
        /// By default the plugin will save all cache and cookie data under the path returned by Application.persistentDataPath.
        /// You can assign a function to this delegate to return a custom root path to define a new path.
        /// <remarks>This delegate will be called on a non Unity thread!</remarks>
        /// </summary>
        private static Func<string> RootCacheFolderProvider => null;

#if !BESTHTTP_DISABLE_PROXY
        /// <summary>
        /// The global, default proxy for all HTTPRequests. The HTTPRequest's Proxy still can be changed per-request. Default value is null.
        /// </summary>
        public static Proxy Proxy => null;
#endif

        /// <summary>
        /// Heartbeat manager to use less threads in the plugin. The heartbeat updates are called from the OnUpdate function.
        /// </summary>
        public static HeartbeatManager Heartbeats
        {
            get { return _heartbeats ??= new HeartbeatManager(); }
        }

        private static HeartbeatManager _heartbeats;

        /// <summary>
        /// A basic BestHTTP.Logger.ILogger implementation to be able to log intelligently additional informations about the plugin's internal mechanism.
        /// </summary>
        public static ILogger Logger
        {
            get
            {
                // Make sure that it has a valid logger instance.
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
        public static TlsClientFactoryDelegate TlsClientFactory { get; set; }
#pragma warning restore CS0169

        public static Connections.TLS.AbstractTls13Client DefaultTlsClientFactory(HttpRequest request,
            List<SecureProtocol.Org.BouncyCastle.Tls.ProtocolName> protocols)
        {
            // http://tools.ietf.org/html/rfc3546#section-3.1
            // -It is RECOMMENDED that clients include an extension of type "server_name" in the client hello whenever they locate a server by a supported name type.
            // -Literal IPv4 and IPv6 addresses are not permitted in "HostName".

            // User-defined list has a higher priority
            List<SecureProtocol.Org.BouncyCastle.Tls.ServerName> hostNames = null;

            // If there's no user defined one and the host isn't an IP address, add the default one
            if (!request.CurrentUri.IsHostIsAnIPAddress())
            {
                hostNames = new List<SecureProtocol.Org.BouncyCastle.Tls.ServerName>(1)
                {
                    new SecureProtocol.Org.BouncyCastle.Tls.ServerName(0,
                        System.Text.Encoding.UTF8.GetBytes(request.CurrentUri.Host))
                };
            }

            return new Connections.TLS.DefaultTls13Client(request, hostNames, protocols);
        }

        /// <summary>
        /// The default value for the HTTPRequest's UseAlternateSSL property.
        /// </summary>
        public static bool UseAlternateSSLDefaultValue { get; set; }
#endif

#if !NETFX_CORE
#pragma warning disable CS0169

        public static Func<HttpRequest, X509Certificate, X509Chain, SslPolicyErrors, bool>
            DefaultCertificationValidator => null;
#pragma warning restore CS0169
#pragma warning disable CS0169
        public static ClientCertificateSelector ClientCertificationProvider { get; set; }
#pragma warning restore CS0169
#endif

        /// <summary>
        /// TCP Client's send buffer size.
        /// </summary>
#pragma warning disable CS0169
        // ReSharper disable once InconsistentNaming
        public static int? SendBufferSize;
#pragma warning restore CS0169

        /// <summary>
        /// TCP Client's receive buffer size.
        /// </summary>
#pragma warning disable CS0169
        // ReSharper disable once UnassignedField.Global
        public static int? ReceiveBufferSize;
#pragma warning restore CS0169

        /// <summary>
        /// An IIOService implementation to handle filesystem operations.
        /// </summary>
        public static readonly PlatformSupport.FileSystem.IIOService IOService;

        /// <summary>
        /// On most systems the maximum length of a path is around 255 character. If a cache entity's path is longer than this value it doesn't get cached. There no platform independent API to query the exact value on the current system, but it's
        /// exposed here and can be overridden. It's default value is 255.
        /// </summary>
        internal static int MaxPathLength { get; set; }

        /// <summary>
        /// User-agent string that will be sent with each requests.
        /// </summary>
        public const string UserAgent = "BestHTTP/2 v2.6.3";

        /// <summary>
        /// It's true if the application is quitting and the plugin is shutting down itself.
        /// </summary>
        public static bool IsQuitting
        {
            get => _isQuitting;
            private set => _isQuitting = value;
        }

        private static volatile bool _isQuitting;

        #endregion

        #region Public Interface

        public static void Setup()
        {
            if (_isSetupCalled)
                return;
            _isSetupCalled = true;
            IsQuitting = false;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[HTTPManager] [method:Setup] [msg]Setup called! UserAgent: {UserAgent}");
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

        public static HttpRequest SendRequest(string url, OnRequestFinishedDelegate callback)
        {
            return SendRequest(new HttpRequest(new Uri(url), HttpMethods.Get, callback));
        }

        public static HttpRequest SendRequest(string url, HttpMethods methodType, OnRequestFinishedDelegate callback)
        {
            return SendRequest(new HttpRequest(new Uri(url), methodType, callback));
        }

        public static HttpRequest SendRequest(string url, HttpMethods methodType, bool isKeepAlive,
            OnRequestFinishedDelegate callback)
        {
            return SendRequest(new HttpRequest(new Uri(url), methodType, isKeepAlive, callback));
        }

        public static HttpRequest SendRequest(string url, HttpMethods methodType, bool isKeepAlive, bool disableCache,
            OnRequestFinishedDelegate callback)
        {
            return SendRequest(new HttpRequest(new Uri(url), methodType, isKeepAlive, disableCache, callback));
        }

        public static HttpRequest SendRequest(HttpRequest request)
        {
            if (!_isSetupCalled)
                Setup();

            if (request.IsCancellationRequested || IsQuitting)
                return request;

#if !BESTHTTP_DISABLE_CACHING
            // If possible load the full response from cache.
            if (HttpCacheService.IsCachedEntityExpiresInTheFuture(request))
            {
                var started = DateTime.Now;
                PlatformSupport.Threading.ThreadedRunner.RunShortLiving((req) =>
                {
                    if (ConnectionHelper.TryLoadAllFromCache("HTTPManager", req, req.Context))
                    {
                        req.Timing.Add("Full Cache Load", DateTime.Now - started);
                        req.State = HttpRequestStates.Finished;
                    }
                    else
                    {
                        // If for some reason it couldn't load we place back the request to the queue.

                        request.State = HttpRequestStates.Queued;
                        RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(request, RequestEvents.Resend));
                    }
                }, request);
            }
            else
#endif
            {
                request.State = HttpRequestStates.Queued;
                RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(request, RequestEvents.Resend));
            }

            return request;
        }

        #endregion

        #region Internal Helper Functions

        /// <summary>
        /// Will return where the various caches should be saved.
        /// </summary>
        public static string GetRootCacheFolder()
        {
            try
            {
                if (RootCacheFolderProvider != null)
                    return RootCacheFolderProvider();
            }
            catch (Exception ex)
            {
                HttpManager.Logger.Exception("HTTPManager", "GetRootCacheFolder", ex);
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

        #endregion

        #region MonoBehaviour Events (Called from HTTPUpdateDelegator)

        /// <summary>
        /// Update function that should be called regularly from a Unity event(Update, LateUpdate). Callbacks are dispatched from this function.
        /// </summary>
        public static void OnUpdate()
        {
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
            Debug.Log(
                $"[HTTPManager] [method:OnQuit] [msg] OnQuit called!");
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
            Debug.Log(
                $"[HTTPManager] [method:AbortAll] [msg] AbortAll called!");
#endif
            // This is an immediate shutdown request!

            RequestEventHelper.Clear();
            ConnectionEventHelper.Clear();
            PluginEventHelper.Clear();
            ProtocolEventHelper.Clear();

            HostManager.Shutdown();

            ProtocolEventHelper.CancelActiveProtocols();
        }

        #endregion
    }
}