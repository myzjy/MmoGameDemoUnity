using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Map
{
    /// <summary>
    /// 地图 所有总信息 request
    /// </summary>
    public class PuzzleAllConfigRequest : Model, IPacket, IReference
    {
        /// <summary>
        /// 事件id 也可以
        /// </summary>
        public int eventId { get; set; }

        public short ProtocolId()
        {
            return 1035;
        }

        public void Clear()
        {
            eventId = 0;
        }

        public static PuzzleAllConfigRequest ValueOf()
        {
            var packet = ReferenceCache.Acquire<PuzzleAllConfigRequest>();
            packet.Clear();
            return packet;
        }
    }

    public class PuzzleAllConfigRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1035;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var data = (PuzzleAllConfigRequest)packet;
            var packetData = new ServerMessageWrite(ProtocolId(), data);
            var jsonString = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(jsonString);
        }

        public IPacket Read(string json = "")
        {
            var packet = PuzzleAllConfigRequest.ValueOf();
            if (string.IsNullOrEmpty(json))
            {
                return packet;
            }

            return packet;
        }
    }
}