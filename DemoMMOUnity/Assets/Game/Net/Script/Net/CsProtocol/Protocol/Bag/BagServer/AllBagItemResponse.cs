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
            var messageWrite = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(messageWrite);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var packet = AllBagItemResponse.ValueOf();
            packet.Unpack(buffer.ToBytes());
            return packet;
        }
    }

    public static class AllBagItemResponseSerializer
    {
        public static void Unpack(AllBagItemResponse response, byte[] bytes)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("返回成功,协议:[{}]", response.ProtocolId());
#endif
            var message = StringUtils.BytesToString(bytes);
            var dataDict = JsonConvert.DeserializeObject<Dictionary<object, object>>(message);
            try
            {
                if (dataDict == null)
                {
                    throw new NullReferenceException($"传递信息为空,请检查{typeof(AllBagItemResponse)}消息体.");
                }


                foreach (var (item, value) in dataDict)
                {
                    if (value == null)
                    {
                        throw new NullReferenceException($"{item}为空,请检查{typeof(AllBagItemResponse)}消息体.");
                    }

                    switch (item)
                    {
                        case "list":
                        {
                            var packetList = JsonConvert.DeserializeObject<List<string>>(value.ToString());
                            var items = new BagUserItemData[packetList.Count];
                            response.list = new List<BagUserItemData>();
                            for (var j = 0; j < packetList.Count; j++)
                            {
                                var str = packetList[j];
                                var byteBuff = ByteBuffer.ValueOf();
                                byteBuff.WriteString(str);
                                var itemData = items[j];
                                var data = ProtocolManager.GetProtocol(itemData.ProtocolId());
                                var iPacketData = data.Read(byteBuff);
                                var protocolData = (BagUserItemData)iPacketData;
                                response.list.Add(protocolData);
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