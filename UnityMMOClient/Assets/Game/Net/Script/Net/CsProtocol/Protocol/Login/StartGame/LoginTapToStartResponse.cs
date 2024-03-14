using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Protocol.Login
{
    public class LoginTapToStartResponse : Model, IReference, IPacket
    {
        /// <summary>
        /// 当不能进入游戏时，提示相关信息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 是否能进入游戏
        /// </summary>
        public bool accessGame { get; set; }

        public void Clear()
        {
            message = string.Empty;
            accessGame = default;
        }

        public short ProtocolId()
        {
            return 1014;
        }
    }

    public class LoginTapToStartResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1014;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            LoginTapToStartResponse data = (LoginTapToStartResponse)packet;
            var message = new ServerMessageWrite(data.ProtocolId(), data);
            var json = JsonConvert.SerializeObject(message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = ReferenceCache.Acquire<LoginTapToStartResponse>();
            packet.Clear();
            foreach (var (key, value) in dict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "message":
                    {
                        packet.message = value.ToString();
                    }
                        break;
                    case "accessGame":
                    {
                        var valueString = value.ToString();
                        packet.accessGame = bool.Parse(valueString);
                    }
                        break;
                }
            }

            return packet;
        }
    }
}