using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Weapon
{
    public class WeaponUsePlayerRequest : Model, IPacket, IReference
    {
        /**
         * 需要查找的玩家id
         */
        public long findUserId { get; set; }

        /**
         *
         * 需要成查找的某件装备 为0 代表 不需要查找特定装备
         * 部位负数或0的时候 就需要查找特定装备
         */
        public long findWeaponId { get; set; }

        public short ProtocolId()
        {
            return 1039;
        }

        public void Clear()
        {
            findUserId = 0;
            findWeaponId = 0;
        }

        public static WeaponUsePlayerRequest ValueOf(long findUserId, long findWeaponId)
        {
            var data = ReferenceCache.Acquire<WeaponUsePlayerRequest>();
            data.Clear();
            data.findWeaponId = findWeaponId;
            data.findUserId = findUserId;
            return data;
        }
    }

    public class WeaponUsePlayerRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1039;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var request = (WeaponUsePlayerRequest)packet;
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