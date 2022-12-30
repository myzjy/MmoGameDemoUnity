using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol
{
    
    public class RegisterRequest : IPacket
    {
        public string account;
        public string password;
        public string affirmPassword;

        public static RegisterRequest ValueOf(string account, string affirmPassword, string password)
        {
            var packet = new RegisterRequest
            {
                account = account,
                affirmPassword = affirmPassword,
                password = password
            };
            return packet;
        }


        public short ProtocolId()
        {
            return 1005;
        }
    }


    public class RegisterRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1005;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {

            RegisterRequest message = (RegisterRequest) packet;
            var _message = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(_message);
            buffer.WriteString(json);

        }

        public IPacket Read(ByteBuffer buffer,Dictionary<object, object> dict)
        {
            // var json = StringUtils.BytesToString(buffer.ToBytes());
            // var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            dict.TryGetValue("packet", out var packetJson);
            var packet = JsonConvert.DeserializeObject<RegisterRequest>(packetJson.ToString());

            return packet;
        }
    }
}