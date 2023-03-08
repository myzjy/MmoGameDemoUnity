using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Bag.BagServer
{
    public class AllBagItemRequest : Model, IPacket
    {
        public int type;
        public static AllBagItemRequest ValueOf()
        {
            var packet = new AllBagItemRequest();

            return packet;
        }

        public short ProtocolId()
        {
            return 1007;
        }
    }

    public class AllBagItemRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1007;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            AllBagItemRequest message = (AllBagItemRequest)packet;
            var _message = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(_message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer, Dictionary<object, object> dict)
        {
            dict.TryGetValue("packet", out var packetJson);
            var packet = JsonConvert.DeserializeObject<AllBagItemRequest>(packetJson.ToString());

            return packet;
        }
    }
}