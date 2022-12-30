using System.Runtime.InteropServices;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net.Core
{
    [StructLayout(LayoutKind.Auto)]
    public struct Message
    {
        public readonly MessageType messageType;
        public readonly IPacket packet;

        public Message(MessageType messageType, IPacket packet)
        {
            this.messageType = messageType;
            this.packet = packet;
        }
    }
}