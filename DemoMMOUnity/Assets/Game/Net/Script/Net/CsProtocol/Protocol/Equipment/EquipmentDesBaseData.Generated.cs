using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Equipment
{
    public partial class EquipmentDesBaseData:Model, IPacket, IReference
    {
        /**
         * 介绍id
         */
        public int desId;

        /**
         * 名字
         */
        public string name;

        /**
         * 介绍
         */
        public string desStr;

        /**
         * 故事
         */
        public string storyDesStr;
        public short ProtocolId()
        {
            return 212;
        }
        public static short GetProtocolId()
        {
            return 212;
        }
        public void Clear()
        {
            desId = 0;
            name = string.Empty;
            desStr = string.Empty;
            storyDesStr = string.Empty;
        }

        public static EquipmentDesBaseData ValueOf()
        {
            var data = ReferenceCache.Acquire<EquipmentDesBaseData>();
            data.Clear();
            return data;
        }
    }

    public class EquipmentDesBaseDataRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 212;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var request = (EquipmentDesBaseData)packet;
            var packetData = new ServerMessageWrite(request.ProtocolId(), request);
            var json = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var packet = EquipmentDesBaseData.ValueOf();
            var json = StringUtils.BytesToString(buffer.ToBytes());
            if (string.IsNullOrEmpty(json))
            {
                return packet;
            }

            var jsonDict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            foreach (var (key,value) in jsonDict)
            {
                var keyStr = key.ToString();
                switch (keyStr)
                {
                    case "desId":
                    {
                        packet.desId = int.Parse(value.ToString());
                    }
                        break;
                    case "name":
                    {
                        packet.name = value.ToString();
                    }
                        break;
                    case "desStr":
                    {
                        packet.desStr = value.ToString();
                    }
                        break;
                    case "storyDesStr":
                    {
                        packet.storyDesStr = value.ToString();
                    }
                        break;
                }
            }
            

            return packet;
        }
    }
}