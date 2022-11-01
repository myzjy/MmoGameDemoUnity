using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net.CsProtocol
{
    public class Ping : IPacket
    {
        public static Ping ValueOf()
        {
            var packet = new Ping();
            return packet;
        }

        public short ProtocolId()
        {
            return 103;
        }
    }

    public class PingRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 103;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            Ping oPing = (Ping)packet;
            var message = new ServerMessageWrite(oPing.ProtocolId(), oPing);
            var json = JsonConvert.SerializeObject(message);
            Debug.Log(json);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            Ping packet = new Ping();
            return packet;
        }
    }
}