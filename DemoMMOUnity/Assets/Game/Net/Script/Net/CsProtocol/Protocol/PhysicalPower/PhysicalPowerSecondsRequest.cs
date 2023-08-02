using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    /// <summary>
    /// 体力恢复每S恢复
    /// </summary>
    public class PhysicalPowerSecondsRequest : Model, IPacket, IReference
    {
        public long nowTime { get; set; }

        public short ProtocolId()
        {
            return 1029;
        }

        public void Clear()
        {
            nowTime = 0;
        }

        public static PhysicalPowerSecondsRequest ValueOf(long nowTimes)
        {
            var packetData = ReferenceCache.Acquire<PhysicalPowerSecondsRequest>();
            packetData.Clear();
            packetData.nowTime = nowTimes;
            return packetData;
        }
    }

    /// <summary>
    /// 体力每秒恢复的协议注册
    /// </summary>
    public class PhysicalPowerSecondsRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1029;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            buffer ??= ByteBuffer.ValueOf();
            var packetData = (PhysicalPowerSecondsRequest)packet;
            var message = new ServerMessageWrite(packetData.ProtocolId(), packetData);
            var json = JsonConvert.SerializeObject(message);
            buffer.WriteString(json);
        }

        public IPacket Read(string json = "")
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = ReferenceCache.Acquire<PhysicalPowerSecondsRequest>();
            packet.Clear();
            dict.TryGetValue("nowTime", out var nowTime);
            if (nowTime == null)
            {
                return null;
            }

            var nowTimeNum = long.Parse(nowTime.ToString());
            packet.nowTime = nowTimeNum;
            return packet;
        }
    }
}