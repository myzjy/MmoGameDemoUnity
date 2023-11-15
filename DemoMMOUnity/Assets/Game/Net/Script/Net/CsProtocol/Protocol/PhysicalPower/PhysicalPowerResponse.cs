using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    /// <summary>
    /// 体力 Response
    /// </summary>
    public class PhysicalPowerResponse : Model, IPacket, IReference
    {
        /// <summary>
        /// 当前体力
        /// </summary>
        public int nowPhysicalPower { get; set; }

        /**
         * 一点体力增长剩余时间
         * <p> 注意这里不是时间戳赋值</p>
         */
        public int residueTime { get; set; }

        /**
         * 最大体力 用于限制 这个值会随着 等级增长
         */
        public int maximumStrength { get; set; }

        /**
         * 我恢复到最大体力的结束时间
         * <p>这里不是时间戳</p>
         */
        public int maximusResidueEndTime { get; set; }

        /**
         * 当前体力实时时间 会跟着剩余时间一起变化
         * */
        public long residueNowTime { get; set; }

        public short ProtocolId()
        {
            return 1024;
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

    public class PhysicalPowerResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1024;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var packetData = (PhysicalPowerResponse)packet;
            var message = new ServerMessageWrite(packetData.ProtocolId(), packetData);
            var json = JsonConvert.SerializeObject(message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var response = PhysicalPowerResponse.ValueOf();
            var json = StringUtils.BytesToString(buffer.ToBytes());
            if (string.IsNullOrEmpty(json))
            {
                return response;
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
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
                        response.maximusResidueEndTime = int.Parse(valueString);
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