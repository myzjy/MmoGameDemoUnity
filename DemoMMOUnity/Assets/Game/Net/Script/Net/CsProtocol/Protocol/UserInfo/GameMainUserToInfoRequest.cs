using ZJYFrameWork.Collection.Reference;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.UserInfo
{
    /// <summary>
    /// 在GameMain 界面请求User 数据 Exp Lv 金币 钻石 付费钻石 角色数据
    /// </summary>
    public class GameMainUserToInfoRequest : Model, IPacket, IReference
    {
        public short ProtocolId()
        {
            return 1031;
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }
    }
}