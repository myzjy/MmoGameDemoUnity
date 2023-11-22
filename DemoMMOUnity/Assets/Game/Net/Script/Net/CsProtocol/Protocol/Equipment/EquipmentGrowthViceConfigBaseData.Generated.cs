using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Equipment
{
    public partial class EquipmentGrowthViceConfigBaseData:Model, IPacket, IReference
    {
        /// <summary>
        /// id
        /// </summary>
        public int viceId;

        /// <summary>
        /// 详细属性
        /// </summary>
        public string viceName;

        /// <summary>
        /// 属性所属pos对应
        /// </summary>
        public int posGrowthType;

        /// <summary>
        /// 副属性的初始值数组
        /// </summary>
        public List<string> initNums;
        public short ProtocolId()
        {
            return 210;
        }
        public static short GetProtocolId()
        {
            return 210;
        }
        public void Clear()
        {
            viceId = 0;
            viceName = string.Empty;
            posGrowthType = 0;
            initNums = new List<string>();
        }

        public static EquipmentGrowthViceConfigBaseData valueOf()
        {
            var data = ReferenceCache.Acquire<EquipmentGrowthViceConfigBaseData>();
            data.Clear();
            return data;
        }
    }

    public  class EquipmentGrowthViceConfigBaseDataRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 210;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var request = (EquipmentGrowthViceConfigBaseData)packet;
            var packetData = new ServerMessageWrite(request.ProtocolId(), request);
            var json = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var packet = EquipmentGrowthViceConfigBaseData.valueOf();
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
                    case "viceId":
                    {
                        packet.viceId = int.Parse(value.ToString());
                    }
                        break;
                    case "viceName":
                    {
                        packet.viceName = value.ToString();
                    }
                        break;
                    case "posGrowthType":
                    {
                        packet.posGrowthType = int.Parse(value.ToString());
                    }
                        break;
                    case "initNums":
                    {
                        var list=JsonConvert.DeserializeObject<List<object>>(value.ToString());

                        packet.initNums = list.Select(a => a.ToString()).ToList();
                    }
                        break;
                }
            }

            return packet;
        }
    }
}