#if !BESTHTTP_DISABLE_WEBSOCKET && (!UNITY_WEBGL || UNITY_EDITOR)

namespace BestHTTP.WebSocket.Frames
{
    public enum WebSocketFrameTypes : byte
    {
        /// <summary>
        /// <code>
        /// 一个碎片消息的第一帧包含该消息的类型(二进制或文本)，该消息的所有连续帧必须是一个延续帧。
        /// 最后一个帧的Fin位必须为1。
        /// </code>
        /// </summary>
        /// <example>
        /// 对于以三个片段发送的文本消息，第一个片段将有一个操作码0x1 (text)和一个FIN位clear，
        /// 第二个片段将有一个0x0 (Continuation)的操作码和一个FIN位清除，
        /// 和第三个片段将有一个0x0(延续)的操作码和一个FIN位被设置。
        /// </example>
        Continuation        = 0x0,
        Text                = 0x1,
        Binary              = 0x2,
        //Reserved1         = 0x3,
        //Reserved2         = 0x4,
        //Reserved3         = 0x5,
        //Reserved4         = 0x6,
        //Reserved5         = 0x7,

        /// <summary>
        /// <code>
        /// 关闭帧可以包含一个主体(帧的"Application data"部分)，表示关闭的原因，
        /// 例如端点关闭，端点收到帧过大，或端点收到帧过大不符合端点期望的格式。
        /// 由于不能保证数据是人类可读的，客户端绝对不能将其显示给最终用户。
        /// </code>
        /// </summary>
        ConnectionClose     = 0x8,

        /// <summary>
        /// Ping帧包含0x9的操作码。Ping帧可以包含“应用程序数据”。
        /// </summary>
        Ping                = 0x9,

        /// <summary>
        /// 响应Ping帧发送的Pong帧必须具有与被响应的Ping帧的消息体中相同的“应用程序数据”。
        /// </summary>
        Pong                = 0xA,
        //Reserved6         = 0xB,
        //Reserved7         = 0xC,
        //Reserved8         = 0xD,
        //Reserved9         = 0xE,
        //Reserved10        = 0xF,
    }
}

#endif