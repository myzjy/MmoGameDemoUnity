#if !BESTHTTP_DISABLE_SOCKETIO

using System;

namespace BestHTTP.SocketIO3
{
    using BestHTTP;
    using Logger;
    using Events;

    public delegate void SocketIOCallback(Socket socket, IncomingPacket packet, params object[] args);

    public delegate void SocketIOAckCallback(Socket socket, IncomingPacket packet, params object[] args);

    public struct EmitBuilder
    {
        private readonly Socket _socket;
        internal bool IsVolatile;
        private int _id;

        internal EmitBuilder(Socket s)
        {
            this._socket = s;
            this.IsVolatile = false;
            this._id = -1;
        }

        public EmitBuilder ExpectAcknowledgement(Action callback)
        {
            this._id = this._socket.Manager.NextAckId;
            string name = IncomingPacket.GenerateAcknowledgementNameFromId(this._id);

            this._socket.TypedEventTable.Register(name, null, _ => callback(), true);
            return this;
        }

        public EmitBuilder ExpectAcknowledgement<T>(Action<T> callback)
        {
            this._id = this._socket.Manager.NextAckId;
            string name = IncomingPacket.GenerateAcknowledgementNameFromId(this._id);

            this._socket.TypedEventTable.Register(name, new[] { typeof(T) }, (args) => callback((T)args[0]), true);

            return this;
        }

        public EmitBuilder Volatile()
        {
            this.IsVolatile = true;
            return this;
        }

        public Socket Emit(string eventName, params object[] args)
        {
            bool blackListed = EventNames.IsBlacklisted(eventName);
            if (blackListed)
                throw new ArgumentException("Blacklisted event: " + eventName);

            var packet = this._socket.Manager.Parser.CreateOutgoing(this._socket, SocketIOEventTypes.Event, this._id,
                eventName, args);
            packet.IsVolatile = this.IsVolatile;
            (this._socket.Manager as IManager).SendPacket(packet);

            return this._socket;
        }
    }

    /// <summary>
    /// This class represents a Socket.IO namespace.
    /// </summary>
    public sealed class Socket : ISocket
    {
        #region Public Properties

        /// <summary>
        /// The SocketManager instance that created this socket.
        /// </summary>
        public SocketManager Manager { get; private set; }

        /// <summary>
        /// The namespace that this socket is bound to.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// Unique Id of the socket.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Id { get; private set; }

        /// <summary>
        /// True if the socket is connected and open to the server. False otherwise.
        /// </summary>
        private bool IsOpen { get; set; }

        public IncomingPacket CurrentPacket => this._currentPacket;

        public LoggingContext Context { get; private set; }

        #endregion

        internal readonly TypedEventTable TypedEventTable;
        private IncomingPacket _currentPacket = IncomingPacket.Empty;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal Socket(string nsp, SocketManager manager)
        {
            this.Context = new LoggingContext(this);
            this.Context.Add("nsp", nsp);

            this.Namespace = nsp;
            this.Manager = manager;
            this.IsOpen = false;
            this.TypedEventTable = new TypedEventTable(this);

            this.On<ConnectResponse>(EventNames.GetNameFor(SocketIOEventTypes.Connect), OnConnected);
        }

        private void OnConnected(ConnectResponse resp)
        {
            this.Id = resp.Sid;
            this.IsOpen = true;
        }


        /// <summary>
        /// 内部函数开始打开套接字。
        /// </summary>
        void ISocket.Open()
        {
            HttpManager.Logger.Information("Socket", $"Open - Manager.State = {Manager.State}", this.Context);

            // The transport already established the connection
            // 服务器已经建立了连接
            if (Manager.State == SocketManager.States.Open)
            {
                OnTransportOpen();
            }
            else if (Manager.Options.AutoConnect &&
                     Manager.State == SocketManager.States.Initial)
            {
                Manager.Open();
            }
        }

        /// <summary>
        /// 断开该套接字/命名空间。
        /// </summary>
        private void Disconnect()
        {
            (this as ISocket).Disconnect(true);
        }

        /// <summary>
        /// 断开连接 this socket/namespace.
        /// </summary>
        void ISocket.Disconnect(bool remove)
        {
            // 向服务器发送断开连接报文
            if (IsOpen)
            {
                var packet = this.Manager.Parser.CreateOutgoing(this, SocketIOEventTypes.Disconnect, -1, null, null);
                (Manager as IManager).SendPacket(packet);

                // IsOpen必须为false，因为在OnPacket预处理中，数据包将再次调用此函数
                IsOpen = false;
                var inComingPacket = new IncomingPacket(
                    transportEvent: TransportEventTypes.Message,
                    packetType: SocketIOEventTypes.Disconnect,
                    nsp: this.Namespace,
                    id: -1);
                (this as ISocket).OnPacket(inComingPacket);
            }

            if (!remove) return;
            this.TypedEventTable.Clear();

            (Manager as IManager).Remove(this);
        }

        /// <summary>
        /// 通过释放volatile事件，如果传输还没有准备好，则该事件将被丢弃。
        /// </summary>
        public EmitBuilder Volatile()
        {
            return new EmitBuilder(this) { IsVolatile = true };
        }

        public EmitBuilder ExpectAcknowledgement(Action callback)
        {
            return new EmitBuilder(this).ExpectAcknowledgement(callback);
        }

        public EmitBuilder ExpectAcknowledgement<T>(Action<T> callback)
        {
            return new EmitBuilder(this).ExpectAcknowledgement(callback);
        }

        public Socket Emit(string eventName, params object[] args)
        {
            return new EmitBuilder(this).Emit(eventName, args);
        }

        public Socket EmitAck(params object[] args)
        {
            return EmitAck(this._currentPacket, args);
        }

        private Socket EmitAck(IncomingPacket packet, params object[] args)
        {
            if (packet.Equals(IncomingPacket.Empty))
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (packet.Id < 0 || (packet.SocketIOEvent != SocketIOEventTypes.Event &&
                                  packet.SocketIOEvent != SocketIOEventTypes.BinaryEvent))
            {
                throw new ArgumentException(
                    "错误的数据包-你不能发送一个带有id &lt的数据包的Ack;0或SocketIOEvent != Event或SocketIOEvent != BinaryEvent!");
            }

            var eventType = packet.SocketIOEvent == SocketIOEventTypes.Event
                ? SocketIOEventTypes.Ack
                : SocketIOEventTypes.BinaryAck;

            var createOutGoing = this.Manager.Parser.CreateOutgoing(
                socket: this,
                socketIOEvent: eventType,
                id: packet.Id,
                name: null,
                args: args);
            (Manager as IManager).SendPacket(createOutGoing);

            return this;
        }


        public void On(SocketIOEventTypes eventType, Action callback)
        {
            string eventName = EventNames.GetNameFor(eventType);
            this.TypedEventTable.Register(
                methodName: eventName,
                paramTypes: null,
                callback: _ => callback());
        }

        public void On<T>(SocketIOEventTypes eventType, Action<T> callback)
        {
            string eventName = EventNames.GetNameFor(eventType);
            var paramTypes = new[]
            {
                typeof(T),
            };

            void CallBack(object[] args)
            {
                T arg1;
                try
                {
                    arg1 = (T)args[0];
                }
                catch (Exception ex)
                {
                    HttpManager.Logger.Exception("Socket",
                        $"On<{typeof(T).Name}>('{eventName}') - cast failed",
                        ex, this.Context);
                    return;
                }

                callback(arg1);
            }

            this.TypedEventTable.Register(
                eventName,
                paramTypes,
                CallBack);
        }

        public void On(string eventName, Action callback)
        {
            this.TypedEventTable.Register(eventName, null, _ => callback());
        }

        private void On<T>(string eventName, Action<T> callback)
        {
            var paramTypes = new[]
            {
                typeof(T),
            };

            void CallBack(object[] args)
            {
                T arg1;
                try
                {
                    arg1 = (T)args[0];
                }
                catch (Exception ex)
                {
                    HttpManager.Logger.Exception("Socket",
                        $"On<{typeof(T).Name}>('{eventName}') - cast failed",
                        ex, this.Context);
                    return;
                }

                callback(arg1);
            }

            this.TypedEventTable.Register(eventName,
                paramTypes, CallBack);
        }

        public void On<T1, T2>(string eventName, Action<T1, T2> callback)
        {
            var paramTypes = new[]
            {
                typeof(T1),
                typeof(T2),
            };

            void CallBack(object[] args)
            {
                T1 arg1;
                T2 arg2;
                try
                {
                    arg1 = (T1)args[0];
                    arg2 = (T2)args[1];
                }
                catch (Exception ex)
                {
                    HttpManager.Logger.Exception("Socket",
                        $"On<{typeof(T1).Name}, {typeof(T2).Name}>('{eventName}') - cast failed",
                        ex, this.Context);
                    return;
                }

                callback(arg1, arg2);
            }

            this.TypedEventTable.Register(eventName,
                paramTypes, CallBack);
        }

        public void On<T1, T2, T3>(string eventName, Action<T1, T2, T3> callback)
        {
            var paramTypes = new[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
            };

            void CallBack(object[] args)
            {
                T1 arg1;
                T2 arg2;
                T3 arg3;
                try
                {
                    arg1 = (T1)args[0];
                    arg2 = (T2)args[1];
                    arg3 = (T3)args[2];
                }
                catch (Exception ex)
                {
                    HttpManager.Logger.Exception("Socket",
                        $"On<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}>('{eventName}') - cast failed",
                        ex, this.Context);
                    return;
                }

                callback(arg1, arg2, arg3);
            }

            this.TypedEventTable.Register(eventName,
                paramTypes, CallBack);
        }

        public void On<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> callback)
        {
            var paramTypes = new[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
            };

            void CallBack(object[] args)
            {
                T1 arg1;
                T2 arg2;
                T3 arg3;
                T4 arg4;
                try
                {
                    arg1 = (T1)args[0];
                    arg2 = (T2)args[1];
                    arg3 = (T3)args[2];
                    arg4 = (T4)args[3];
                }
                catch (Exception ex)
                {
                    HttpManager.Logger.Exception("Socket",
                        $"On<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}>('{eventName}') - cast failed",
                        ex, this.Context);
                    return;
                }

                callback(arg1, arg2, arg3, arg4);
            }

            this.TypedEventTable.Register(eventName,
                paramTypes, CallBack);
        }

        public void On<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> callback)
        {
            var paramTypes = new[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5)
            };

            void CallBack(object[] args)
            {
                T1 arg1;
                T2 arg2;
                T3 arg3;
                T4 arg4;
                T5 arg5;
                try
                {
                    arg1 = (T1)args[0];
                    arg2 = (T2)args[1];
                    arg3 = (T3)args[2];
                    arg4 = (T4)args[3];
                    arg5 = (T5)args[4];
                }
                catch (Exception ex)
                {
                    HttpManager.Logger.Exception("Socket",
                        $"On<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}>('{eventName}') - cast failed",
                        ex, this.Context);
                    return;
                }

                callback(arg1, arg2, arg3, arg4, arg5);
            }

            this.TypedEventTable.Register(eventName,
                paramTypes, CallBack);
        }

        public void Once(string eventName, Action callback)
        {
            void CallBack(object[] args)
            {
                callback();
            }

            this.TypedEventTable.Register(
                methodName: eventName,
                paramTypes: null,
                callback: CallBack,
                once: true);
        }

        public void Once<T>(string eventName, Action<T> callback)
        {
            var paramTypes = new[]
            {
                typeof(T),
            };

            void CallBack(object[] args)
            {
                callback(
                    obj: (T)args[0]);
            }

            this.TypedEventTable.Register(
                methodName: eventName,
                paramTypes: paramTypes,
                callback: CallBack,
                once: true);
        }

        public void Once<T1, T2>(string eventName, Action<T1, T2> callback)
        {
            var paramTypes = new[]
            {
                typeof(T1),
                typeof(T2),
            };

            void CallBack(object[] args)
            {
                callback(
                    arg1: (T1)args[0],
                    arg2: (T2)args[1]);
            }

            this.TypedEventTable.Register(
                methodName: eventName,
                paramTypes: paramTypes,
                callback: CallBack,
                once: true);
        }

        public void Once<T1, T2, T3>(string eventName, Action<T1, T2, T3> callback)
        {
            var paramTypes = new[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
            };

            void CallBack(object[] args)
            {
                callback(
                    arg1: (T1)args[0],
                    arg2: (T2)args[1],
                    arg3: (T3)args[2]);
            }

            this.TypedEventTable.Register(
                methodName: eventName,
                paramTypes: paramTypes,
                callback: CallBack,
                once: true);
        }

        public void Once<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> callback)
        {
            var paramTypes = new[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
            };

            void CallBack(object[] args)
            {
                callback(
                    arg1: (T1)args[0],
                    arg2: (T2)args[1],
                    arg3: (T3)args[2],
                    arg4: (T4)args[3]);
            }

            this.TypedEventTable.Register(
                methodName: eventName,
                paramTypes: paramTypes,
                callback: CallBack,
                once: true);
        }

        public void Once<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> callback)
        {
            var paramTypes = new[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5)
            };

            void CallBack(object[] args)
            {
                callback(
                    arg1: (T1)args[0],
                    arg2: (T2)args[1],
                    arg3: (T3)args[2],
                    arg4: (T4)args[3],
                    arg5: (T5)args[4]);
            }

            this.TypedEventTable.Register(
                methodName: eventName,
                paramTypes: paramTypes,
                callback: CallBack,
                once: true);
        }

        /// <summary>
        /// 移除对给定事件的所有回调。
        /// </summary>
        public void Off()
        {
            this.TypedEventTable.Clear();
        }

        /// <summary>
        /// 移除对给定事件的所有回调。
        /// </summary>
        private void Off(string eventName)
        {
            this.TypedEventTable.Unregister(eventName);
        }

        /// <summary>
        /// 移除对给定事件的所有回调。
        /// </summary>
        public void Off(SocketIOEventTypes type)
        {
            var nameFor = EventNames.GetNameFor(type: type);
            Off(nameFor);
        }


        /// <summary>
        /// OnPacket链的最后一次调用(Transport ->Manager→Socket)，如果有任何回调，我们将分派事件
        /// </summary>
        void ISocket.OnPacket(IncomingPacket packet)
        {
            // Some preprocessing of the packet
            switch (packet.SocketIOEvent)
            {
                case SocketIOEventTypes.Connect:
                    break;

                case SocketIOEventTypes.Disconnect:
                {
                    if (IsOpen)
                    {
                        IsOpen = false;
                        this.TypedEventTable.Call(packet);
                        Disconnect();
                    }
                }
                    break;
                case SocketIOEventTypes.Unknown:
                    break;
                case SocketIOEventTypes.Event:
                    break;
                case SocketIOEventTypes.Ack:
                    break;
                case SocketIOEventTypes.Error:
                    break;
                case SocketIOEventTypes.BinaryEvent:
                    break;
                case SocketIOEventTypes.BinaryAck:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            try
            {
                this._currentPacket = packet;

                // Dispatch the event to all subscriber
                this.TypedEventTable.Call(packet);
            }
            finally
            {
                this._currentPacket = IncomingPacket.Empty;
            }
        }


        public Subscription GetSubscription(string name)
        {
            return this.TypedEventTable.GetSubscription(name);
        }

        /// <summary>
        /// 向用户级发出内部无包事件。
        /// </summary>
        void ISocket.EmitEvent(SocketIOEventTypes type, params object[] args)
        {
            (this as ISocket).EmitEvent(EventNames.GetNameFor(type), args);
        }

        /// <summary>
        /// 向用户级发出内部无包事件。
        /// </summary>
        void ISocket.EmitEvent(string eventName, params object[] args)
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                this.TypedEventTable.Call(eventName, args);
            }
        }

        void ISocket.EmitError(string msg)
        {
            // ReSharper disable once IdentifierTypo
            var outcoming =
                this.Manager.Parser.CreateOutgoing(this, SocketIOEventTypes.Error, -1, null, new Error(msg));
            var packet = outcoming.IsBinary
                ? this.Manager.Parser.Parse(this.Manager, outcoming.PayloadData)
                : this.Manager.Parser.Parse(this.Manager, outcoming.Payload);

            (this as ISocket).EmitEvent(SocketIOEventTypes.Error, packet.DecodedArg ?? packet.DecodedArgs);
        }


        /// <summary>
        /// 在连接底层传输时调用
        /// </summary>
        internal void OnTransportOpen()
        {
            HttpManager.Logger.Information("Socket", "OnTransportOpen - IsOpen: " + this.IsOpen, this.Context);

            if (this.IsOpen)
            {
                return;
            }

            object authData = null;
            try
            {
                authData = this.Manager.Options.Auth?.Invoke(this.Manager, this);
            }
            catch (Exception ex)
            {
                HttpManager.Logger.Exception("Socket", "OnTransportOpen - Options.Auth", ex, this.Context);
            }

            try
            {
                (Manager as IManager).SendPacket(
                    this.Manager.Parser.CreateOutgoing(this, SocketIOEventTypes.Connect, -1, null, authData));
            }
            catch (Exception ex)
            {
                HttpManager.Logger.Exception("Socket", "OnTransportOpen", ex, this.Context);
            }
        }
    }
}

#endif