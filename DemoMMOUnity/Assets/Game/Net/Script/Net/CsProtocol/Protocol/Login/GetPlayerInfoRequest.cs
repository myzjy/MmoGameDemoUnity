using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    public class GetPlayerInfoRequest : Model, IPacket
    {
        public string token;

        public static GetPlayerInfoRequest ValueOf(string token)
        {
            var packet = new GetPlayerInfoRequest
            {
                token = token
            };
            return packet;
        }

        public short ProtocolId()
        {
            return 1004;
        }
    }

    public class GetPlayerInfoRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1004;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                buffer.WriteBool(false);
                return;
            }

            var InfoRequest = (GetPlayerInfoRequest)packet;
            var message = new ServerMessageWrite(InfoRequest.ProtocolId(), InfoRequest);
            var json = JsonConvert.SerializeObject(message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer, string json)
        {
            return null;
        }
    }
}