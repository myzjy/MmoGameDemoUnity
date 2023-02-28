using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    public class LoginRequest : Model, IPacket
    {
        public string account;
        public string password;

        public short ProtocolId()
        {
            return 1000;
        }

        public static LoginRequest ValueOf(string account, string password)
        {
            var packet = new LoginRequest()
            {
                account = account,
                password = password
            };
            return packet;
        }
    }

    public class LoginRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1000;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var loginRequest = (LoginRequest)packet;
            var message = new ServerMessageWrite(loginRequest.ProtocolId(), loginRequest);
            var json = JsonConvert.SerializeObject(message);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(json);
#endif
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer, Dictionary<object, object> dict)
        {
            var packet = JsonConvert.DeserializeObject<LoginRequest>(dict["packet"].ToString());
            return packet;
        }
    }
}