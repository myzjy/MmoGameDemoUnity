using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Bag.BagServer
{
    public class AllBagItemResponse : Model, IPacket, IReference
    {
        public List<BagUserItemData> list;

        public static AllBagItemResponse ValueOf()
        {
            var packet = ReferenceCache.Acquire<AllBagItemResponse>();
            packet.Clear();
            return packet;
        }

        public static AllBagItemResponse ValueOf(List<BagUserItemData> list)
        {
            var packet = new AllBagItemResponse
            {
                list = list
            };
            return packet;
        }


        public short ProtocolId()
        {
            return 1008;
        }

        public override void Unpack(byte[] bytes)
        {
            AllBagItemResponseSerializer.Unpack(this, bytes);
        }

        public void Clear()
        {
            list?.Clear();
            list = new List<BagUserItemData>();
        }
    }


    public class AllBagItemResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1008;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            AllBagItemResponse message = (AllBagItemResponse)packet;
            var _message = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(_message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer, string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);

            var packet = AllBagItemResponse.ValueOf();
            var list = dict.Select(a => a).ToList();
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                var item = list[i];
                if (item.Value == null)
                {
                    throw new NullReferenceException($"{item.Key}为空,请检查{typeof(AllBagItemResponse)}消息体.");
                }

                switch (item.Key)
                {
                    case "list":
                    {
                        var packetList = JsonConvert.DeserializeObject<List<string>>(item.Value.ToString());
                        var items = new BagUserItemData[packetList.Count];
                        packet.list = new List<BagUserItemData>();
                        for (int j = 0; j < packetList.Count; j++)
                        {
                            var str = packetList[j];
                            BagUserItemData itemData = items[j];
                            var data = ProtocolManager.GetProtocol(itemData.ProtocolId());
                            var iPacketData = data.Read(buffer, str);
                            var protocolData = (BagUserItemData)iPacketData;
                            packet.list.Add(protocolData);
                        }
                    }
                        break;
                }
            }

            return packet;
        }
    }

    public static class AllBagItemResponseSerializer
    {
        public static void Unpack(AllBagItemResponse response, byte[] bytes)
        {
            var byteBuff = ByteBuffer.ValueOf();
            var message = StringUtils.BytesToString(bytes);
            var data = JsonConvert.DeserializeObject<Dictionary<object, object>>(message);
            try
            {
                if (data == null)
                {
                    throw new NullReferenceException($"传递信息为空,请检查{typeof(AllBagItemResponse)}消息体.");
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