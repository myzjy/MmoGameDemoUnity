using System.Runtime.InteropServices;

namespace FrostEngine
{
    [StructLayout(LayoutKind.Auto)]
    public struct Message
    {
        public readonly NetMessageType messageType;
        public readonly string packet;

        public Message(NetMessageType messageType, string packet)
        {
            this.messageType = messageType;
            this.packet = packet;
        }
    }
}