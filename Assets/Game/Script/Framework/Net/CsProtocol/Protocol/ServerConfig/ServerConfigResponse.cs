using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Bag.Item;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.ServerConfig
{
    /// <summary>
    /// 系统基础配置表相关
    /// </summary>
    public class ServerConfigResponse : Model, IPacket
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

        public override void Unpack(byte[] bytes)
        {
            base.Unpack(bytes);
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

        public IPacket Read(ByteBuffer buffer, Dictionary<object, object> dict)
        {
            dict.TryGetValue("packet", out var packetJson);
            var packet = JsonConvert.DeserializeObject<ServerConfigResponse>(packetJson.ToString());

            return packet;
        }
    }
}