#if !BESTHTTP_DISABLE_SOCKETIO

namespace BestHTTP.SocketIO
{
    /// <summary>
    /// Possible event types on the transport level.
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
        /// 断开Socket命名空间，或者断开与Socket命名空间的连接。
        /// </summary>
        Disconnect = 1,

        /// <summary>
        /// 一般事件。事件的名称在有效负载中。
        /// </summary>
        Event = 2,

        /// <summary>
        /// 对事件的承认。
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
        /// 进制事件的确认。
        /// </summary>
        BinaryAck = 6
    }

    /// <summary>
    /// SocketIO服务器可能发送的错误代码。
    /// </summary>
    public enum SocketIOErrors
    {
        /// <summary>
        /// Transport unknown
        /// </summary>
        UnknownTransport = 0,

        /// <summary>
        /// Session ID unknown
        /// </summary>
        UnknownSid = 1,

        /// <summary>
        /// 糟糕的握手方式
        /// </summary>
        BadHandshakeMethod = 2,

        /// <summary>
        /// 坏的请求
        /// </summary>
        BadRequest = 3,

        /// <summary>
        /// 试图访问禁止的资源
        /// </summary>
        Forbidden = 4,

        /// <summary>
        /// 插件内部错误!
        /// </summary>
        Internal = 5,

        /// <summary>
        /// 由插件捕获但在用户代码中引发的异常。
        /// </summary>
        User = 6,

        /// <summary>
        /// 一个自定义的，服务器发送的错误，很可能来自Socket。IO中间件。
        /// </summary>
        Custom = 7,
    }
}

#endif
