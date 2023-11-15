using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol
{
    public class RegisterRequest : Model, IPacket,IReference
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
        public static RegisterRequest ValueOf()
        {
            var packet = ReferenceCache.Acquire<RegisterRequest>();
            return packet;
        }

        public short ProtocolId()
        {
            return 1005;
        }

        public void Clear()
        {
            account = string.Empty;
            password = string.Empty;
            affirmPassword = string.Empty;
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
            RegisterRequest message = (RegisterRequest)packet;
            var messageWrite = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(messageWrite);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var packet = RegisterRequest.ValueOf();

            return packet;
        }
    }
}