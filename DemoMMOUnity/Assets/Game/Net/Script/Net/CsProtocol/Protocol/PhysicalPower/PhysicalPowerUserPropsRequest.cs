using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    /// <summary>
    /// 使用体力
    /// </summary>
    public class PhysicalPowerUserPropsRequest : Model, IPacket
    {
        /// <summary>
        /// 使用体力的数量
        /// </summary>
        public int usePropNum { get; set; }

        public short ProtocolId()
        {
            return 1025;
        }

        public static PhysicalPowerUserPropsRequest ValueOf(int usePropNum)
        {
            var packet = new PhysicalPowerUserPropsRequest
            {
                usePropNum = usePropNum
            };
            return packet;
        }
    }

    public class PhysicalPowerUserPropsRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1025;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var packetData = (PhysicalPowerUserPropsRequest)packet;
            var message = new ServerMessageWrite(packetData.ProtocolId(), packetData);
            var json = JsonConvert.SerializeObject(message);
            buffer.WriteString(json);
        }
        
        public IPacket Read(ByteBuffer buffer, string json)
        {
            return null;
        }
    }
}