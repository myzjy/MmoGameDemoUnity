#if !BESTHTTP_DISABLE_SOCKETIO

using System.Collections.Generic;

namespace BestHTTP.SocketIO.JsonEncoders
{
    /// <summary>
    /// 接口，能够编写自定义Json编码器/解码器。
    /// </summary>
    public interface IJsonEncoder
    {
        /// <summary>
        /// Decode函数必须从Json格式的字符串参数中创建一个对象列表。如果解码失败，它应该返回null。
        /// </summary>
        List<object> Decode(string json);

        /// <summary>
        /// Encode函数必须从参数中创建一个json格式的字符串。如果编码失败，它应该返回null。
        /// </summary>
        string Encode(List<object> obj);
    }
}

#endif