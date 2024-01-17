using ZJYFrameWork.Collection.Reference;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Weapon
{
    public class WeaponUsePlayerRequest:Model,IPacket,IReference
    {
        public short ProtocolId()
        {
            return 1039;
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }
    }
}