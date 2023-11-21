using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Equipment
{
    public partial class EquipmentGrowthConfigBaseData:Model, IPacket, IReference
    {
        /**
         * id
         */
        public int id;
        /// <summary>
        /// 圣遗物位置
        /// </summary>
        public int locationOfEquipmentType;
        /// <summary>
        /// 位置的名字
        /// </summary>
        public string posName;
        public short ProtocolId()
        {
            return 211;
        }

        public void Clear()
        {
            id = 0;
            locationOfEquipmentType = 0;
            posName = string.Empty;
        }

        public static EquipmentGrowthConfigBaseData valueOf()
        {
            var data = ReferenceCache.Acquire<EquipmentGrowthConfigBaseData>();
            data.Clear();
            return data;
        }
    }

    public class EquipmentGrowthConfigBaseDataRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 211;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var request = (EquipmentGrowthConfigBaseData)packet;
            var packetData = new ServerMessageWrite(packet.ProtocolId(), request);
            var json = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var packet = EquipmentGrowthConfigBaseData.valueOf();
            if (string.IsNullOrEmpty(json))
            {
                return packet;
            }

            return packet;
        }
    }
}