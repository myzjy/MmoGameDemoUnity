using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    public class LoginRequest : IPacket
    {
        public string account;
        public string password;

        public static LoginRequest ValueOf(string account, string password)
        {
            var packet = new LoginRequest()
            {
                account = account,
                password = password
            };
            return packet;
        }

        public short ProtocolId()
        {
            return 1000;
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
            Debug.Log(json);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = JsonConvert.DeserializeObject<LoginRequest>(dict["packet"].ToString());
            return packet;
        }
    }
}