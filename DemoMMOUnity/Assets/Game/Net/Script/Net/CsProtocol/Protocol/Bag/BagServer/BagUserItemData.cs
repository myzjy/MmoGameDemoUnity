using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Protocol.Bag.BagServer
{
    public class BagUserItemData : Model, IPacket, IReference
    {
        public int _id;
        public long masterUserId;
        public int nowItemNum;
        public int itemId;

        public static BagUserItemData ValueOf()
        {
            var packet = ReferenceCache.Acquire<BagUserItemData>();
            packet.Clear();
            return packet;
        }

        public static BagUserItemData ValueOf(int _id, int itemId, long masterUserId, int nowItemNum)
        {
            var packet = new BagUserItemData
            {
                _id = _id,
                itemId = itemId,
                masterUserId = masterUserId,
                nowItemNum = nowItemNum
            };
            return packet;
        }


        public short ProtocolId()
        {
            return 200;
        }

        public override void Unpack(byte[] bytes)
        {
            BagUserItemDataSerializer.Unpack(this, bytes);
        }

        public void Clear()
        {
            _id = -1;
            itemId = -1;
            masterUserId = -1;
            nowItemNum = -1;
        }
    }


    public class BagUserItemDataRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 200;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            BagUserItemData message = (BagUserItemData)packet;
            var _message = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(_message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var packet = BagUserItemData.ValueOf();
            packet.Unpack(buffer.ToBytes());
            return packet;
        }
    }

    internal abstract class BagUserItemDataSerializer
    {
        public static void Unpack(BagUserItemData response, byte[] bytes)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("返回成功,协议:[{}]", response.ProtocolId());
#endif
            var message = StringUtils.BytesToString(bytes);
            var data = JsonConvert.DeserializeObject<Dictionary<object, object>>(message);
            try
            {
                if (data == null)
                {
                    throw new NullReferenceException($"传递信息为空,请检查{typeof(BagUserItemData)}消息体.");
                }

                foreach (var (Key, Value) in data)
                {
                    switch (Key)
                    {
                        case "_id":
                        {
                            if (Value == null)
                            {
                                throw new NullReferenceException($"{Key}为空,请检查{typeof(BagUserItemData)}消息体.");
                            }

                            response._id = int.Parse(Value.ToString());
                        }
                            break;
                        case "masterUserId":
                        {
                            if (Value == null)
                            {
                                throw new NullReferenceException($"{Key}为空,请检查{typeof(BagUserItemData)}消息体.");
                            }

                            response.masterUserId = long.Parse(Value.ToString());
                        }
                            break;
                        case "itemId":
                        {
                            if (Value == null)
                            {
                                throw new NullReferenceException($"{Key}为空,请检查{typeof(BagUserItemData)}消息体.");
                            }

                            response.itemId = int.Parse(Value.ToString());
                        }
                            break;
                        case "nowItemNum":
                        {
                            if (Value == null)
                            {
                                throw new NullReferenceException($"{Key}为空,请检查{typeof(BagUserItemData)}消息体.");
                            }

                            response.nowItemNum = int.Parse(Value.ToString());
                        }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw new NullReferenceException(e.ToString());
            }
        }
    }
}