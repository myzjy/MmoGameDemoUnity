#if !BESTHTTP_DISABLE_WEBSOCKET

#if !UNITY_WEBGL || UNITY_EDITOR
using System;
using System.Diagnostics;
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
                clientNoContextTakeover: false,
                serverNoContextTakeover: false,
                desiredClientMaxWindowBits: Decompression.Zlib.ZlibConstants.WindowBitsMax,
                desiredServerMaxWindowBits: Decompression.Zlib.ZlibConstants.WindowBitsMax,
                minDataLengthToCompress: PerMessageCompression.MinDataLengthToCompressDefault);
            this.Extensions = new IExtension[]
            {
                perMessage
            };
#endif
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        /// <summary>
        /// 根据给定的uri，协议和origin创建一个WebSocket实例。
        /// </summary>
        /// <param name="uri">WebSocket服务器的uri</param>
        /// <param name="origin">
        ///<p>WebSocket服务器的uri不打算处理来自任何网页的输入，</p>
        /// <p>但只用于某些网站应该验证|Origin|字段是他们期望的来源。</p>
        /// <p>如果指定的原点对服务器来说是不可接受的，</p>
        /// <p>那么它应该用包含HTTP 403禁止状态码的应答来响应WebSocket握手。</p>
        /// </param>
        /// <param name="protocol">客户端想要使用的应用程序级协议(例如。“聊天”、“排行榜”等等)。如果不使用，可以为null或空字符串吗.</param>
        private WebSocket(Uri uri, string origin, string protocol)
            : this(
                uri: uri,
                origin: origin,
                protocol: protocol,
                extensions: null)
        {
#if !BESTHTTP_DISABLE_GZIP
            var perMessage = new PerMessageCompression(
                level: Decompression.Zlib.CompressionLevel.Default,
                clientNoContextTakeover: false,
                serverNoContextTakeover: false,
                desiredClientMaxWindowBits: Decompression.Zlib.ZlibConstants.WindowBitsMax,
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
        /// 根据给定的uri，协议和origin创建一个WebSocket实例。
        /// </summary>
        /// <param name="uri">WebSocket服务器的uri</param>
        /// <param name="origin">
        ///<p>WebSocket服务器的uri不打算处理来自任何网页的输入，</p>
        /// <p>但只用于某些网站应该验证|Origin|字段是他们期望的来源。</p>
        /// <p>如果指定的原点对服务器来说是不可接受的，</p>
        /// <p>那么它应该用包含HTTP 403禁止状态码的应答来响应WebSocket握手。</p>
        /// </param>
        /// <param name="protocol">客户端想要使用的应用程序级协议(例如。“聊天”、“排行榜”等等)。如果不使用，可以为null或空字符串吗.</param>
        /// <param name="extensions">可选的IExtensions实现</param>
        private WebSocket(Uri uri, string origin, string protocol
#if !UNITY_WEBGL || UNITY_EDITOR
            , params IExtension[] extensions
#endif
        )

        {
            this.Context = new LoggingContext(boundto: this);

#if !UNITY_WEBGL || UNITY_EDITOR
            this.Extensions = extensions;

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2
            if (HttpManager.Http2Settings.WebSocketOverHttp2Settings.EnableWebSocketOverHttp2 &&
                HttpProtocolFactory.IsSecureProtocol(uri: uri))
            {
                var uriBuilder = new UriBuilder("https", uri.Host, uri.Port);
                // 尝试找到一个支持连接协议的HTTP/2连接。
                var con = Core.HostManager.GetHost(uri.Host)
                    .GetHostDefinition(Core.HostDefinition.GetKeyFor(uriBuilder.Uri
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
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    {
                        var st = new StackTrace(new StackFrame(true));
                        var sf = st.GetFrame(0);
                        StringBuilder sb = new StringBuilder(6);
                        sb.Append($"[{sf.GetFileName()}]");
                        sb.Append($"[method:{sf.GetMethod().Name}]");
                        sb.Append($"{sf.GetMethod().Name}");
                        sb.Append($"Line:{sf.GetFileLineNumber()}");
                        sb.Append($"[msg]已找到启用连接协议的连接!");
                        Debug.Log($"{sb}");
                    }
#endif

                    var httpConnection = con as HTTPConnection;
                    var http2Handler = httpConnection?.requestHandler as Connections.HTTP2.Http2Handler;

                    this._implementation = new OverHttp2(
                        parent: this,
                        handler: http2Handler,
                        uri: uri,
                        origin: origin,
                        protocol: protocol);
                }
            }
#endif

            this._implementation ??= new OverHTTP1(
                parent: this,
                uri: uri,
                origin: origin,
                protocol: protocol);
#else
            this.implementation = new WebGLBrowser(this, uri, origin, protocol);
#endif

            // 在WebGL下，当只使用WebSocket协议时，不会调用Setup()，所以我们必须在这里调用它。
            HttpManager.Setup();
        }

        public WebSocketStates State => this._implementation.State;

        /// <summary>
        /// WebSocket服务器的连接是打开的。
        /// </summary>
        public bool IsOpen => this._implementation.IsOpen;

        /// <summary>
        ///等待写入线路的数据。
        /// </summary>
        public int BufferedAmount => this._implementation.BufferedAmount;

        /// <summary>
        /// 这个websocket实例的日志上下文。
        /// </summary>
        public LoggingContext Context { get; private set; }

#if !UNITY_WEBGL || UNITY_EDITOR
        internal void FallbackToHttp1()
        {
            if (this._implementation == null)
            {
                return;
            }

            this._implementation = new OverHTTP1(
                parent: this,
                uri: this._implementation.Uri,
                origin: this._implementation.Origin,
                protocol: this._implementation.Protocol);
            this._implementation.StartOpen();
        }
#endif

        /// <summary>
        /// 启动打开流程。
        /// </summary>
        public void Open()
        {
            this._implementation.StartOpen();
        }

        /// <summary>
        /// 它将在一帧内将给定的消息发送给服务器.
        /// </summary>
        public void Send(string message)
        {
            if (!IsOpen)
            {
                return;
            }

            this._implementation.Send(message);
        }

        /// <summary>
        /// 它将在一帧内将给定的数据发送给服务器。
        /// </summary>
        public void Send(byte[] buffer)
        {
            if (!IsOpen)
            {
                return;
            }

            this._implementation.Send(buffer);
        }

        /// <summary>
        /// 将从字节数组中发送count字节，从偏移量开始。
        /// </summary>
        public void Send(byte[] buffer, ulong offset, ulong count)
        {
            if (!IsOpen)
            {
                return;
            }

            this._implementation.Send(buffer, offset, count);
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        /// <summary>
        /// 它将把给定的帧发送到服务器。
        /// </summary>
        public void Send(WebSocketFrame frame)
        {
            if (!IsOpen)
                return;

            this._implementation.Send(frame);
        }
#endif

        /// <summary>
        /// 它将启动关闭与服务器的连接。
        /// </summary>
        public void Close()
        {
            if (State >= WebSocketStates.Closing)
                return;

            this._implementation.StartClose(1000, "Bye!");
        }

        /// <summary>
        ///它将启动关闭与发送给定代码和消息的服务器的连接。
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
            //WebSocket不是一个基于请求-响应的协议，所以我们需要一个通过代理的“隧道”
            HTTPProxy proxy = HttpManager.Proxy as HTTPProxy;
            if (proxy != null && proxy.UseProxyForAddress(uri))
            {
                proxy = new HTTPProxy(proxy.Address,
                    proxy.Credentials,
                    false, /*turn on 'tunneling'*/
                    false, /*sendWholeUri*/
                    proxy.NonTransparentForHTTPS);
            }

            return proxy;
        }
#endif

#if !UNITY_WEBGL || UNITY_EDITOR

        /// <summary>
        /// 设置为true将启动一个新线程向WebSocket服务器发送ping
        /// </summary>
        public bool StartPingThread { get; set; }

        /// <summary>
        /// 两个ping之间的延迟，以毫秒为单位。最小值为100，默认值为1000。
        /// </summary>
        public int PingFrequency { get; set; }

        /// <summary>
        ///如果StartPingThread设置为true，插件将关闭连接，如果不设置则触发OnError事件
        ///在给定时间内从服务器接收消息。缺省值是2秒。
        /// </summary>
        public TimeSpan CloseAfterNoMessage { get; set; }

        /// <summary>
        ///内部HTTPRequest对象。
        /// </summary>
        public HttpRequest InternalRequest => this._implementation.InternalRequest;

        /// <summary>
        /// 扩展实现插件将与服务器协商使用。
        /// </summary>
        public IExtension[] Extensions { get; private set; }

        /// <summary>
        /// 根据ping-pong消息往返时间计算的延迟。
        /// </summary>
        public int Latency => this._implementation.Latency;

        /// <summary>
        /// 当我们收到服务器发来的最后一条消息时。
        /// </summary>
        public DateTime LastMessageReceived => this._implementation.LastMessageReceived;

        /// <summary>
        ///当Websocket Over HTTP/2实现连接失败且EnableImplementationFallback为true时，插件尝试回退到HTTP/1实现。
        ///当这种情况发生时，一个新的InternalRequest被创建，所有以前的自定义修改(如添加的头)将丢失。使用OnInternalRequestCreated，这些修改可以被重新应用。
        /// </summary>
        public Action<WebSocket, HttpRequest> OnInternalRequestCreated;
#endif

#if !UNITY_WEBGL || UNITY_EDITOR

        public static byte[] EncodeCloseData(UInt16 code, string message)
        {
            //如果有body, body的前两个字节必须是一个2字节的无符号整数
            //(在网络字节顺序中)表示在7.4节(http://tools.ietf.org/html/rfc6455#section-7.4)中定义的值/code/的状态码。在2字节整数之后，
            // body可以包含utf -8编码的值为/reason/的数据，其解释在本规范中没有定义。
            //这些数据不一定是人类可读的，但可能对调试或传递与打开连接的脚本相关的信息有用。
            int msgLen = Encoding.UTF8.GetByteCount(message);
            using var ms = new BufferPoolMemoryStream(2 + msgLen);
            byte[] buff = BitConverter.GetBytes(code);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buff, 0, buff.Length);

            ms.Write(buff, 0, buff.Length);

            buff = Encoding.UTF8.GetBytes(message);
            ms.Write(buff, 0, buff.Length);

            return ms.ToArray();
        }

        internal static string GetSecKey(object[] from)
        {
            const int keysLength = 16;
            byte[] keys = BufferPool.Get(keysLength, true);
            int pos = 0;

            foreach (var t in from)
            {
                byte[] hash = BitConverter.GetBytes(t.GetHashCode());

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