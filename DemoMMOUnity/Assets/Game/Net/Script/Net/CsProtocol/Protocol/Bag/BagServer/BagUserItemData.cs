using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Bag.BagServer
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

        public IPacket Read(string json)
        {
            var packet = BagUserItemData.ValueOf();
            var data = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var list = data.Select(a => a).ToList();
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                var item = list[i];
                switch (item.Key)
                {
                    case "_id":
                    {
                        if (item.Value == null)
                        {
                            throw new NullReferenceException($"{item.Key}为空,请检查{typeof(BagUserItemData)}消息体.");
                        }

                        packet._id = int.Parse(item.Value.ToString());
                    }
                        break;
                    case "masterUserId":
                    {
                        if (item.Value == null)
                        {
                            throw new NullReferenceException($"{item.Key}为空,请检查{typeof(BagUserItemData)}消息体.");
                        }

                        packet.masterUserId = long.Parse(item.Value.ToString());
                    }
                        break;
                    case "itemId":
                    {
                        if (item.Value == null)
                        {
                            throw new NullReferenceException($"{item.Key}为空,请检查{typeof(BagUserItemData)}消息体.");
                        }

                        packet.itemId = int.Parse(item.Value.ToString());
                    }
                        break;
                    case "nowItemNum":
                    {
                        if (item.Value == null)
                        {
                            throw new NullReferenceException($"{item.Key}为空,请检查{typeof(BagUserItemData)}消息体.");
                        }

                        packet.nowItemNum = int.Parse(item.Value.ToString());
                    }
                        break;
                }
            }

            return packet;
        }
    }

    internal abstract class BagUserItemDataSerializer
    {
        public static void Unpack(BagUserItemData response, byte[] bytes)
        {
            var message = StringUtils.BytesToString(bytes);
            var data = JsonConvert.DeserializeObject<Dictionary<object, object>>(message);
            try
            {
                if (data == null)
                {
                    throw new NullReferenceException($"传递信息为空,请检查{typeof(BagUserItemData)}消息体.");
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