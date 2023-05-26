using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol
{
    /// <summary>
    /// 游戏登录拦截
    /// </summary>
    public class LoginTapToStartRequest : Model, IReference, IPacket
    {
        public string clientName { get; set; }

        public void Clear()
        {
            clientName = string.Empty;
        }

        public static LoginTapToStartRequest ValueOf()
        {
            var packet = ReferenceCache.Acquire<LoginTapToStartRequest>();
            packet.Clear();
            packet.clientName =
#if UNITY_EDITOR
                "editor";
#elif UNITY_ANDROID || UNITY_IOS
                "app";
#else
                "pc";
#endif
            return packet;
        }


        public short ProtocolId()
        {
            return 1013;
        }
    }

    public class LoginTapToStartRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1013;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            LoginTapToStartRequest message = (LoginTapToStartRequest)packet;
            var _message = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(_message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer, Dictionary<object, object> dict = null)
        {
            if (dict == null) return null;
            var packet = JsonConvert.DeserializeObject<GetPlayerInfoRequest>(dict["packet"].ToString());


            return packet;
        }
    }
}