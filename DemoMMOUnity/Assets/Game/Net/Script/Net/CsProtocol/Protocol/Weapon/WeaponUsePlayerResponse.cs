using ZJYFrameWork.Collection.Reference;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Weapon
{
    public class WeaponUsePlayerResponse:Model,IPacket,IReference
    {
        public short ProtocolId()
        {
            return 1040;
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }
    }
}