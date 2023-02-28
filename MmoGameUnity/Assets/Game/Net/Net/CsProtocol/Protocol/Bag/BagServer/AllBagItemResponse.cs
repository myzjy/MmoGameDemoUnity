using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Bag.BagServer
{
    public class AllBagItemResponse : Model, IPacket
    {
        public List<BagUserItemData> list;

        public static AllBagItemResponse ValueOf()
        {
            var packet = new AllBagItemResponse
            {
                list = new List<BagUserItemData>()
            };
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

        public IPacket Read(ByteBuffer buffer, Dictionary<object, object> dict)
        {
            dict.TryGetValue("packet", out var packetJson);

            if (packetJson != null)
            {
                var json = packetJson.ToString();
                var bytes = buffer.GetBytes(json);
                var packet = AllBagItemResponse.ValueOf();
                packet.Unpack(bytes);

                return packet;
            }

            return null;
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

                var list = data.Select(a => a).ToList();
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
                            response.list = new List<BagUserItemData>();
                            for (int j = 0; j < packetList.Count; j++)
                            {
                                var str = packetList[j];
                                var by = byteBuff.GetBytes(str);
                                BagUserItemData datas = items[j];
                                datas.Unpack(by);
                                response.list.Add(datas);
                            }
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