using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net.CsProtocol.Protocol.PhysicalPower
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

        public IPacket Read(ByteBuffer buffer)
        {
            var packet = ReferenceCache.Acquire<PhysicalPowerRequest>();
            return packet;
        }
    }
}