using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Bag.BagServer
{
    public class AllBagItemRequest : Model, IPacket, IReference
    {
        public int type { get; set; }

        public static AllBagItemRequest ValueOf()
        {
            var packet = new AllBagItemRequest();
            packet.Clear();
            return packet;
        }

        public short ProtocolId()
        {
            return 1007;
        }

        public void Clear()
        {
            type = 0;
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
            var messageWrite = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(messageWrite);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var packet = AllBagItemRequest.ValueOf();
            return packet;
        }
    }
}