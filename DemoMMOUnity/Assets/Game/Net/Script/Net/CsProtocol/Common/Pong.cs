using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net.CsProtocol
{
    public class Pong : Model, IPacket
    {
        public long time;

        public short ProtocolId()
        {
            return 104;
        }

        public static Pong ValueOf(long time)
        {
            var packet = new Pong
            {
                time = time
            };
            return packet;
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

        public IPacket Read(string json)
        {
            // var json = StringUtils.BytesToString(buffer.ToBytes());
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);

            if (dict != null)
            {
                Pong pong = new Pong();
                foreach (var item in dict)
                {
                    string itemKey = item.Key.ToString();
                    switch (itemKey)
                    {
                        case "time":
                            long time = long.Parse(item.Value.ToString());
                            pong = Pong.ValueOf(time);
                            break;
                        default:
                            break;
                    }
                }

                return pong;
            }
            else
            {
                throw new NullReferenceException($"Pong Response消息为空找不到 packet 解析出错");
            }
        }
    }
}