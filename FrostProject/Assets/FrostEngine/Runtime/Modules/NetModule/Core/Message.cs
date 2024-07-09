using System.Runtime.InteropServices;

namespace FrostEngine
{
    [StructLayout(LayoutKind.Auto)]
    public struct Message
    {
        public readonly MessageType messageType;
        public readonly string packet;

        public Message(MessageType messageType, string packet)
        {
            this.messageType = messageType;
            this.packet = packet;
        }
    }
}