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
            var _message = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(_message);
            buffer.WriteString(json);
        }

        public IPacket Read(string json)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = ReferenceCache.Acquire<AllBagItemRequest>();
            foreach (var (key, value) in dict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "type":
                    {
                    }
                        break;
                }
            }

            return packet;
        }
    }
}