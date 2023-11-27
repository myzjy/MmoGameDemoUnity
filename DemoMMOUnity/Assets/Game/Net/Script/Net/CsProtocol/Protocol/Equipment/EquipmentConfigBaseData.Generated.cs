using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Equipment
{
    public partial class EquipmentConfigBaseData:Model,IPacket,IReference
    {   
        /**
         * quality
         */
        public int quality;
        /**
         * 强化到这个等级 强化获取额外属性条或者升级附属性条
         */
        public int lv1;
        /**
         * 强化到这个等级 强化获取额外属性条或者升级附属性条
         */
        public int lv2;
        /**
         * 强化到这个等级 强化获取额外属性条或者升级附属性条
         */
        public int lv3;
        /**
         * 强化到这个等级 强化获取额外属性条或者升级附属性条
         */
        public int lv4;
        public short ProtocolId()
        {
            return 207;
        }
        public static short GetProtocolId()
        {
            return 207;
        }
        public void Clear()
        {
            quality = 0;
            lv1 = 0;
            lv2 = 0;
            lv3 = 0;
            lv4 = 0;
        }

        public static EquipmentConfigBaseData valueOf()
        {
            var packet = ReferenceCache.Acquire<EquipmentConfigBaseData>();
            packet.Clear();
            return packet;
        }
    }

    public class EquipmentConfigBaseDataRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 207;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var requestData = (EquipmentConfigBaseData)packet;
            var packetData = new ServerMessageWrite(packet.ProtocolId(), requestData);
            var json = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var packet = EquipmentConfigBaseData.valueOf();
            if (string.IsNullOrEmpty(json))
            {
                return packet;
            }

            var jsonDict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            foreach (var (key,value) in jsonDict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "quality":
                    {
                        packet.quality = int.Parse(value.ToString());
                    }
                        break;
                    case "lv1":
                    {
                        packet.lv1 = int.Parse(value.ToString());
                    }
                        break;
                    case "lv2":
                    {
                        packet.lv2 = int.Parse(value.ToString());
                    }
                        break;
                    case "lv3":
                    {
                        packet.lv3 = int.Parse(value.ToString());
                    }
                        break;
                    case "lv4":
                    {
                        packet.lv4 = int.Parse(value.ToString());
                    }
                        break;
                }
            }
            
            
            return packet;
        }
    }
}