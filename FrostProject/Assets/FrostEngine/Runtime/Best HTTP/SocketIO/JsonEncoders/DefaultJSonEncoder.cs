#if !BESTHTTP_DISABLE_SOCKETIO

using System.Collections.Generic;
using BestHTTP.JSON;

namespace BestHTTP.SocketIO.JsonEncoders
{
    /// <summary>
    /// 默认的IJsonEncoder实现。它使用了来自 BestHTTP 的Json类。JSON命名空间来编码和解码。
    /// </summary>
    public sealed class DefaultJSonEncoder : IJsonEncoder
    {
        public List<object> Decode(string json)
        {
            return Json.Decode(json) as List<object>;
        }

        public string Encode(List<object> obj)
        {
            return Json.Encode(obj);
        }
    }
}

#endif