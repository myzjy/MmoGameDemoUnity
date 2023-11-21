using ZJYFrameWork.Collection.Reference;

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
}