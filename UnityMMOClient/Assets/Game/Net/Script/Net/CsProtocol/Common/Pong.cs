using System;
using System.Collections.Generic;
using FrostEngine;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol
{
    public class Pong : Model, IPacket, IReference
    {
        public long time;

        public short ProtocolId()
        {
            return 104;
        }

        public static Pong ValueOf()
        {
            var packet = ReferenceCache.Acquire<Pong>();
            packet.Clear();
            return packet;
        }

        public static Pong ValueOf(long time)
        {
            var packet = new Pong
            {
                time = time
            };
            return packet;
        }

        public void Clear()
        {
            time = 0;
        }

        public override void Unpack(byte[] bytes)
        {
            PongSerializer.Unpack(this, bytes);
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
            var packet = Pong.ValueOf();
            packet.Unpack(buffer.ToBytes());
            return packet;
        }
    }

    public abstract class PongSerializer : Serializer
    {
        /// <summary>
        /// 此为json解析,不直接用json库解析,因为可能json传过来的时候,少字段,这样可以明确是那个字段少
        /// </summary>
        /// <param name="response"></param>
        /// <param name="bytes"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void Unpack(Pong response, byte[] bytes)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("返回成功,协议:[{}]", response.ProtocolId());
#endif
            var json = StringUtils.BytesToString(bytes);
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);

            if (dict != null)
            {
                foreach (var item in dict)
                {
                    string itemKey = item.Key.ToString();
                    switch (itemKey)
                    {
                        case "time":
                            long time = long.Parse(item.Value.ToString());
                            response.time = time;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                throw new NullReferenceException($"Pong Response消息为空找不到 packet 解析出错");
            }
        }
    }
}