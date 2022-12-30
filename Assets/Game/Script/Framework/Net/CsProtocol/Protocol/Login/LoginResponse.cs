using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    public class LoginResponse : IPacket
    {
        public string token;
        public long uid;
        public string userName;

        public static LoginResponse ValueOf(string token,long uid,string userName)
        {
            var packet = new LoginResponse
            {
                token = token,
                uid=uid,
                userName = userName
            };
            return packet;
        }

        public short ProtocolId()
        {
            return 1001;
        }
    }

    public class LoginResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1001;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var loginResponse = (LoginResponse)packet;
            var message = new ServerMessageWrite(loginResponse.ProtocolId(), loginResponse);
            var json = JsonConvert.SerializeObject(message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer,Dictionary<object, object> dic)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = JsonConvert.DeserializeObject<LoginResponse>(dict["packet"].ToString());

            return packet;
        }
    }
}