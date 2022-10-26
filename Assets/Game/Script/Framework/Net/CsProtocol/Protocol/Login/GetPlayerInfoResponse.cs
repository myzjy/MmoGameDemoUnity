using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    public class GetPlayerInfoResponse: IPacket
    {
        public static GetPlayerInfoResponse ValueOf()
        {
            var packet = new GetPlayerInfoResponse
            {
            };
            return packet;
        }
        public short ProtocolId()
        {
            return 1005;
        }
    }
    public class GetPlayerInfoResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1005;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                buffer.WriteBool(false);
                return;
            }
            var InfoRequest = (GetPlayerInfoResponse)packet;
            var message = new ServerMessageWrite(InfoRequest.ProtocolId(), InfoRequest);
            var json = JsonConvert.SerializeObject(message);
            Debug.Log(json);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = JsonConvert.DeserializeObject<GetPlayerInfoResponse>(dict["packet"].ToString());
            return packet;
        }
    }
}