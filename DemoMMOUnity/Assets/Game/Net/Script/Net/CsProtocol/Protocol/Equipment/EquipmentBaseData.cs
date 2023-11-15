using System;
using ZJYFrameWork.Collection.Reference;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Equipment
{
    public class EquipmentBaseData : Model, IPacket, IReference
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

        public override void Unpack(byte[] bytes)
        {
            base.Unpack(bytes);  
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
            throw new NotImplementedException();
        }

        public IPacket Read(ByteBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}