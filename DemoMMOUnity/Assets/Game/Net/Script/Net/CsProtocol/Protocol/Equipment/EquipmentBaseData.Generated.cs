using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Equipment
{
    public partial class EquipmentBaseData : Model, IPacket, IReference
    {
        /**
         * 介绍id
         */
        public int desId { get; set; }

        /**
         * 品阶
         */
        public int quality { get; set; }

        /**
         * 装备只能装配在什么位置
         */
        public int equipmentPosType { get; set; }

        /**
         * 圣遗物的名字
         */
        public string equipmentName { get; set; }

        /**
         * 主属性集合可以获取那些属性
         */
        public string mainAttributes { get; set; }


        public short ProtocolId()
        {
            return 209;
        }
        public static short GetProtocolId()
        {
            return 209;
        }

        public void Clear()
        {
            desId = 0;
            quality = 0;
            equipmentPosType = -1;
            equipmentName = string.Empty;
            mainAttributes = string.Empty;
        }

        public static EquipmentBaseData ValueOf()
        {
            var data = ReferenceCache.Acquire<EquipmentBaseData>();
            data.Clear();
            return data;
        }
    }

    public class EquipmentBaseDataRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 209;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var request = (EquipmentBaseData)packet;
            var packetData = new ServerMessageWrite(request.ProtocolId(), request);
            var json = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var buffStr = StringUtils.BytesToString(buffer.ToBytes());
            var packet = EquipmentBaseData.ValueOf();
            if (string.IsNullOrEmpty(buffStr))
            {
                return packet;
            }

            var jsonDict = JsonConvert.DeserializeObject<Dictionary<object, object>>(buffStr);
            foreach (var (key, value) in jsonDict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "desId":
                    {
                        packet.desId = int.Parse(value.ToString());
                    }
                        break;
                    case "quality":
                    {
                        packet.quality = int.Parse(value.ToString());
                    }
                        break;
                    case "equipmentName":
                    {
                        packet.equipmentName = value.ToString();
                    }
                        break;
                    case "equipmentPosType":
                    {
                        packet.equipmentPosType = int.Parse( value.ToString());
                    }
                        break;
                    case "mainAttributes":
                    {
                        packet.mainAttributes = value.ToString();
                    }
                        break;

                }
            }

            return packet;
        }
    }
}