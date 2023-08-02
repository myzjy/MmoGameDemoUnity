using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol
{
    public class RegisterResponse : Model, IPacket, IReference
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

        public void Clear()
        {
            mRegister = false;
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
            RegisterResponse message = (RegisterResponse)packet;
            var _message = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(_message);
            buffer.WriteString(json);
        }

        public IPacket Read(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);

            var packet = ReferenceCache.Acquire<RegisterResponse>();
            dict.TryGetValue("mRegister", out var mRegister);
            if (mRegister != null)
            {
                packet.mRegister = bool.Parse(mRegister.ToString());
            }

            return packet;
        }
    }
}