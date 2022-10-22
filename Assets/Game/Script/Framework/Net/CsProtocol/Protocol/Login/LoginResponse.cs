using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    public class LoginResponse : IPacket
    {
        public string token;

        public static LoginResponse ValueOf(string token)
        {
            var packet = new LoginResponse
            {
                token = token
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

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var message = JsonConvert.DeserializeObject<ServerMessageWrite>(json);

            var packet = (LoginRequest)message.packet;

            return packet;
        }
    }
}