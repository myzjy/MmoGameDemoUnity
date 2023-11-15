using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Protocol.Login
{
    /// <summary>
    /// 登录时返回用户相关数据
    /// </summary>
    public class GetPlayerInfoResponse : Model, IPacket
    {
        public short ProtocolId()
        {
            return 1011;
        }
    }

    public class GetPlayerInfoResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1011;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            RegisterRequest message = (RegisterRequest)packet;
            var _message = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(_message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {

            return null;
        }
    }

    public class GetPlayerInfoResponseRegistrationSerializer
    {
    }
}