using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Bag.Item
{
    public class ItemBaseData : Model, IPacket
    {
        public int id;
        public string name;
        public string icon;
        public int minNum;
        public int maxNum;
        public int type;
        public string des;

        public static ItemBaseData ValueOf()
        {
            return new ItemBaseData();
        }

        public static ItemBaseData ValueOf(string des, string icon, int id, int maxNum, int minNum, string name,
            int type)
        {
            var packet = new ItemBaseData
            {
                des = des,
                icon = icon,
                id = id,
                maxNum = maxNum,
                minNum = minNum,
                name = name,
                type = type
            };
            return packet;
        }


        public short ProtocolId()
        {
            return 201;
        }
    }

    public class ItemBaseDataRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 201;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            ItemBaseData message = (ItemBaseData)packet;
            var _message = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(_message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer, Dictionary<object, object> dict)
        {
            dict.TryGetValue("packet", out var packetJson);
            if (packetJson != null)
            {
                var packet = JsonConvert.DeserializeObject<ItemBaseData>(packetJson.ToString());

                return packet;
            }

            return null;
        }
    }
}