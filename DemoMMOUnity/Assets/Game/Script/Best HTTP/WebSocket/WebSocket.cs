#if !BESTHTTP_DISABLE_WEBSOCKET

#if !UNITY_WEBGL || UNITY_EDITOR
using System;
using System.Text;
using BestHTTP.Connections;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.WebSocket.Frames;
using BestHTTP.WebSocket.Extensions;
#endif

namespace BestHTTP.WebSocket
{
    public sealed class WebSocket
    {
        /// <summary>
        /// websocket帧的最大有效负载大小。默认值为32kib。
        /// </summary>
        public const uint MaxFragmentSize = ushort.MaxValue / 2;

        /// <summary>
        /// 底层的、真正的实现。
        /// </summary>
        private WebSocketBaseImplementation _implementation;

        /// <summary>
        /// 当从服务器接收到新的二进制消息时调用。
        /// </summary>
        public OnWebSocketBinaryDelegate OnBinary;

        /// <summary>
        /// 当WebSocket连接关闭时调用。
        /// </summary>
        public OnWebSocketClosedDelegate OnClosed;

        /// <summary>
        /// 当遇到错误时调用。参数将是错误的描述。
        /// </summary>
        public OnWebSocketErrorDelegate OnError;

#if !UNITY_WEBGL || UNITY_EDITOR
        /// <summary>
        /// 当接收到不完整的帧时调用。不会尝试在内部重新组装这些片段，并且在此事件之后不会存储对该框架的引用。
        /// </summary>
        // ReSharper disable once UnassignedField.Global
        public OnWebSocketIncompleteFrameDelegate OnIncompleteFrame;
#endif

        /// <summary>
        /// 当从服务器接收到新的文本消息时调用。
        /// </summary>
        public OnWebSocketMessageDelegate OnMessage;

        /// <summary>
        /// 当与WebSocket服务器建立连接时调用。
        /// </summary>
        public OnWebSocketOpenDelegate OnOpen;

        /// <summary>
        /// 从给定uri创建一个WebSocket实例。
        /// </summary>
        /// <param name="uri">WebSocket服务器的uri</param>
        public WebSocket(Uri uri)
            : this(
                uri: uri,
                origin: string.Empty,
                protocol: string.Empty)
        {
#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_GZIP
            var perMessage = new PerMessageCompression(
                level: Decompression.Zlib.CompressionLevel.Default,
                clientNoContextTakeover:  false,
                serverNoContextTakeover: false,
                desiredClientMaxWindowBits:  Decompression.Zlib.ZlibConstants.WindowBitsMax,
                desiredServerMaxWindowBits: Decompression.Zlib.ZlibConstants.WindowBitsMax, 
                minDataLengthToCompress: PerMessageCompression.MinDataLengthToCompressDefault);
            this.Extensions = new IExtension[]
            {
                perMessage
            };
#endif
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        private WebSocket(Uri uri, string origin, string protocol)
            : this(uri, origin, protocol, null)
        {
#if !BESTHTTP_DISABLE_GZIP
            var perMessage = new PerMessageCompression(
                level: Decompression.Zlib.CompressionLevel.Default,
                clientNoContextTakeover:  false,
                serverNoContextTakeover: false,
                desiredClientMaxWindowBits:  Decompression.Zlib.ZlibConstants.WindowBitsMax,
                desiredServerMaxWindowBits: Decompression.Zlib.ZlibConstants.WindowBitsMax, 
                minDataLengthToCompress: PerMessageCompression.MinDataLengthToCompressDefault);
            this.Extensions = new IExtension[]
            {
                perMessage
            };
#endif
        }
#endif

        /// <summary>
        /// Creates a WebSocket instance from the given uri, protocol and origin.
        /// </summary>
        /// <param name="uri">The uri of the WebSocket server</param>
        /// <param name="origin">Servers that are not intended to process input from any web page but only for certain sites SHOULD verify the |Origin| field is an origin they expect.
        /// If the origin indicated is unacceptable to the server, then it SHOULD respond to the WebSocket handshake with a reply containing HTTP 403 Forbidden status code.</param>
        /// <param name="protocol">The application-level protocol that the client want to use(eg. "chat", "leaderboard", etc.). Can be null or empty string if not used.</param>
        /// <param name="extensions">Optional IExtensions implementations</param>
        public WebSocket(Uri uri, string origin, string protocol
#if !UNITY_WEBGL || UNITY_EDITOR
            , params IExtension[] extensions
#endif
        )

        {
            this.Context = new LoggingContext(this);

#if !UNITY_WEBGL || UNITY_EDITOR
            this.Extensions = extensions;

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2
            if (HttpManager.Http2Settings.WebSocketOverHttp2Settings.EnableWebSocketOverHttp2 &&
                HttpProtocolFactory.IsSecureProtocol(uri))
            {
                // Try to find a HTTP/2 connection that supports the connect protocol.
                var con = BestHTTP.Core.HostManager.GetHost(uri.Host).GetHostDefinition(Core.HostDefinition.GetKeyFor(
                    new UriBuilder("https", uri.Host, uri.Port).Uri
#if !BESTHTTP_DISABLE_PROXY
                    , GetProxy(uri)
#endif
                )).Find(c =>
                {
                    var httpConnection = c as HTTPConnection;
                    var http2Handler = httpConnection?.requestHandler as Connections.HTTP2.Http2Handler;

                    return http2Handler != null &&
                           http2Handler.Settings.RemoteSettings
                               [Connections.HTTP2.Http2Settings.EnableConnectProtocol] != 0;
                });

                if (con != null)
                {
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        var str = "Connection with enabled Connect Protocol found!";
                        Debug.Log($"[WebSocket] [method:WebSocket] [msg] {str}");
#endif
                    }

                    var httpConnection = con as HTTPConnection;
                    var http2Handler = httpConnection?.requestHandler as Connections.HTTP2.Http2Handler;

                    this._implementation = new OverHTTP2(this, http2Handler, uri, origin, protocol);
                }
            }
#endif

            if (this._implementation == null)
                this._implementation = new OverHTTP1(this, uri, origin, protocol);
#else
            this.implementation = new WebGLBrowser(this, uri, origin, protocol);
#endif

            // Under WebGL when only the WebSocket protocol is used Setup() isn't called, so we have to call it here.
            HttpManager.Setup();
        }

        public WebSocketStates State
        {
            get { return this._implementation.State; }
        }

        /// <summary>
        /// The connection to the WebSocket server is open.
        /// </summary>
        public bool IsOpen
        {
            get { return this._implementation.IsOpen; }
        }

        /// <summary>
        /// Data waiting to be written to the wire.
        /// </summary>
        public int BufferedAmount
        {
            get { return this._implementation.BufferedAmount; }
        }

        /// <summary>
        /// Logging context of this websocket instance.
        /// </summary>
        public LoggingContext Context { get; private set; }

#if !UNITY_WEBGL || UNITY_EDITOR
        internal void FallbackToHTTP1()
        {
            if (this._implementation == null)
                return;

            this._implementation = new OverHTTP1(this, this._implementation.Uri, this._implementation.Origin,
                this._implementation.Protocol);
            this._implementation.StartOpen();
        }
#endif

        /// <summary>
        /// Start the opening process.
        /// </summary>
        public void Open()
        {
            this._implementation.StartOpen();
        }

        /// <summary>
        /// It will send the given message to the server in one frame.
        /// </summary>
        public void Send(string message)
        {
            if (!IsOpen)
                return;

            this._implementation.Send(message);
        }

        /// <summary>
        /// It will send the given data to the server in one frame.
        /// </summary>
        public void Send(byte[] buffer)
        {
            if (!IsOpen)
                return;

            this._implementation.Send(buffer);
        }

        /// <summary>
        /// Will send count bytes from a byte array, starting from offset.
        /// </summary>
        public void Send(byte[] buffer, ulong offset, ulong count)
        {
            if (!IsOpen)
                return;

            this._implementation.Send(buffer, offset, count);
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        /// <summary>
        /// It will send the given frame to the server.
        /// </summary>
        public void Send(WebSocketFrame frame)
        {
            if (!IsOpen)
                return;

            this._implementation.Send(frame);
        }
#endif

        /// <summary>
        /// It will initiate the closing of the connection to the server.
        /// </summary>
        public void Close()
        {
            if (State >= WebSocketStates.Closing)
                return;

            this._implementation.StartClose(1000, "Bye!");
        }

        /// <summary>
        /// It will initiate the closing of the connection to the server sending the given code and message.
        /// </summary>
        public void Close(UInt16 code, string message)
        {
            if (!IsOpen)
                return;

            this._implementation.StartClose(code, message);
        }

#if !BESTHTTP_DISABLE_PROXY
        internal Proxy GetProxy(Uri uri)
        {
            // WebSocket is not a request-response based protocol, so we need a 'tunnel' through the proxy
            HTTPProxy proxy = HttpManager.Proxy as HTTPProxy;
            if (proxy != null && proxy.UseProxyForAddress(uri))
                proxy = new HTTPProxy(proxy.Address,
                    proxy.Credentials,
                    false, /*turn on 'tunneling'*/
                    false, /*sendWholeUri*/
                    proxy.NonTransparentForHTTPS);

            return proxy;
        }
#endif

#if !UNITY_WEBGL || UNITY_EDITOR

        /// <summary>
        /// Set to true to start a new thread to send Pings to the WebSocket server
        /// </summary>
        public bool StartPingThread { get; set; }

        /// <summary>
        /// The delay between two Pings in milliseconds. Minimum value is 100, default is 1000.
        /// </summary>
        public int PingFrequency { get; set; }

        /// <summary>
        /// If StartPingThread set to true, the plugin will close the connection and emit an OnError event if no
        /// message is received from the server in the given time. Its default value is 2 sec.
        /// </summary>
        public TimeSpan CloseAfterNoMessage { get; set; }

        /// <summary>
        /// The internal HTTPRequest object.
        /// </summary>
        public HttpRequest InternalRequest
        {
            get { return this._implementation.InternalRequest; }
        }

        /// <summary>
        /// IExtension implementations the plugin will negotiate with the server to use.
        /// </summary>
        public IExtension[] Extensions { get; private set; }

        /// <summary>
        /// Latency calculated from the ping-pong message round-trip times.
        /// </summary>
        public int Latency
        {
            get { return this._implementation.Latency; }
        }

        /// <summary>
        /// When we received the last message from the server.
        /// </summary>
        public DateTime LastMessageReceived
        {
            get { return this._implementation.LastMessageReceived; }
        }

        /// <summary>
        /// When the Websocket Over HTTP/2 implementation fails to connect and EnableImplementationFallback is true, the plugin tries to fall back to the HTTP/1 implementation.
        /// When this happens a new InternalRequest is created and all previous custom modifications (like added headers) are lost. With OnInternalRequestCreated these modifications can be reapplied.
        /// </summary>
        public Action<WebSocket, HttpRequest> OnInternalRequestCreated;
#endif

#if !UNITY_WEBGL || UNITY_EDITOR

        public static byte[] EncodeCloseData(UInt16 code, string message)
        {
            //If there is a body, the first two bytes of the body MUST be a 2-byte unsigned integer
            // (in network byte order) representing a status code with value /code/ defined in Section 7.4 (http://tools.ietf.org/html/rfc6455#section-7.4). Following the 2-byte integer,
            // the body MAY contain UTF-8-encoded data with value /reason/, the interpretation of which is not defined by this specification.
            // This data is not necessarily human readable but may be useful for debugging or passing information relevant to the script that opened the connection.
            int msgLen = Encoding.UTF8.GetByteCount(message);
            using (var ms = new BufferPoolMemoryStream(2 + msgLen))
            {
                byte[] buff = BitConverter.GetBytes(code);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buff, 0, buff.Length);

                ms.Write(buff, 0, buff.Length);

                buff = Encoding.UTF8.GetBytes(message);
                ms.Write(buff, 0, buff.Length);

                return ms.ToArray();
            }
        }

        internal static string GetSecKey(object[] from)
        {
            const int keysLength = 16;
            byte[] keys = BufferPool.Get(keysLength, true);
            int pos = 0;

            for (int i = 0; i < from.Length; ++i)
            {
                byte[] hash = BitConverter.GetBytes((Int32)from[i].GetHashCode());

                for (int cv = 0; cv < hash.Length && pos < keysLength; ++cv)
                    keys[pos++] = hash[cv];
            }

            var result = Convert.ToBase64String(keys, 0, keysLength);
            BufferPool.Release(keys);

            return result;
        }
#endif
    }
}

#endif