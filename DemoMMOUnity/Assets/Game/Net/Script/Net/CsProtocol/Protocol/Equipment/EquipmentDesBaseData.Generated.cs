using ZJYFrameWork.Collection.Reference;

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
    
}