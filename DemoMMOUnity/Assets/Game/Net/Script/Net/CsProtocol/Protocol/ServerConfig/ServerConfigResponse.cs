using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Bag.BagServer;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.ServerConfig
{
    /// <summary>
    /// 系统基础配置表相关
    /// </summary>
    public class ServerConfigResponse : Model, IPacket, IReference
    {
        public List<ItemBaseData> bagItemEntityList;

        public short ProtocolId()
        {
            return 1010;
        }

        public static ServerConfigResponse ValueOf(List<ItemBaseData> bagItemEntityList)
        {
            var packet = new ServerConfigResponse
            {
                bagItemEntityList = bagItemEntityList
            };
            return packet;
        }

        public void Clear()
        {
            bagItemEntityList = new List<ItemBaseData>();
        }
    }

    public class ServerConfigResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1010;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            ServerConfigResponse message = (ServerConfigResponse)packet;
            var _message = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(_message);
            buffer.WriteString(json);
        }

        public IPacket Read(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = ReferenceCache.Acquire<ServerConfigResponse>();
            foreach (var (key, value) in dict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "bagItemEntityList":
                    {
                        var valueString = value.ToString();
                        var bagEntityList = JsonConvert.DeserializeObject<List<object>>(valueString);
                        int length = bagEntityList.Count;
                        packet.bagItemEntityList = new List<ItemBaseData>();
                        for (int i = 0; i < length; i++)
                        {
                            var entityObj = bagEntityList[i];
                            var packetData = ProtocolManager.GetProtocol(201);
                            var packetDataRead = (ItemBaseData)packetData.Read(entityObj.ToString());
                            packet.bagItemEntityList.Add(packetDataRead);
                        }
                    }
                        break;
                }
            }

            return packet;
        }
    }
}