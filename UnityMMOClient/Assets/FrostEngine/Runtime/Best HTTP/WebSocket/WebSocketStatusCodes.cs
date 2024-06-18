#if !BESTHTTP_DISABLE_WEBSOCKET

namespace BestHTTP.WebSocket
{
    /// <summary>
    /// http://tools.ietf.org/html/rfc6455#section-7.4.1
    /// </summary>
    public enum WebSocketStausCodes : ushort
    {
        /// <summary>
        /// 指示一个正常的闭包，意味着建立连接的目的已经实现。
        /// </summary>
        NormalClosure       = 1000,

        /// <summary>
        /// 指示端点正在“离开”，例如服务器关闭或浏览器已经导航离开页面。
        /// </summary>
        GoingAway           = 1001,

        /// <summary>
        /// 指示端点由于协议错误正在终止连接。
        /// </summary>
        ProtocolError       = 1002,

        /// <summary>
        /// 表示终端正在终止连接，因为它收到了一种它不能接受的数据类型
        /// (例如，只理解文本数据的端点如果接收到二进制消息可能会发送此消息)。
        /// </summary>
        WrongDataType       = 1003,

        /// <summary>
        /// 保留。具体的含义可以在将来定义。
        /// </summary>
        Reserved            = 1004,

        /// <summary>
        ///一个保留值，不能被终端设置为关闭控制帧中的状态码。
        ///它被指定用于期望一个状态码来表明没有状态码实际存在的应用程序。
        /// </summary>
        NoStatusCode        = 1005,

        /// <summary>
        /// 一个保留值，不能被终端设置为关闭控制帧中的状态码。
        /// 它被指定用于期望一个状态码来指示连接异常关闭的应用程序，例如，没有发送或接收一个关闭控制帧。
        /// </summary>
        ClosedAbnormally    = 1006,

        /// <summary>
        ///表示终端正在终止连接，因为它在消息中收到了与消息类型不一致的数据(例如，文本消息中有非utf -8 [RFC3629]数据)。
        /// </summary>
        DataError           = 1007,

        /// <summary>
        ///表示端点正在终止连接，因为它收到了违反其策略的消息。
        ///这是一个通用的状态代码，当没有其他更合适的状态代码(例如1003或1009)或需要隐藏有关策略的特定细节时，可以返回。
        /// </summary>
        PolicyError         = 1008,

        /// <summary>
        ///表示端点正在终止连接，因为它收到了一个太大的消息，它无法处理。
        /// </summary>
        TooBigMessage       = 1009,

        /// <summary>
        ///表示终端(客户端)正在终止连接，因为它期望服务器协商一个或多个扩展。
        ///但是服务器没有在WebSocket握手的响应消息中返回它们。
        ///需要的扩展列表应该出现在Close帧的/reason/部分。注意，这个状态代码不是由服务器使用的，因为它可以使WebSocket握手失败。
        /// </summary>
        ExtensionExpected   = 1010,

        /// <summary>
        ///表示服务器正在终止连接，因为它遇到了一个意外的条件，阻止了它完成请求。
        /// </summary>
        WrongRequest        = 1011,

        /// <summary>
        /// 一个保留值，不能被终端设置为关闭控制帧中的状态码。它被指定用于期望状态代码指示由于未能执行TLS握手而关闭连接的应用程序(例如，无法验证服务器证书)。
        /// </summary>
        TLSHandshakeError   = 1015
    }
}

#endif