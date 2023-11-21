using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Equipment
{
    public partial class EquipmentPrimaryConfigBaseData
    {
    }

    public class EquipmentPrimaryConfigBaseDataRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 208;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var packetData = (EquipmentPrimaryConfigBaseData)packet;
            var json = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var packet = EquipmentPrimaryConfigBaseData.valueOf();
            if (string.IsNullOrEmpty(json))
            {
                return packet;
            }

            var jsonDict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            foreach (var (key, value) in jsonDict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "id":
                    {
                        packet.id = int.Parse(value.ToString());
                    }
                        break;
                    case "primaryQuality":
                    {
                        packet.primaryQuality = int.Parse(value.ToString());
                    }
                        break;
                    case "growthPosInt":
                    {
                        packet.growthPosInt = int.Parse(value.ToString());
                    }
                        break;
                    case "growthPosName":
                    {
                        packet.growthPosName = value.ToString();
                    }
                        break;
                    case "primaryGrowthInts":
                    {
                        packet.primaryGrowthInts = value.ToString();
                    }
                        break;
                    case "primaryGrowthMaxInt":
                    {
                        packet.primaryGrowthMaxInt = value.ToString();
                    }
                        break;
                    case "primaryGrowthName":
                    {
                        packet.primaryGrowthName = value.ToString();
                    }
                        break;
                }
            }

            return packet;
        }
    }
}