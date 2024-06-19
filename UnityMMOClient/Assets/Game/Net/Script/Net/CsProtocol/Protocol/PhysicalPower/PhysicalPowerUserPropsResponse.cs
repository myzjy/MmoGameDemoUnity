using System.Collections.Generic;
using FrostEngine;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Protocol.PhysicalPower
{
    public class PhysicalPowerUserPropsResponse : Model, IPacket, IReference
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
            return 1026;
        }

        public static PhysicalPowerUserPropsResponse ValueOf(
            int nowPhysicalPower,
            int residueTime,
            int maximumStrength,
            int maximusResidueEndTime,
            long residueNowTime)
        {
            var packet = ReferenceCache.Acquire<PhysicalPowerUserPropsResponse>();
            packet.Clear();
            packet.nowPhysicalPower = nowPhysicalPower;
            packet.residueTime = residueTime;
            packet.maximumStrength = maximumStrength;
            packet.maximusResidueEndTime = maximusResidueEndTime;
            packet.residueNowTime = residueNowTime;
            return packet;
        }

        public void Clear()
        {
            nowPhysicalPower = 0;
            residueTime = 0;
            maximumStrength = 0;
            maximusResidueEndTime = 0;
            residueNowTime = 0;
        }
    }

    public class PhysicalPowerUserPropsResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1026;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var packetData = (PhysicalPowerUserPropsResponse)packet;
            var message = new ServerMessageWrite(packetData.ProtocolId(), packetData);
            var json = JsonConvert.SerializeObject(message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var response = ReferenceCache.Acquire<PhysicalPowerUserPropsResponse>();
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

    public abstract class PhysicalPowerUserPropsResponseSerializer
    {
        public static void Unpack(PhysicalPowerUserPropsResponse response, Dictionary<object, object> dict)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"返回成功协议号：[{response.ProtocolId()}]");
#endif
            if (dict == null)
            {
                return;
            }
        }
    }
}