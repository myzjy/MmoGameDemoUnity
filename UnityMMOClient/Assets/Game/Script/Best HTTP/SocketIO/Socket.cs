#if !BESTHTTP_DISABLE_SOCKETIO

using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO
{
    using BestHTTP;
    using Events;

    /// <summary>
    /// 这个类表示一个套接字。IO命名空间。
    /// </summary>
    public sealed class Socket : ISocket
    {
        /// <summary>
        /// 创建此套接字的SocketManager实例。
        /// </summary>
        public SocketManager Manager { get; private set; }

        /// <summary>
        /// 该套接字所绑定的名称空间。
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// Unique Id of the socket.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Id { get; private set; }

        /// <summary>
        /// 如果套接字已连接并向服务器打开，则为True。否则错误。
        /// </summary>
        private bool IsOpen { get; set; }

        /// <summary>
        /// 当此属性为True时，套接字将使用父SocketManager's Encoder解码数据包的有效负载数据。
        /// 您必须在订阅任何事件之前设置此属性!默认值为True;
        /// </summary>
        private bool AutoDecodePayload { get; set; }

        /// <summary>
        /// 用于存储与给定id关联的确认回调的表。
        /// </summary>
        private Dictionary<int, SocketIOAckCallback> _ackCallbacks;

        /// <summary>
        /// 帮助该类管理事件订阅和分派事件的回调表。
        /// </summary>
        private readonly EventTable _eventCallbacks;

        /// <summary>
        /// 缓存列表以节省一些GC分配。
        /// </summary>
        private readonly List<object> _arguments = new List<object>();

        /// <summary>
        /// 内部构造函数。
        /// </summary>
        internal Socket(string nsp, SocketManager manager)
        {
            this.Namespace = nsp;
            this.Manager = manager;
            this.IsOpen = false;
            this.AutoDecodePayload = true;
            this._eventCallbacks = new EventTable(this);
        }

        /// <summary>
        /// 内部函数开始打开套接字。
        /// </summary>
        void ISocket.Open()
        {
            HttpManager.Logger.Information("Socket", $"Open - Manager.State = {Manager.State}");

            // 运输车已经建立了连接
            if (Manager.State == SocketManager.States.Open)
                OnTransportOpen();
            else if (Manager.Options.AutoConnect && Manager.State == SocketManager.States.Initial)
                Manager.Open();
        }

        /// <summary>
        /// Disconnects this socket/namespace.
        /// </summary>
        private void Disconnect()
        {
            (this as ISocket).Disconnect(true);
        }

        /// <summary>
        /// Disconnects this socket/namespace.
        /// </summary>
        void ISocket.Disconnect(bool remove)
        {
            // 向服务器发送断开连接报文
            if (IsOpen)
            {
                Packet packet = new Packet(
                    transportEvent: TransportEventTypes.Message,
                    packetType: SocketIOEventTypes.Disconnect,
                    nsp: this.Namespace,
                    payload: string.Empty);
                (Manager as IManager).SendPacket(packet);

                // IsOpen必须为false，因为在OnPacket预处理中，数据包将再次调用此函数
                IsOpen = false;
                (this as ISocket).OnPacket(packet);
            }

            if (_ackCallbacks != null)
                _ackCallbacks.Clear();

            if (remove)
            {
                _eventCallbacks.Clear();

                (Manager as IManager).Remove(this);
            }
        }

        public Socket Emit(string eventName, params object[] args)
        {
            return Emit(eventName, null, args);
        }

        private Socket Emit(string eventName, SocketIOAckCallback callback, params object[] args)
        {
            bool blackListed = EventNames.IsBlacklisted(eventName);
            if (blackListed)
            {
                throw new ArgumentException("Blacklisted event: " + eventName);
            }

            _arguments.Clear();
            _arguments.Add(eventName);

            //查找并交换任何二进制数据(byte[])到一个占位符字符串。
            //服务器端这些将被交换回来。
            List<byte[]> attachments = null;
            if (args is { Length: > 0 })
            {
                int idx = 0;
                foreach (var t in args)
                {
                    if (t is byte[] binData)
                    {
                        attachments ??= new List<byte[]>();

                        Dictionary<string, object> placeholderObj = new Dictionary<string, object>(2)
                        {
                            { Packet.Placeholder, true },
                            { "num", idx++ }
                        };

                        _arguments.Add(placeholderObj);

                        attachments.Add(binData);
                    }
                    else
                    {
                        _arguments.Add(t);
                    }
                }
            }

            string payload;

            try
            {
                payload = Manager.Encoder.Encode(_arguments);
            }
            catch (Exception ex)
            {
                (this as ISocket).EmitError(
                    errCode: SocketIOErrors.Internal,
                    msg: $"编码有效负载时出错:{ex.Message}  {ex.StackTrace}");

                return this;
            }

            // 在此函数中不再进一步使用它，因此可以将其清除为不包含任何不需要的引用。
            _arguments.Clear();

            if (payload == null)
                throw new ArgumentException("将参数编码为JSON失败!");

            int id = 0;

            if (callback != null)
            {
                id = Manager.NextAckId;

                _ackCallbacks ??= new Dictionary<int, SocketIOAckCallback>();

                _ackCallbacks[id] = callback;
            }

            Packet packet = new Packet(
                transportEvent: TransportEventTypes.Message,
                packetType: attachments == null ? SocketIOEventTypes.Event : SocketIOEventTypes.BinaryEvent,
                nsp: this.Namespace,
                payload: payload,
                attachment: 0,
                id: id);

            if (attachments != null)
                packet.Attachments = attachments; // This will set the AttachmentCount property too.

            (Manager as IManager).SendPacket(packet);

            return this;
        }

        public Socket EmitAck(Packet originalPacket, params object[] args)
        {
            if (originalPacket == null)
            {
                throw new ArgumentNullException(nameof(originalPacket));
            }

            if ( /*originalPacket.Id == 0 ||*/
                (originalPacket.SocketIOEvent != SocketIOEventTypes.Event &&
                 originalPacket.SocketIOEvent != SocketIOEventTypes.BinaryEvent))
            {
                throw new ArgumentException(
                    "错误的数据包-你不能发送一个Ack的数据包 id == 0 and SocketIOEvent != Event or SocketIOEvent != BinaryEvent!");
            }

            _arguments.Clear();
            if (args is { Length: > 0 })
            {
                _arguments.AddRange(args);
            }

            string payload;
            try
            {
                payload = Manager.Encoder.Encode(_arguments);
            }
            catch (Exception ex)
            {
                (this as ISocket).EmitError(
                    errCode: SocketIOErrors.Internal,
                    msg: $"编码有效负载时出错:{ex.Message}  {ex.StackTrace}");

                return this;
            }

            if (payload == null)
            {
                throw new ArgumentException("将参数编码为JSON失败!");
            }

            Packet packet = new Packet(
                transportEvent: TransportEventTypes.Message,
                packetType: originalPacket.SocketIOEvent == SocketIOEventTypes.Event
                    ? SocketIOEventTypes.Ack
                    : SocketIOEventTypes.BinaryAck,
                nsp: this.Namespace,
                payload: payload,
                attachment: 0,
                id: originalPacket.Id);

            (Manager as IManager).SendPacket(packet);

            return this;
        }

        /// <summary>
        /// 为给定的名称注册一个回调
        /// </summary>
        public void On(string eventName, SocketIOCallback callback)
        {
            _eventCallbacks.Register(eventName, callback, false, this.AutoDecodePayload);
        }

        public void On(SocketIOEventTypes type, SocketIOCallback callback)
        {
            string eventName = EventNames.GetNameFor(type);

            _eventCallbacks.Register(eventName, callback, false, this.AutoDecodePayload);
        }

        public void On(string eventName, SocketIOCallback callback, bool autoDecodePayload)
        {
            _eventCallbacks.Register(eventName, callback, false, autoDecodePayload);
        }

        public void On(SocketIOEventTypes type, SocketIOCallback callback, bool autoDecodePayload)
        {
            var eventName = EventNames.GetNameFor(type);

            _eventCallbacks.Register(eventName, callback, false, autoDecodePayload);
        }

        public void Once(string eventName, SocketIOCallback callback)
        {
            _eventCallbacks.Register(eventName, callback, true, this.AutoDecodePayload);
        }

        public void Once(SocketIOEventTypes type, SocketIOCallback callback)
        {
            _eventCallbacks.Register(EventNames.GetNameFor(type), callback, true, this.AutoDecodePayload);
        }

        public void Once(string eventName, SocketIOCallback callback, bool autoDecodePayload)
        {
            _eventCallbacks.Register(eventName, callback, true, autoDecodePayload);
        }

        public void Once(SocketIOEventTypes type, SocketIOCallback callback, bool autoDecodePayload)
        {
            _eventCallbacks.Register(EventNames.GetNameFor(type), callback, true, autoDecodePayload);
        }


        /// <summary>
        /// 删除所有事件的所有回调。
        /// </summary>
        public void Off()
        {
            _eventCallbacks.Clear();
        }

        /// <summary>
        /// 移除对给定事件的所有回调。
        /// </summary>
        private void Off(string eventName)
        {
            _eventCallbacks.Unregister(eventName);
        }

        /// <summary>
        /// 移除对给定事件的所有回调。
        /// </summary>
        public void Off(SocketIOEventTypes type)
        {
            Off(EventNames.GetNameFor(type));
        }

        /// <summary>
        /// 删除指定的回调。
        /// </summary>
        public void Off(string eventName, SocketIOCallback callback)
        {
            _eventCallbacks.Unregister(eventName, callback);
        }

        /// <summary>
        /// 删除指定的回调。
        /// </summary>
        public void Off(SocketIOEventTypes type, SocketIOCallback callback)
        {
            _eventCallbacks.Unregister(EventNames.GetNameFor(type), callback);
        }


        /// <summary>
        /// OnPacket链的最后一次调用(Transport -> Manager -> Socket),如果有任何回调，我们将分派事件
        /// </summary>
        void ISocket.OnPacket(Packet packet)
        {
            // Some preprocessing of the packet
            switch (packet.SocketIOEvent)
            {
                case SocketIOEventTypes.Connect:
                {
                    if (this.Manager.Options.ServerVersion != SupportedSocketIOVersions.v3)
                    {
                        this.Id = this.Namespace != "/"
                            ? $"{this.Namespace}#{this.Manager.Handshake.Sid}"
                            : this.Manager.Handshake.Sid;
                    }
                    else
                    {
                        if (JSON.Json.Decode(packet.Payload) is Dictionary<string, object> data)
                        {
                            this.Id = data["sid"].ToString();
                        }
                    }

                    this.IsOpen = true;
                }
                    break;

                case SocketIOEventTypes.Disconnect:
                {
                    if (IsOpen)
                    {
                        IsOpen = false;
                        _eventCallbacks.Call(EventNames.GetNameFor(SocketIOEventTypes.Disconnect), packet);
                        Disconnect();
                    }
                }
                    break;

                // 从服务器发送的json字符串创建一个Error对象
                case SocketIOEventTypes.Error:
                {
                    bool success = false;
                    object result = JSON.Json.Decode(packet.Payload, ref success);
                    if (success)
                    {
                        var errDict = result as Dictionary<string, object>;
                        Error err = null;

                        if (errDict != null)
                        {
                            string code = null;
                            if (errDict.TryGetValue("code", out var tmpObject))
                                code = tmpObject.ToString();

                            if (code != null &&
                                int.TryParse(code, out var errorCode) &&
                                errorCode is >= 0 and <= 7)
                            {
                                errDict.TryGetValue("message", out tmpObject);
                                err = new Error(
                                    code: (SocketIOErrors)errorCode,
                                    msg: tmpObject != null ? tmpObject.ToString() : string.Empty);
                            }
                        }

                        err ??= new Error(SocketIOErrors.Custom, packet.Payload);

                        _eventCallbacks.Call(EventNames.GetNameFor(SocketIOEventTypes.Error), packet, err);

                        return;
                    }
                }
                    break;
            }

            // 将事件分派给所有订阅者
            _eventCallbacks.Call(packet);

            // call Ack callbacks
            if (packet.SocketIOEvent is not (SocketIOEventTypes.Ack or SocketIOEventTypes.BinaryAck)
                || _ackCallbacks == null) return;
            if (_ackCallbacks.TryGetValue(packet.Id, out var ackCallback) &&
                ackCallback != null)
            {
                try
                {
                    ackCallback(this, packet, this.AutoDecodePayload ? packet.Decode(Manager.Encoder) : null);
                }
                catch (Exception ex)
                {
                    HttpManager.Logger.Exception("Socket", "ackCallback", ex);
                }
            }

            _ackCallbacks.Remove(packet.Id);
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
                _eventCallbacks.Call(eventName, null, args);
            }
        }

        void ISocket.EmitError(SocketIOErrors errCode, string msg)
        {
            (this as ISocket).EmitEvent(
                type: SocketIOEventTypes.Error,
                args: new Error(errCode, msg));
        }

        /// <summary>
        /// 在连接底层传输时调用
        /// </summary>
        internal void OnTransportOpen()
        {
            HttpManager.Logger.Information("Socket", "OnTransportOpen - IsOpen: " + this.IsOpen);

            if (this.IsOpen)
            {
                return;
            }

            if (this.Namespace != "/" ||
                this.Manager.Options.ServerVersion == SupportedSocketIOVersions.v3)
            {
                try
                {
                    string authData = null;
                    if (this.Manager.Options.ServerVersion == SupportedSocketIOVersions.v3)
                    {
                        authData = this.Manager.Options.Auth != null
                            ? this.Manager.Options.Auth(this.Manager, this)
                            : "{}";
                    }

                    var packetData = new Packet(
                        transportEvent: TransportEventTypes.Message,
                        packetType: SocketIOEventTypes.Connect,
                        nsp: this.Namespace,
                        payload: authData);
                    (Manager as IManager).SendPacket(packetData);
                }
                catch (Exception ex)
                {
                    HttpManager.Logger.Exception("Socket", "OnTransportOpen", ex);
                }
            }
            else
            {
                this.IsOpen = true;
            }
        }
    }
}

#endif