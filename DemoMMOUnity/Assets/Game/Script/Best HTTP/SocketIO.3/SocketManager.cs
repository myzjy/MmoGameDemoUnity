#if !BESTHTTP_DISABLE_SOCKETIO

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using BestHTTP.SocketIO3.Transports;
using BestHTTP.Extensions;
using BestHTTP.SocketIO3.Parsers;
using BestHTTP.SocketIO3.Events;
using BestHTTP.Logger;

namespace BestHTTP.SocketIO3
{
    public sealed class SocketManager : IHeartbeat, IManager
    {
        /// <summary>
        /// SocketManager实例的可能状态。
        /// </summary>
        public enum States
        {
            /// <summary>
            /// SocketManager的初始状态
            /// </summary>
            Initial,

            /// <summary>
            /// SocketManager当前正在打开。
            /// </summary>
            Opening,

            /// <summary>
            /// SocketManager是打开的，事件可以发送到服务器。
            /// </summary>
            Open,

            /// <summary>
            /// 暂停传输升级
            /// </summary>
            Paused,

            /// <summary>
            /// 发生了一个错误，SocketManager现在试图再次连接到服务器。
            /// </summary>
            Reconnecting,

            /// <summary>
            /// SocketManager是关闭的，由用户或服务器发起
            /// </summary>
            Closed
        }

        /// <summary>
        /// 支持的套接字。IO协议版本
        /// </summary>
        public int ProtocolVersion => 4;

        /// <summary>
        /// 此套接字的当前状态。IO 管理。
        /// </summary>
        public States State
        {
            get => _state;
            private set
            {
                PreviousState = _state;
                _state = value;
            }
        }

        private States _state;

        /// <summary>
        /// 此管理器将使用的SocketOptions实例。
        /// </summary>
        public SocketOptions Options { get; private set; }

        /// <summary>
        /// Socket。IO的Uri端点。
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// 服务器发送并解析握手数据。
        /// </summary>
        public HandshakeData Handshake { get; private set; }

        /// <summary>
        /// 当前使用的主传输实例。
        /// </summary>
        private ITransport Transport { get; set; }

        /// <summary>
        /// 基于请求的传输的请求计数器。
        /// </summary>
        public ulong RequestCounter { get; internal set; }

        /// <summary>
        /// The root("/") Socket.
        /// </summary>
        public Socket Socket => GetSocket();

        /// <summary>
        /// 索引器访问与给定名称空间关联的套接字。
        /// </summary>
        public Socket this[string nsp] => GetSocket(nsp);

        /// <summary>
        /// 尝试了多少次重新连接。
        /// </summary>
        private int ReconnectAttempts { get; set; }

        /// <summary>
        /// 解析器来编码和解码消息并创建强类型对象。
        /// </summary>
        public IParser Parser { get; set; }

        /// <summary>
        /// 此套接字的日志上下文。输入输出连接。
        /// </summary>
        public LoggingContext Context { get; private set; }

        /// <summary>
        /// 时间戳支持基于请求的传输。
        /// </summary>
        internal static ulong Timestamp
        {
            get
            {
                var dateTime = new DateTime(1970, 1, 1);
                return (ulong)(DateTime.UtcNow.Subtract(dateTime)).TotalMilliseconds;
            }
        }

        /// <summary>
        /// 自动递增属性以返回Ack id。
        /// </summary>
        internal int NextAckId => System.Threading.Interlocked.Increment(ref _nextAckId);

        private int _nextAckId;

        /// <summary>
        /// 用于存储管理器以前状态的内部属性。
        /// </summary>
        private States PreviousState { get; set; }

        /// <summary>
        /// 交通正在升级。
        /// </summary>
        internal ITransport UpgradingTransport { get; set; }


        /// <summary>
        /// Namespace name -> Socket mapping
        /// </summary>
        private readonly Dictionary<string, Socket> _namespaces = new Dictionary<string, Socket>();

        /// <summary>
        /// 套接字列表，以便轻松遍历它们。
        /// </summary>
        private readonly List<Socket> _sockets = new List<Socket>();

        /// <summary>
        /// 未发送的包列表。只在必须使用时实例化。
        /// </summary>
        private List<OutgoingPacket> _offlinePackets;

        /// <summary>
        /// 当我们发出最后一个心跳(Ping)消息时。
        /// </summary>
        private DateTime _lastHeartbeat = DateTime.MinValue;

        /// <summary>
        ///当我们尝试重新连接时
        /// </summary>
        private DateTime _reconnectAt;

        /// <summary>
        /// 当我们开始连接到服务器时。
        /// </summary>
        private DateTime _connectionStarted;

        /// <summary>
        /// 私有标志，以避免多次关闭呼叫
        /// </summary>
        private bool _closing;

        /// <summary>
        /// 在引擎。IO v4 / socket。IO v3服务器发送ping消息，而不是客户端。
        /// </summary>
        private DateTime _lastPingReceived;


        /// <summary>
        ///构造函数来创建一个SocketManager实例，该实例将连接到给定的uri。
        /// </summary>
        public SocketManager(Uri uri)
            : this(
                uri: uri,
                parser: new DefaultJsonParser(),
                options: new SocketOptions())
        {
        }

        public SocketManager(Uri uri, IParser parser)
            : this(
                uri: uri,
                parser: parser,
                options: new SocketOptions())
        {
        }

        public SocketManager(Uri uri, SocketOptions options)
            : this(
                uri: uri,
                parser: new DefaultJsonParser(),
                options: options)
        {
        }

        /// <summary>
        /// 构造函数来创建SocketManager实例。
        /// </summary>
        private SocketManager(Uri uri, IParser parser, SocketOptions options)
        {
            this.Context = new LoggingContext(this);

            var path = uri.PathAndQuery;
            if (path.Length <= 1)
            {
                var append = uri.OriginalString[^1] == '/' ? "socket.io/" : "/socket.io/";

                uri = new Uri(uri.OriginalString + append);
            }

            this.Uri = uri;
            this.Options = options ?? new SocketOptions();
            this.State = States.Initial;
            this.PreviousState = States.Initial;
            this.Parser = parser ?? new DefaultJsonParser();

            if (!uri.Scheme.StartsWith("ws")) return;
            if (options != null)
            {
                options.ConnectWith = TransportTypes.WebSocket;
            }
        }

        /// <summary>
        /// Returns with the specified namespace
        /// </summary>
        public Socket GetSocket(string nsp = "/")
        {
            if (string.IsNullOrEmpty(nsp))
                throw new ArgumentNullException(nameof(nsp));

            /*if (nsp[0] != '/')
                nsp = "/" + nsp;*/

            if (_namespaces.TryGetValue(nsp, out var socket)) return socket;
            // 没有找到套接字，请创建一个
            socket = new Socket(nsp, this);

            _namespaces.Add(nsp, socket);
            _sockets.Add(socket);

            ((ISocket)socket).Open();

            return socket;
        }

        /// <summary>
        /// 从此管理器中删除Socket实例的内部函数。
        /// </summary>
        /// <param name="socket"></param>
        void IManager.Remove(Socket socket)
        {
            _namespaces.Remove(socket.Namespace);
            _sockets.Remove(socket);

            if (_sockets.Count == 0)
                Close();
        }

        /// <summary>
        ///该函数将开始打开Socket。IO连接通过发送握手请求。
        ///如果Options的AutoConnect为true，它将被自动调用。
        /// </summary>
        public void Open()
        {
            if (State != States.Initial &&
                State != States.Closed &&
                State != States.Reconnecting)
                return;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                StringBuilder sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}]");
                sb.Append($"{sf.GetMethod().Name}");
                sb.Append($"Line:{sf.GetFileLineNumber()}");
                sb.Append($"[msg] Opening");
                Debug.Log($"{sb}");
            }
#endif

            _reconnectAt = DateTime.MinValue;

            switch (Options.ConnectWith)
            {
                case TransportTypes.Polling:
                    Transport = new PollingTransport(this);
                    break;
#if !BESTHTTP_DISABLE_WEBSOCKET
                case TransportTypes.WebSocket:
                    Transport = new WebSocketTransport(this);
                    break;
#endif
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Transport.Open();


            (this as IManager).EmitEvent("connecting");

            State = States.Opening;

            _connectionStarted = DateTime.UtcNow;

            HttpManager.Heartbeats.Subscribe(this);

            // The root namespace will be opened by default
            //GetSocket("/");
        }

        /// <summary>
        /// 关闭该套接字。输入输出连接。
        /// </summary>
        private void Close()
        {
            (this as IManager).Close();
        }

        /// <summary>
        /// 关闭该套接字。输入输出连接。
        /// </summary>
        void IManager.Close(bool removeSockets)
        {
            if (State == States.Closed || _closing)
                return;
            _closing = true;

            HttpManager.Logger.Information("SocketManager", "Closing", this.Context);

            HttpManager.Heartbeats.Unsubscribe(this);

            // 断开插座。Disconnect函数将调用Remove函数将其从Sockets列表中移除。
            if (removeSockets)
            {
                while (_sockets.Count > 0)
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    (_sockets[^1] as ISocket).Disconnect(removeSockets);
                }
            }
            else
            {
                foreach (var t in _sockets)
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    ((ISocket)t).Disconnect(removeSockets);
                }
            }

            // 在套接字断开后设置为关闭。通过这种方式，我们可以将断开连接事件发送到服务器。
            State = States.Closed;

            _lastHeartbeat = DateTime.MinValue;
            _lastPingReceived = DateTime.MinValue;

            if (removeSockets && _offlinePackets != null)
            {
                _offlinePackets.Clear();
            }

            // 也要从字典中删除引用。
            if (removeSockets)
            {
                _namespaces.Clear();
            }

            Handshake = null;

            Transport?.Close();

            Transport = null;

            UpgradingTransport?.Close();
            UpgradingTransport = null;

            _closing = false;
        }

        /// <summary>
        /// 在发生错误时从ITransport实现调用，我们可能不得不尝试重新连接。
        /// </summary>
        void IManager.TryToReconnect()
        {
            if (State is States.Reconnecting or States.Closed)
            {
                return;
            }

            if (!Options.Reconnection || HttpManager.IsQuitting)
            {
                Close();

                return;
            }

            if (++ReconnectAttempts >= Options.ReconnectionAttempts)
            {
                (this as IManager).EmitEvent("reconnect_failed");
                Close();

                return;
            }

            Random rand = new Random();

            int delay = (int)Options.ReconnectionDelay.TotalMilliseconds * ReconnectAttempts;

            var randMin = (int)(delay - (delay * Options.RandomizationFactor));
            var randMax = (int)(delay + (delay * Options.RandomizationFactor));
            var mathMin = Math.Min(
                val1: rand.Next(
                    minValue: randMin,
                    maxValue: randMax),
                val2: (int)Options.ReconnectionDelayMax.TotalMilliseconds);
            var fromMilliseconds = TimeSpan.FromMilliseconds(mathMin);
            _reconnectAt = DateTime.UtcNow + fromMilliseconds;

            (this as IManager).Close(false);

            State = States.Reconnecting;

            foreach (var t in _sockets)
            {
                ((ISocket)t).Open();
            }

            // 在Close()函数中我们取消了注册
            HttpManager.Heartbeats.Subscribe(this);

            HttpManager.Logger.Information("SocketManager", "Reconnecting", this.Context);
        }

        /// <summary>
        /// 在连接到服务器时由传输程序调用。
        /// </summary>
        bool IManager.OnTransportConnected(ITransport trans)
        {
            HttpManager.Logger.Information("SocketManager",
                $"OnTransportConnected State: {this.State}, PreviousState: {this.PreviousState}, Current Transport: {trans.Type}, Upgrading Transport: {(UpgradingTransport != null ? UpgradingTransport.Type.ToString() : "null")}",
                this.Context);

            if (State != States.Opening)
            {
                return false;
            }

            if (PreviousState == States.Reconnecting)
            {
                (this as IManager).EmitEvent("reconnect");
            }

            State = States.Open;

            if (PreviousState == States.Reconnecting)
            {
                (this as IManager).EmitEvent("reconnect_before_offline_packets");
            }

            foreach (var socket in _sockets)
            {
                socket?.OnTransportOpen();
            }

            ReconnectAttempts = 0;

            // 发送我们在没有可用的运输工具时收集的数据包。
            SendOfflinePackets();

#if !BESTHTTP_DISABLE_WEBSOCKET
            // 我们可以升级到WebSocket传输吗?
            if (Transport.Type == TransportTypes.WebSocket ||
                !Handshake.Upgrades.Contains("websocket")) return true;
            UpgradingTransport = new WebSocketTransport(this);
            UpgradingTransport.Open();
#endif

            return true;
        }

        void IManager.OnTransportError(ITransport trans, string err)
        {
            if (UpgradingTransport != null && trans != UpgradingTransport)
            {
                return;
            }

            (this as IManager).EmitError(err);

            trans.Close();
            (this as IManager).TryToReconnect();
        }

        void IManager.OnTransportProbed(ITransport trans)
        {
            HttpManager.Logger.Information("SocketManager", "\"probe\" packet received", this.Context);

            // 如果我们必须重新连接，我们将直接使用我们能够升级的传输设备
            Options.ConnectWith = trans.Type;

            // 暂停自己，等待任何发送和接收回合结束。
            State = States.Paused;
        }

        /// <summary>
        /// 选择发送数据包的最佳传输方式。
        /// </summary>
        private ITransport SelectTransport()
        {
            if (State != States.Open || Transport == null)
            {
                return null;
            }

            return Transport.IsRequestInProgress ? null : Transport;
        }

        /// <summary>
        /// 将选择最佳传输并发送OfflinePackets列表中的所有数据包。
        /// </summary>
        private void SendOfflinePackets()
        {
            var trans = SelectTransport();

            //发送我们没有发送的数据包，因为没有可用的传输。
            //该函数在事件处理程序获得'connected'事件之前被调用，因此
            //理论上数据包顺序是保留的。
            if (_offlinePackets is not { Count: > 0 } || trans == null) return;
            trans.Send(_offlinePackets);
            _offlinePackets.Clear();
        }

        /// <summary>
        /// 从Socket类调用的内部函数它会立即发送数据包，或者如果没有可用的传输，它会将数据包存储在OfflinePackets列表中。
        /// </summary>
        void IManager.SendPacket(OutgoingPacket packet)
        {
            HttpManager.Logger.Information("SocketManager", "SendPacket " + packet.ToString(), this.Context);

            var trans = SelectTransport();

            if (trans != null)
            {
                try
                {
                    trans.Send(packet);
                }
                catch (Exception ex)
                {
                    (this as IManager).EmitError(ex.Message + " " + ex.StackTrace);
                }
            }
            else
            {
                if (packet.IsVolatile)
                {
                    return;
                }

                HttpManager.Logger.Information("SocketManager", "SendPacket - Offline stashing packet", this.Context);

                _offlinePackets ??= new List<OutgoingPacket>();

                // 同一个数据包可以通过多个socket发送。
                _offlinePackets.Add(packet);
            }
        }

        /// <summary>
        /// 从当前运行的传输中调用。将传递给必须调用回调函数的Socket。
        /// </summary>
        void IManager.OnPacket(IncomingPacket packet)
        {
            if (State == States.Closed)
            {
                HttpManager.Logger.Information("SocketManager", "OnPacket - State == States.Closed", this.Context);
                return;
            }

            switch (packet.TransportEvent)
            {
                case TransportEventTypes.Open:
                {
                    if (Handshake == null)
                    {
                        Handshake = packet.DecodedArg as HandshakeData;

                        (this as IManager).OnTransportConnected(Transport);

                        return;
                    }
                    else
                        HttpManager.Logger.Information("SocketManager", "OnPacket - Already received handshake data!",
                            this.Context);
                }
                    break;

                case TransportEventTypes.Ping:
                {
                    _lastPingReceived = DateTime.UtcNow;
                    //IncomingPacket pingPacket = new Packet(TransportEventTypes.Pong, SocketIOEventTypes.Unknown, "/", 0);

                    (this as IManager).SendPacket(this.Parser.CreateOutgoing(TransportEventTypes.Pong, null));
                }
                    break;

                case TransportEventTypes.Pong: break;
                case TransportEventTypes.Unknown:
                    break;
                case TransportEventTypes.Close:
                    break;
                case TransportEventTypes.Message:
                    break;
                case TransportEventTypes.Upgrade:
                    break;
                case TransportEventTypes.Noop:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_namespaces.TryGetValue(packet.Namespace, out var socket))
            {
                (socket as ISocket).OnPacket(packet);
            }
            else if (packet.TransportEvent == TransportEventTypes.Message)
            {
                HttpManager.Logger.Warning("SocketManager", "Namespace \"" + packet.Namespace + "\" not found!",
                    this.Context);
            }
        }

        /// <summary>
        /// 向所有可用的名称空间发送一个事件。
        /// </summary>
        public void EmitAll(string eventName, params object[] args)
        {
            foreach (var t in _sockets)
            {
                t.Emit(eventName, args);
            }
        }

        /// <summary>
        /// 如果根名称空间还不存在，则向根名称空间发出内部无包事件而不创建它。
        /// </summary>
        void IManager.EmitEvent(string eventName, params object[] args)
        {
            if (_namespaces.TryGetValue("/", out var socket))
            {
                (socket as ISocket).EmitEvent(eventName, args);
            }
        }

        /// <summary>
        /// 如果根名称空间还不存在，则向根名称空间发出内部无包事件而不创建它。
        /// </summary>
        void IManager.EmitEvent(SocketIOEventTypes type, params object[] args)
        {
            (this as IManager).EmitEvent(EventNames.GetNameFor(type), args);
        }

        void IManager.EmitError(string msg)
        {
            var outComing =
                this.Parser.CreateOutgoing(this._sockets[0], SocketIOEventTypes.Error, -1, null, new Error(msg));
            var inc = outComing.IsBinary
                ? this.Parser.Parse(this, outComing.PayloadData)
                : this.Parser.Parse(this, outComing.Payload);

            (this as IManager).EmitEvent(SocketIOEventTypes.Error, inc.DecodedArg ?? inc.DecodedArgs);
        }

        void IManager.EmitAll(string eventName, params object[] args)
        {
            foreach (var t in _sockets)
            {
                ((ISocket)t).EmitEvent(eventName, args);
            }
        }


        /// <summary>
        /// 每帧从HTTPManager的OnUpdate函数调用。它的主要功能是发送心跳信息。
        /// </summary>
        void IHeartbeat.OnHeartbeatUpdate(TimeSpan dif)
        {
            switch (State)
            {
                case States.Paused:
                {
                    // 为了确保没有消息丢失，升级包只在现有传输的所有缓冲区被刷新并且传输被视为暂停后才会发送。
                    if (!Transport.IsRequestInProgress &&
                        !Transport.IsPollingInProgress)
                    {
                        State = States.Open;

                        // 关闭电流传输
                        Transport.Close();

                        // 然后切换到新升级的
                        Transport = UpgradingTransport;
                        UpgradingTransport = null;

                        // 我们将发送一个升级包(“5”)。
                        Transport.Send(this.Parser.CreateOutgoing(TransportEventTypes.Upgrade, null));

                        goto case States.Open;
                    }
                }
                    break;

                case States.Opening:
                {
                    if (DateTime.UtcNow - _connectionStarted >= Options.Timeout)
                    {
                        (this as IManager).EmitError("Connection timed out!");
                        (this as IManager).EmitEvent("connect_error");
                        (this as IManager).EmitEvent("connect_timeout");
                        (this as IManager).TryToReconnect();
                    }
                }
                    break;

                case States.Reconnecting:
                {
                    if (_reconnectAt != DateTime.MinValue
                        && DateTime.UtcNow >= _reconnectAt)
                    {
                        (this as IManager).EmitEvent("reconnect_attempt");
                        (this as IManager).EmitEvent("reconnecting");

                        Open();
                    }
                }
                    break;

                case States.Open:
                {
                    ITransport trans = null;

                    // 选择要使用的传输
                    if (Transport is { State: TransportStates.Open })
                    {
                        trans = Transport;
                    }

                    // not yet open?
                    if (trans is not { State: TransportStates.Open })
                    {
                        return;
                    }

                    // 开始轮询服务器以查找事件
                    trans.Poll();

                    // 开始发送未发送的数据包
                    SendOfflinePackets();

                    // 这是我们第一次遇到这种情况。将LastHeartbeat设置为当前时间，因为我们刚刚打开。
                    if (_lastHeartbeat == DateTime.MinValue)
                    {
                        _lastHeartbeat = DateTime.UtcNow;
                        _lastPingReceived = DateTime.UtcNow;
                        return;
                    }

                    if (DateTime.UtcNow - _lastPingReceived >
                        TimeSpan.FromMilliseconds(Handshake.PingInterval + Handshake.PingTimeout))
                        (this as IManager).TryToReconnect();
                }
                    break; // case States.Open:
            }
        }
    }
}

#endif