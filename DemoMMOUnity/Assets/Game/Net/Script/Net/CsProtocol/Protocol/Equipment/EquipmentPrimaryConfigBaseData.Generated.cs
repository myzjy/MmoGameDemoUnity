using ZJYFrameWork.Collection.Reference;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Equipment
{
    public partial class EquipmentPrimaryConfigBaseData:Model,IPacket,IReference
    {
        /**
         * id
         */
        public int id;
        /**
         * 圣遗物 品阶
         */
        public int primaryQuality;
        
        /**
         * 圣遗物 属性 位置
         */
        public int growthPosInt;

        /**
         * 属性位置所属名字
         */
        public string primaryGrowthName;

        /**
         * 1级初始属性
         */
        public string primaryGrowthInts;

        /**
         * 品阶的最大等级的最大属性值
         */
        public string primaryGrowthMaxInt;

        /**
         * 属性名字
         */
        public string growthPosName;
        public short ProtocolId()
        {
            return 208;
        }

        public void Clear()
        {
            id = 0;
            primaryQuality = 0;
            growthPosInt = 0;
            primaryGrowthName = string.Empty;
            primaryGrowthInts = string.Empty;
            primaryGrowthMaxInt = string.Empty;
            growthPosName = string.Empty;
        }

        public static EquipmentPrimaryConfigBaseData valueOf()
        { 
          var data = ReferenceCache.Acquire<EquipmentPrimaryConfigBaseData>();
          data.Clear(); 
          return data;
        }
    }
    public class EquipmentPrimaryConfigBaseDataRegistration : IProtocolRegistration
    {
     public short ProtocolId()
     {
      return 208;
     }

     public void Write(ByteBuffer buffer, IPacket packet)
     {
      throw new System.NotImplementedException();
     }

     public IPacket Read(ByteBuffer buffer)
     {
      throw new System.NotImplementedException();
     }
    }
}