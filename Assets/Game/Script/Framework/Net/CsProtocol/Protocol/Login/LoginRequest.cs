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