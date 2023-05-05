using System;
using System.Collections.Generic;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;
using Newtonsoft.Json;
using Unity.VisualScripting;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol
{
    public class ItemBaseData : Model, IPacket, IReference
    {
        /* 道具id */
        public int id;

        /** 道具名 **/
        public string name;

        /// <summary>
        /// icon 资源名
        /// </summary>
        public string icon;

        /// <summary>
        /// 最小个数
        /// </summary>
        public int minNum;

        /// <summary>
        /// 最大数量
        /// </summary>
        public int maxNum;

        /// <summary>
        /// 道具类型
        /// </summary>
        public int type;

        /// <summary>
        /// 介绍 是一个 字符组合，需要拆分
        /// </summary>
        public string des;

        public static ItemBaseData ValueOf()
        {
            var item = ReferenceCache.Acquire<ItemBaseData>();
            item.Clear();
            return item;
        }

        public static ItemBaseData ValueOf(string des, string icon, int id, int maxNum, int minNum, string name,
            int type)
        {
            var packet = new ItemBaseData
            {
                des = des,
                icon = icon,
                id = id,
                maxNum = maxNum,
                minNum = minNum,
                name = name,
                type = type
            };
            return packet;
        }


        public short ProtocolId()
        {
            return 201;
        }

        public void Clear()
        {
            des = string.Empty;
            icon = string.Empty;
            id = -1;
            maxNum = -1;
            minNum = -1;
            name = string.Empty;
            type = -1;
        }

        public override void Unpack(byte[] bytes)
        {
            ItemBaseDataSerializer.Unpack(this, bytes);
        }
    }

    public class ItemBaseDataRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 201;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            ItemBaseData message = (ItemBaseData)packet;
            var messageWrite = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(messageWrite);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer, Dictionary<object, object> dict)
        {
            dict.TryGetValue("packet", out var packetJson);
            if (packetJson != null)
            {
                var json = packetJson.ToString();
                var packet = ItemBaseData.ValueOf();
                var bytes = buffer.GetBytes(json);
                packet.Unpack(bytes);
                return packet;
            }

            return null;
        }
    }

    internal abstract class ItemBaseDataSerializer : Serializer
    {
        /// <summary>
        /// 此为json解析,不直接用json库解析,因为可能json传过来的时候,少字段,这样可以明确是那个字段少
        /// </summary>
        /// <param name="response"></param>
        /// <param name="bytes"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void Unpack(ItemBaseData response, byte[] bytes)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("返回成功,协议:[{}]", response.ProtocolId());
#endif
            var byteBuff = ByteBuffer.ValueOf();
            var message = byteBuff.GetString(bytes);
            if (ReadAndCheck(message))
            {
                return;
            }

            var data = JsonConvert.DeserializeObject<Dictionary<object, object>>(message);
            try
            {
                if (data != null)
                {
                    foreach (var (key, value) in data)
                    {
                        var keyString = key.ToString();
                        switch (keyString)
                        {
                            case "id":
                            {
                                var valueStr = value.ToString();
                                response.id = int.Parse(valueStr);
                            }
                                break;
                            case "name":
                            {
                                if (value == null)
                                {
                                    throw new NullReferenceException("消息错误，请检查配置");
                                }

                                response.name = value.ToString();
                            }
                                break;
                            case "icon":
                            {
                                if (value == null)
                                {
                                    throw new NullReferenceException("消息错误，请检查配置");
                                }

                                response.icon = value.ToString();
                            }
                                break;
                            case "minNum":
                            {
                                if (value == null)
                                {
                                    throw new NullReferenceException("消息错误，请检查配置");
                                }

                                var valueStr = value.ToString();
                                response.minNum = int.Parse(valueStr);
                            }
                                break;
                            case "maxNum":
                            {
                                if (value == null)
                                {
                                    throw new NullReferenceException("消息错误，请检查配置");
                                }

                                var valueStr = value.ToString();
                                response.maxNum = int.Parse(valueStr);
                            }
                                break;
                            case "type":
                            {
                                if (value == null)
                                {
                                    throw new NullReferenceException("消息错误，请检查配置");
                                }

                                var valueStr = value.ToString();
                                response.type = int.Parse(valueStr);
                            }
                                break;
                            case "des":
                            {
                                if (value == null)
                                {
                                    throw new NullReferenceException("消息错误，请检查配置");
                                }

                                response.des = value.ToString();
                            }
                                break;
                        }
                    }
                }
                else
                {
                    throw new AggregateException($"{typeof(ItemBaseData)},当前协议解析错误注意！！！！！");
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.LogError(e);
#endif
                throw;
            }
        }
    }
}