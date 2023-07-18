using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    /// <summary>
    /// 获取体力
    /// 
    /// </summary>
    public class PhysicalPowerRequest : Model, IPacket, IReference
    {
        public long uid { get; set; }

        public short ProtocolId()
        {
            return 1023;
        }

        public void Clear()
        {
            uid = 0;
        }

        public static PhysicalPowerRequest ValueOf(long uid)
        {
            var packet = ReferenceCache.Acquire<PhysicalPowerRequest>();
            packet.Clear();
            packet.uid = uid;
            return packet;
        }
    }

    public class PhysicalPowerRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1023;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var data = (PhysicalPowerRequest)packet;
            var packetData = new ServerMessageWrite(protocolId: ProtocolId(), data);
            var jsonString = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(jsonString);
        }

        public IPacket Read(ByteBuffer buffer, string json = "")
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = ReferenceCache.Acquire<PhysicalPowerRequest>();
            return packet;
        }
    }
}