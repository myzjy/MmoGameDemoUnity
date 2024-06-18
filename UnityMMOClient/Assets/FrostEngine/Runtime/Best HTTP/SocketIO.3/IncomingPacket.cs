#if !BESTHTTP_DISABLE_SOCKETIO

namespace BestHTTP.SocketIO3
{
    using System.Collections.Generic;
    using PlatformSupport.Memory;
    using Events;

    public struct OutgoingPacket
    {
        public bool IsBinary => string.IsNullOrEmpty(this.Payload);

        public string Payload { get; set; }

        // ReSharper disable once IdentifierTypo
        public List<byte[]> Attachements { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public BufferSegment PayloadData { get; set; }

        public bool IsVolatile { get; set; }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(this.Payload) ? this.Payload : this.PayloadData.ToString();
        }
    }

    public struct IncomingPacket
    {
        public static readonly IncomingPacket Empty =
            new IncomingPacket(
                transportEvent: TransportEventTypes.Unknown,
                packetType: SocketIOEventTypes.Unknown,
                nsp: null,
                id: -1);

        /// <summary>
        /// 传输层报文的事件类型。
        /// </summary>
        public TransportEventTypes TransportEvent { get; private set; }

        /// <summary>
        /// 数据包在Socket中的类型。IO协议。
        /// </summary>
        public SocketIOEventTypes SocketIOEvent { get; private set; }

        /// <summary>
        /// 报文内部的ack-id。
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 广播公司的姓名
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// 当前包后期望的二进制数据计数。
        /// </summary>
        // ReSharper disable once IdentifierTypo
        public int AttachementCount { get; set; }

        /// <summary>
        /// list of binary data received.
        /// </summary>
        // ReSharper disable once IdentifierTypo
        public List<BufferSegment> Attachements { get; set; }

        /// <summary>
        /// 从有效负载字符串解码的事件名称。
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 解析器解码的参数。
        /// </summary>
        public object[] DecodedArgs { get; set; }

        public object DecodedArg { get; set; }

        public IncomingPacket(TransportEventTypes transportEvent, SocketIOEventTypes packetType, string nsp, int id)
        {
            this.TransportEvent = transportEvent;
            this.SocketIOEvent = packetType;
            this.Namespace = nsp;
            this.Id = id;

            this.AttachementCount = 0;
            this.Attachements = null;

            this.EventName = this.SocketIOEvent != SocketIOEventTypes.Unknown
                ? EventNames.GetNameFor(this.SocketIOEvent)
                : EventNames.GetNameFor(this.TransportEvent);

            this.DecodedArg = this.DecodedArgs = null;
        }

        /// <summary>
        /// 返回此包的有效负载。
        /// </summary>
        public override string ToString()
        {
            return $"[Packet {this.TransportEvent}{this.SocketIOEvent}/{this.Namespace},{this.Id}[{this.EventName}]]";
        }

        public override bool Equals(object obj)
        {
            return obj is IncomingPacket packet && Equals(packet);
        }

        public bool Equals(IncomingPacket packet)
        {
            return this.TransportEvent == packet.TransportEvent &&
                   this.SocketIOEvent == packet.SocketIOEvent &&
                   this.Id == packet.Id &&
                   this.Namespace == packet.Namespace &&
                   this.EventName == packet.EventName &&
                   this.DecodedArg == packet.DecodedArg &&
                   this.DecodedArgs == packet.DecodedArgs;
        }

        public override int GetHashCode()
        {
            int hashCode = -1860921451;
            hashCode = hashCode * -1521134295 + TransportEvent.GetHashCode();
            hashCode = hashCode * -1521134295 + SocketIOEvent.GetHashCode();
            hashCode = hashCode * -1521134295 + Id.GetHashCode();

            if (Namespace != null)
            {
                hashCode = hashCode * -1521134295 + Namespace.GetHashCode();
            }

            if (EventName != null)
            {
                hashCode = hashCode * -1521134295 + EventName.GetHashCode();
            }

            if (DecodedArgs != null)
            {
                hashCode = hashCode * -1521134295 + DecodedArgs.GetHashCode();
            }

            if (DecodedArg != null)
            {
                hashCode = hashCode * -1521134295 + DecodedArg.GetHashCode();
            }

            return hashCode;
        }

        public static string GenerateAcknowledgementNameFromId(int id)
        {
            return string.Concat("Generated Callback Name for Id: ##", id.ToString(), "##");
        }
    }
}

#endif