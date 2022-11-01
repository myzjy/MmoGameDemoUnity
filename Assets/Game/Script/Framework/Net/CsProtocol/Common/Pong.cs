using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol
{
    public class Pong : IPacket
    {
        public long time;

        public static Pong ValueOf(long time)
        {
            var packet = new Pong
            {
                time = time
            };
            return packet;
        }

        public short ProtocolId()
        {
            return 104;
        }
    }

    public class PongRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 104;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                buffer.WriteBool(false);
                return;
            }

            buffer.WriteBool(true);
            Pong message = (Pong)packet;
            buffer.WriteLong(message.time);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);

            dict.TryGetValue("packet", out var packetJson);


            var packet = JsonConvert.DeserializeObject<Pong>(packetJson.ToString());


            return packet;
        }
    }
}