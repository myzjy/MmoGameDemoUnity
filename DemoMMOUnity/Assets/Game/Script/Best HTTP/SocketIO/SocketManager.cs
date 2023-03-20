#if !BESTHTTP_DISABLE_SOCKETIO

using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.SocketIO.Transports;
using BestHTTP.Extensions;
using BestHTTP.SocketIO.JsonEncoders;
using BestHTTP.SocketIO.Events;

namespace BestHTTP.SocketIO
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
        /// 用于对事件参数进行编码/解码的默认Json编码/解码器。
        /// </summary>
        private static readonly IJsonEncoder DefaultEncoder = new DefaultJSonEncoder();

        /// <summary>
        /// 支持的套接字。IO协议版本
        /// </summary>
        public int ProtocolVersion => this.Options.ServerVersion == SupportedSocketIOVersions.v3 ? 4 : 3;

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
        /// JSon编码器，用于将发送的数据编码为JSon，并将接收到的JSon解码为对象列表。
        /// </summary>
        public IJsonEncoder Encoder { get; set; }

        /// <summary>
        /// 时间戳支持基于请求的传输。
        /// </summary>
        internal UInt64 Timestamp
        {
            get
            {
                var dateTime = new DateTime(1970, 1, 1);
                var utcNow = DateTime.UtcNow.Subtract(dateTime);
                return (UInt64)(utcNow).TotalMilliseconds;
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
        private List<Packet> _offlinePackets;

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
        /// 连接是否在等待ping响应。
        /// </summary>
        private bool _isWaitingPong;

        /// <summary>
        /// 在引擎。IO v4 / socket。IO v3服务器发送ping消息，而不是客户端。
        /// </summary>
        private DateTime _lastPingReceived;


        /// <summary>
        ///构造函数来创建一个SocketManager实例，该实例将连接到给定的uri。
        /// </summary>
        public SocketManager(Uri uri)
            : this(uri, new SocketOptions())
        {
        }

        /// <summary>
        /// 构造函数来创建SocketManager实例。
        /// </summary>
        private SocketManager(Uri uri, SocketOptions options)
        {
            Uri = uri;
            Options = options ?? new SocketOptions();
            State = States.Initial;
            PreviousState = States.Initial;
            Encoder = SocketManager.DefaultEncoder;
        }


        /// <summary>
        /// Returns with the specified namespace
        /// </summary>
        private Socket GetSocket(string nsp = "/")
        {
            if (string.IsNullOrEmpty(nsp))
                throw new ArgumentNullException(nameof(nsp));

            /*if (nsp[0] != '/')
                nsp = "/" + nsp;*/

            if (_namespaces.TryGetValue(nsp, out var socket)) return socket;
            // No socket found, create one
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

            HttpManager.Logger.Information("SocketManager", "Opening");

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
            }

            Transport.Open();


            (this as IManager).EmitEvent("connecting");

            State = States.Opening;

            _connectionStarted = DateTime.UtcNow;

            HttpManager.Heartbeats.Subscribe(this);

            // The root namespace will be opened by default
            GetSocket();
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

            HttpManager.Logger.Information("SocketManager", "Closing");

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
            _isWaitingPong = false;
            _lastPingReceived = DateTime.MinValue;

            if (removeSockets && _offlinePackets != null)
                _offlinePackets.Clear();

            // 也要从字典中删除引用。
            if (removeSockets)
                _namespaces.Clear();

            Handshake = null;

            if (Transport != null)
                Transport.Close();
            Transport = null;

            if (UpgradingTransport != null)
                UpgradingTransport.Close();
            UpgradingTransport = null;

            _closing = false;
        }

        /// <summary>
        /// 在发生错误时从ITransport实现调用，我们可能不得不尝试重新连接。
        /// </summary>
        void IManager.TryToReconnect()
        {
            if (State == States.Reconnecting ||
                State == States.Closed)
                return;

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

            // In the Close() function we unregistered
            HttpManager.Heartbeats.Subscribe(this);

            HttpManager.Logger.Information("SocketManager", "Reconnecting");
        }

        /// <summary>
        /// 在连接到服务器时由传输程序调用。
        /// </summary>
        bool IManager.OnTransportConnected(ITransport trans)
        {
            HttpManager.Logger.Information("SocketManager",
                $"OnTransportConnected State: {this.State}, PreviousState: {this.PreviousState}, Current Transport: {trans.Type}, Upgrading Transport: {(UpgradingTransport != null ? UpgradingTransport.Type.ToString() : "null")}");

            if (State != States.Opening)
                return false;

            if (PreviousState == States.Reconnecting)
                (this as IManager).EmitEvent("reconnect");

            State = States.Open;

            if (PreviousState == States.Reconnecting)
                (this as IManager).EmitEvent("reconnect_before_offline_packets");

            foreach (var socket in _sockets.Where(socket => socket != null))
            {
                socket.OnTransportOpen();
            }

            ReconnectAttempts = 0;

            // 发送我们在没有可用的运输工具时收集的数据包。
            SendOfflinePackets();

#if !BESTHTTP_DISABLE_WEBSOCKET
            // Can we upgrade to WebSocket transport?
            if (Transport.Type != TransportTypes.WebSocket &&
                Handshake.Upgrades.Contains("websocket"))
            {
                UpgradingTransport = new WebSocketTransport(this);
                UpgradingTransport.Open();
            }
#endif

            return true;
        }

        void IManager.OnTransportError(ITransport trans, string err)
        {
            (this as IManager).EmitError(SocketIOErrors.Internal, err);

            trans.Close();
            (this as IManager).TryToReconnect();
        }

        void IManager.OnTransportProbed(ITransport trans)
        {
            HttpManager.Logger.Information("SocketManager", "\"probe\" packet received");

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
                return null;

            return Transport.IsRequestInProgress ? null : Transport;
        }

        /// <summary>
        /// 将选择最佳传输并发送OfflinePackets列表中的所有数据包。
        /// </summary>
        private void SendOfflinePackets()
        {
            ITransport trans = SelectTransport();

            //发送我们没有发送的数据包，因为没有可用的传输。
            //该函数在事件处理程序获得'connected'事件之前被调用，因此
            //理论上数据包顺序是保留的。
            if (_offlinePackets is { Count: > 0 } && trans != null)
            {
                trans.Send(_offlinePackets);
                _offlinePackets.Clear();
            }
        }

        /// <summary>
        /// 从Socket类调用的内部函数它会立即发送数据包，或者如果没有可用的传输，它会将数据包存储在OfflinePackets列表中。
        /// </summary>
        void IManager.SendPacket(Packet packet)
        {
            HttpManager.Logger.Information("SocketManager", "SendPacket " + packet);

            ITransport trans = SelectTransport();

            if (trans != null)
            {
                try
                {
                    trans.Send(packet);
                }
                catch (Exception ex)
                {
                    (this as IManager).EmitError(SocketIOErrors.Internal, ex.Message + " " + ex.StackTrace);
                }
            }
            else
            {
                HttpManager.Logger.Information("SocketManager", "SendPacket - Offline stashing packet");

                _offlinePackets ??= new List<Packet>();

                // The same packet can be sent through multiple Sockets.
                _offlinePackets.Add(packet.Clone());
            }
        }

        /// <summary>
        /// 从当前运行的传输中调用。将传递给必须调用回调函数的Socket。
        /// </summary>
        void IManager.OnPacket(Packet packet)
        {
            if (State == States.Closed)
            {
                HttpManager.Logger.Information("SocketManager", "OnPacket - State == States.Closed");
                return;
            }

            switch (packet.TransportEvent)
            {
                case TransportEventTypes.Open:
                    if (Handshake == null)
                    {
                        Handshake = new HandshakeData();
                        if (!Handshake.Parse(packet.Payload))
                            HttpManager.Logger.Warning("SocketManager",
                                "Expected handshake data, but wasn't able to parse. Payload: " + packet.Payload);

                        (this as IManager).OnTransportConnected(Transport);

                        return;
                    }
                    else
                        HttpManager.Logger.Information("SocketManager", "OnPacket - Already received handshake data!");

                    break;

                case TransportEventTypes.Ping:
                    if (this.Options.ServerVersion == SupportedSocketIOVersions.Unknown)
                    {
                        HttpManager.Logger.Information("SocketManager",
                            "Received Ping packet from server, setting ServerVersion to v3!");
                        this.Options.ServerVersion = SupportedSocketIOVersions.v3;
                    }

                    _lastPingReceived = DateTime.UtcNow;
                    (this as IManager).SendPacket(new Packet(TransportEventTypes.Pong, SocketIOEventTypes.Unknown, "/",
                        string.Empty));
                    break;

                case TransportEventTypes.Pong:
                    _isWaitingPong = false;
                    break;
            }

            if (_namespaces.TryGetValue(packet.Namespace, out var socket))
                (socket as ISocket).OnPacket(packet);
            else
                HttpManager.Logger.Warning("SocketManager", "Namespace \"" + packet.Namespace + "\" not found!");
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

        void IManager.EmitError(SocketIOErrors errCode, string msg)
        {
            (this as IManager).EmitEvent(SocketIOEventTypes.Error, new Error(errCode, msg));
        }

        void IManager.EmitAll(string eventName, params object[] args)
        {
            foreach (var t in _sockets)
                ((ISocket)t).EmitEvent(eventName, args);
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
                        Transport.Send(new Packet(TransportEventTypes.Upgrade, SocketIOEventTypes.Unknown, "/",
                            string.Empty));

                        goto case States.Open;
                    }
                }
                    break;

                case States.Opening:
                {
                    if (DateTime.UtcNow - _connectionStarted >= Options.Timeout)
                    {
                        (this as IManager).EmitError(SocketIOErrors.Internal, "Connection timed out!");
                        (this as IManager).EmitEvent("connect_error");
                        (this as IManager).EmitEvent("connect_timeout");
                        (this as IManager).TryToReconnect();
                    }
                }

                    break;

                case States.Reconnecting:
                {
                    if (_reconnectAt != DateTime.MinValue && DateTime.UtcNow >= _reconnectAt)
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

                    //开始轮询服务器以查找事件
                    trans.Poll();

                    // 开始发送未发送的数据包
                    SendOfflinePackets();

                    // 这是我们第一次遇到这种情况。将LastHeartbeat设置为当前时间，因为我们刚刚打开。
                    if (_lastHeartbeat == DateTime.MinValue)
                    {
                        _lastHeartbeat = DateTime.UtcNow;
                        _lastPingReceived = DateTime.UtcNow;
                        if (this.Options.ServerVersion == SupportedSocketIOVersions.Unknown)
                        {
                            (this as IManager).SendPacket(new Packet(TransportEventTypes.Ping,
                                SocketIOEventTypes.Unknown, "/", string.Empty));
                            _isWaitingPong = true;
                        }

                        return;
                    }

                    switch (this.Options.ServerVersion)
                    {
                        case SupportedSocketIOVersions.v2:
                        {
                            // 是时候向服务器发送一个ping事件了
                            if (!_isWaitingPong && DateTime.UtcNow - _lastHeartbeat > Handshake.PingInterval)
                            {
                                (this as IManager).SendPacket(new Packet(
                                    TransportEventTypes.Ping,
                                    SocketIOEventTypes.Unknown, 
                                    "/", 
                                    string.Empty));

                                _lastHeartbeat = DateTime.UtcNow;
                                _isWaitingPong = true;
                            }

                            // 在给定的时间内没有收到pong事件，我们被断开。
                            if (_isWaitingPong && DateTime.UtcNow - _lastHeartbeat > Handshake.PingTimeout)
                            {
                                _isWaitingPong = false;
                                (this as IManager).TryToReconnect();
                            }
                        }

                            break;

                        case SupportedSocketIOVersions.v3:
                        {
                            if (DateTime.UtcNow - _lastPingReceived > Handshake.PingInterval + Handshake.PingTimeout)
                            {
                                (this as IManager).TryToReconnect();
                            }
                        }

                            break;

                        case SupportedSocketIOVersions.Unknown:
                        {
                            var diff = DateTime.UtcNow - _lastHeartbeat;
                            if (diff > Handshake.PingTimeout)
                            {
                                this.Options.ServerVersion = _isWaitingPong
                                    ? SupportedSocketIOVersions.v3
                                    : SupportedSocketIOVersions.v2;
                            }
                        }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                    break; // case States.Open:
            }
        }

    }
}

#endif