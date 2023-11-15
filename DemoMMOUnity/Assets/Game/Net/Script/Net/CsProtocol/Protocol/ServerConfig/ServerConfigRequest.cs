using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.ServerConfig
{
    /// <summary>
    /// 请求服务器上面配置表
    /// </summary>
    public class ServerConfigRequest : Model, IPacket
    {
        public static ServerConfigRequest ValueOf()
        {
            var packet = new ServerConfigRequest();

            return packet;
        }


        public short ProtocolId()
        {
            return 1009;
        }
    }

    public class ServerConfigRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1009;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            ServerConfigRequest message = (ServerConfigRequest)packet;
            var _message = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(_message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            return null;
        }
    }
}