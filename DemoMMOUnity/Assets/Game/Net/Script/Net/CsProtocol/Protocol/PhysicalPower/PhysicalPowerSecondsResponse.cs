using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    /// <summary>
    /// 每秒恢复体力的恢复
    /// </summary>
    public class PhysicalPowerSecondsResponse : Model, IPacket, IReference
    {
        /// <summary>
        /// 当前体力
        /// </summary>
        public int nowPhysicalPower { get; set; }

        public int residueTime { get; set; }
        public int maximumStrength { get; set; }
        public long maximusResidueEndTime { get; set; }
        public long residueNowTime { get; set; }

        public short ProtocolId()
        {
            return 1030;
        }

        public void Clear()
        {
            nowPhysicalPower = 0;
            residueTime = 0;
            maximumStrength = 0;
            maximusResidueEndTime = 0;
            residueNowTime = 0;
        }

        public static PhysicalPowerSecondsResponse ValueOf()
        {
            var packet = ReferenceCache.Acquire<PhysicalPowerSecondsResponse>();
            packet.Clear();
            return packet;
        }

        public static PhysicalPowerSecondsResponse ValueOf(int nowPhysicalPower, int residueTime, int maximumStrength,
            int maximusResidueEndTime, int residueNowTime)
        {
            var packet = ReferenceCache.Acquire<PhysicalPowerSecondsResponse>();
            packet.Clear();
            packet.nowPhysicalPower = nowPhysicalPower;
            packet.residueTime = residueTime;
            packet.maximumStrength = maximumStrength;
            packet.maximusResidueEndTime = maximusResidueEndTime;
            packet.residueNowTime = residueNowTime;
            return packet;
        }
    }

    public class PhysicalPowerSecondsResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1030;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var packetData = (PhysicalPowerSecondsResponse)packet;
            var message = new ServerMessageWrite(packetData.ProtocolId(), packetData);
            var json = JsonConvert.SerializeObject(message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer, string json = "")
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var response = ReferenceCache.Acquire<PhysicalPowerSecondsResponse>();
            foreach (var (key, value) in dict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "nowPhysicalPower":
                    {
                        if (value != null)
                        {
                            var valueString = value.ToString();
                            response.nowPhysicalPower = int.Parse(valueString);
                        }
                    }
                        break;
                    case "residueTime":
                    {
                        var valueString = value.ToString();
                        response.residueTime = int.Parse(valueString);
                    }
                        break;
                    case "maximumStrength":
                    {
                        var valueString = value.ToString();
                        response.maximumStrength = int.Parse(valueString);
                    }
                        break;
                    case "maximusResidueEndTime":
                    {
                        var valueString = value.ToString();
                        response.maximusResidueEndTime = long.Parse(valueString);
                    }
                        break;
                    case "residueNowTime":
                    {
                        var valueString = value.ToString();
                        response.residueNowTime = long.Parse(valueString);
                    }
                        break;
                }
            }

            return response;
        }
    }
}