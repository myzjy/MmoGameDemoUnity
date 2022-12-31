using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol
{
    public class Pong :Model, IPacket
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

        public IPacket Read(ByteBuffer buffer,Dictionary<object,object> dict)
        {
            // var json = StringUtils.BytesToString(buffer.ToBytes());
            // var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);

            dict.TryGetValue("packet", out var packetJson);
            if (packetJson != null)
            {
                var packetDict = JsonConvert.DeserializeObject<Dictionary<object, object>>(packetJson.ToString());
                //这里根据当前类是否需要解析其他字段而定
                if (packetDict != null)
                {
                    Pong pong = new Pong();
                    foreach (var item in packetDict)
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
                    //
                    throw new NullReferenceException($"Pong Response消息为空找不到 packet 解析出错");
                }
            }
            else
            {
                throw new NullReferenceException($"Pong Response消息为空找不到 packet 解析出错");
            }

            var packet = JsonConvert.DeserializeObject<Pong>(packetJson.ToString());


            return packet;
        }
    }
}