using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net.CsProtocol.Protocol.UserInfo
{
    /// <summary>
    /// 在GameMain 界面请求User 数据 Exp Lv 金币 钻石 付费钻石 角色数据
    /// </summary>
    public class GameMainUserToInfoRequest : Model, IPacket, IReference
    {
        public long uid { get; set; }

        public short ProtocolId()
        {
            return 1031;
        }

        public void Clear()
        {
            uid = 0;
        }

        public static GameMainUserToInfoRequest ValueOf(long uid)
        {
            var packet = ReferenceCache.Acquire<GameMainUserToInfoRequest>();
            packet.Clear();
            packet.uid = uid;
            return packet;
        }
    }

    public class GameMainUserToInfoRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1031;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var request = (GameMainUserToInfoRequest)packet;
            var packetData = new ServerMessageWrite(request.ProtocolId(), request);
            var jsonData = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(jsonData);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            return null;
        }
    }
}