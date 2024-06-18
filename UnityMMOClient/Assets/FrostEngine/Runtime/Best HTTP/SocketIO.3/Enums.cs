#if !BESTHTTP_DISABLE_SOCKETIO

namespace BestHTTP.SocketIO3
{
    /// <summary>
    /// 传输级别上可能的事件类型。
    /// </summary>
    public enum TransportEventTypes : int
    {
        Unknown = -1,
        Open = 0,
        Close = 1,
        Ping = 2,
        Pong = 3,
        Message = 4,
        Upgrade = 5,
        Noop = 6
    }

    /// <summary>
    /// SocketIO协议事件类型。
    /// </summary>
    public enum SocketIOEventTypes : int
    {
        Unknown = -1,

        /// <summary>
        /// 连接到名称空间，或者我们连接到名称空间
        /// </summary>
        Connect = 0,

        /// <summary>
        /// 断开命名空间，或者断开与命名空间的连接。
        /// </summary>
        Disconnect = 1,

        /// <summary>
        /// 一般事件。事件的名称在有效负载中。
        /// </summary>
        Event = 2,

        /// <summary>
        /// 事件的确认。
        /// </summary>
        Ack = 3,

        /// <summary>
        /// 由服务器或插件发送的错误
        /// </summary>
        Error = 4,

        /// <summary>
        /// 一个附加在包上的二进制的一般事件。事件的名称在有效负载中。
        /// </summary>
        BinaryEvent = 5,

        /// <summary>
        /// 二进制事件的确认。
        /// </summary>
        BinaryAck = 6
    }
}

#endif
