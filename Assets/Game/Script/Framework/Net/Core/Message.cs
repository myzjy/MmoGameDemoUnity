using System.Runtime.InteropServices;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net.Core
{
    [StructLayout(LayoutKind.Auto)]
    public struct Message
    {
        public MessageType messageType;
        public IPacket packet;

        public Message(MessageType messageType, IPacket packet)
        {
            this.messageType = messageType;
            this.packet = packet;
        }
    }
}