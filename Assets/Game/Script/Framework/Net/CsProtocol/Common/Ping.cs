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

            Ping message = (Ping)packet;
        }

        public IPacket Read(ByteBuffer buffer)
        {
            Ping packet = new Ping();
            return packet;
        }
    }
}