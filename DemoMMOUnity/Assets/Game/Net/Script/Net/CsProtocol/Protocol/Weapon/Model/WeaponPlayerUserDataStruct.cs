using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.CsProtocol;
using NotImplementedException = System.NotImplementedException;

namespace ZJYFrameWork.Module.Weapon.Model
{
    public class WeaponPlayerUserDataStruct : Net.Model, IPacket, IReference
    {
        public short ProtocolId()
        {
            return 213;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}