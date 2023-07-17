using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    public class PhysicalPowerUserPropsResponse : Model, IPacket, IReference
    {
        public int nowPhysicalPower { get; set; }
        public int residueTime { get; set; }
        public int maximumStrength { get; set; }
        public int maximusResidueEndTime { get; set; }
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

        public IPacket Read(ByteBuffer buffer, string json)
        {
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