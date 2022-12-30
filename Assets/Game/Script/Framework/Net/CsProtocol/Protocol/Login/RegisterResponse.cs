using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace  ZJYFrameWork.Net.CsProtocol
{
    public class RegisterResponse: IPacket
    {
        public bool mRegister;

        public static RegisterResponse ValueOf(bool mRegister)
        {
            var packet = new RegisterResponse
            {
                mRegister = mRegister
            };
            return packet;
        }


        public short ProtocolId()
        {
            return 1006;
        }
    }
    public class RegisterResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1006;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {

            RegisterResponse message = (RegisterResponse) packet;
            var _message = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(_message);
            buffer.WriteString(json);

        }

        public IPacket Read(ByteBuffer buffer,Dictionary<object, object> dict)
        {
            // var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>( StringUtils.BytesToString(buffer.ToBytes()));
            dict.TryGetValue("packet", out var packetJson);
            if (packetJson != null)
            {
                var packet = JsonConvert.DeserializeObject<RegisterResponse>(packetJson.ToString());

                return packet;
            }

            return null;
        }
    }
}