using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Bag.BagServer
{
    /// <summary>
    /// 获取背包
    /// </summary>
    public class BagServerRequest:IPacket
    {
        public static BagServerRequest ValueOf()
        {
            return new BagServerRequest();
        }
        public short ProtocolId()
        {
            return 1007;
        }
    }
    public class BagServerRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1007;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var bagRequest = (BagServerRequest)packet;
            var message = new ServerMessageWrite(bagRequest.ProtocolId(), bagRequest);
            var json = JsonConvert.SerializeObject(message);
            Debug.Log(json);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = JsonConvert.DeserializeObject<BagServerRequest>(dict["packet"].ToString());
            return packet;
        }
    }
}