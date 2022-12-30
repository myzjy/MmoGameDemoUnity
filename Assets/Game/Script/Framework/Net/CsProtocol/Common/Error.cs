using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol
{
    public class Error : IPacket
    {
        public int errorCode;
        public string errorMessage;
        public int module;

        public static Error ValueOf(int errorCode, string errorMessage, int module)
        {
            var packet = new Error
            {
                errorCode = errorCode,
                errorMessage = errorMessage,
                module = module
            };
            return packet;
        }

        public short ProtocolId()
        {
            return 101;
        }
    }

    public class ErrorRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 101;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                buffer.WriteBool(false);
                return;
            }

            var error = (Error)packet;
            var message = new ServerMessageWrite(error.ProtocolId(), error);
            var json = JsonConvert.SerializeObject(message);
            // Debug.Log(json);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer,Dictionary<object,object> dict)
        {
            // var json = StringUtils.BytesToString(buffer.ToBytes());
            // var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = JsonConvert.DeserializeObject<Error>(dict["packet"].ToString());
            return packet;
        }
    }
}