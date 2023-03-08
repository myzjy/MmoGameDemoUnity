using System.Runtime.InteropServices;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net.Core
{
    [StructLayout(LayoutKind.Auto)]
    public struct ServerMessageWrite
    {
        public short protocolId;
        public IPacket packet;
        public ServerMessageWrite(short protocolId, IPacket packet)
        {
            this.protocolId = protocolId;
            this.packet = packet;
        }
    }
}