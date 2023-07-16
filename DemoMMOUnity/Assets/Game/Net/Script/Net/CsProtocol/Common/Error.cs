using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol
{
    public class Error : IPacket, IReference
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

        public void Clear()
        {
            errorCode = 0;
            errorMessage = "";
            module = 0;
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
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer, string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = ReferenceCache.Acquire<Error>();
            foreach (var (key, value) in dict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "module":
                    {
                        if (value == null) break;
                        var valueString = value.ToString();
                        packet.module = int.Parse(valueString);
                    }
                        break;
                    case "errorCode":
                    {
                        if (value == null) break;
                        var valueString = value.ToString();
                        packet.errorCode = int.Parse(valueString);
                    }
                        break;
                    case "errorMessage":
                    {
                        if (value == null)
                        {
                            break;
                        }

                        packet.errorMessage = value.ToString();
                    }
                        break;
                }
            }

            return packet;
        }
    }
}