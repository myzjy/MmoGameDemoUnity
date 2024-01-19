using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Protocol.Weapon
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
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var packet = ReferenceCache.Acquire<WeaponUsePlayerRequest>();
            packet.Clear();
            if (string.IsNullOrEmpty(json))
            {
                return packet;
            }

            var dictBase = JsonConvert.DeserializeObject<Dictionary<Object, Object>>(json);
            if (dictBase == null)
            {
#if UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG)
                Debug.LogError("消息解析错误，请检查");
#endif
                return packet;
            }

            string keyString = string.Empty;
            string valueString = string.Empty;
            foreach (var (key, value) in dictBase)
            {
                keyString = key != null ? key.ToString() : string.Empty;
                valueString = value != null ? value.ToString() : string.Empty;
                switch (keyString)
                {
                    case "findUserId":
                    {
                        if (!string.IsNullOrEmpty(valueString))
                        {
                            packet.findUserId = long.Parse(valueString);
                        }
                    }
                        break;

                    case "findWeaponId":
                    {
                        if (!string.IsNullOrEmpty(valueString))
                        {
                            packet.findWeaponId = long.Parse(valueString);
                        }
                    }
                        break;
                }
            }

            return null;
        }
    }
}