#if !BESTHTTP_DISABLE_WEBSOCKET && (!UNITY_WEBGL || UNITY_EDITOR)

using BestHTTP.WebSocket.Frames;

namespace BestHTTP.WebSocket.Extensions
{
    public interface IExtension
    {
        /// <summary>
        /// 这是第一步:在这里，我们可以向请求添加头以发起扩展协商。
        /// </summary>
        /// <param name="request"></param>
        void AddNegotiation(HttpRequest request);

        /// <summary>
        /// 如果websocket升级成功，它将调用这个函数来解析服务器的协商响应。在这个函数中，应该设置IsEnabled。
        /// </summary>
        bool ParseNegotiation(WebSocketResponse resp);

        /// <summary>
        /// 这个函数应该根据inFlag参数返回一个新的头标志。扩展应该只设置头中的Rsv1-3位。
        /// </summary>
        byte GetFrameHeader(WebSocketFrame writer, byte inFlag);

        /// <summary>
        /// 该函数将被调用，以便能够转换将发送到服务器的数据。
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        byte[] Encode(WebSocketFrame writer);

        /// <summary>
        /// 这个函数可以用来解码服务器发送的数据。
        /// </summary>
        byte[] Decode(byte header, byte[] data, int length);
    }
}

#endif