using System;
using System.Collections;
using System.Collections.Generic;
using FrostEngine;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Module.Weapon.Model;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Weapon
{
    public class WeaponUsePlayerResponse : Model, IPacket, IReference
    {
        /**
         * 当前玩家调用 查询到的是谁的 装备
         */
        public long usePlayerUid { get; set; }

        /**
         * 玩家武器数据
         */
        public List<WeaponPlayerUserDataStruct> weaponPlayerUserDataStructList { get; set; }

        public short ProtocolId()
        {
            return 1040;
        }

        public void Clear()
        {
            usePlayerUid = -1;
            if (weaponPlayerUserDataStructList != null)
            {
                weaponPlayerUserDataStructList.Clear();
            }
            else
            {
                weaponPlayerUserDataStructList = new List<WeaponPlayerUserDataStruct>();
            }
        }

        public static WeaponUsePlayerResponse ValueOf()
        {
            var data = ReferenceCache.Acquire<WeaponUsePlayerResponse>();
            data.Clear();
            return data;
        }
    }

    public class WeaponUsePlayerResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1040;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var request = (WeaponUsePlayerResponse)packet;
            var packetData = new ServerMessageWrite(request.ProtocolId(), request);
            var jsonData = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(jsonData);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var packet = WeaponUsePlayerResponse.ValueOf();
            if (string.IsNullOrEmpty(json))
            {
                return packet;
            }

            var dictBase = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
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
                    case "usePlayerUid":
                    {
                        if (!string.IsNullOrEmpty(valueString))
                        {
                            packet.usePlayerUid = long.Parse(valueString);
                        }
                    }
                        break;

                    case "weaponPlayerUserDataStructList":
                    {
                        if (!string.IsNullOrEmpty(valueString))
                        {
                            if (valueString == "[]")
                            {
                            }
                            else
                            {
                                ByteBuffer strBuffer = ByteBuffer.ValueOf();
                                var stringList = JsonConvert.DeserializeObject<List<Object>>(valueString);
                                if (stringList.Count > 0)
                                {
                                    foreach (var str in stringList)
                                    {
                                        var objString = str.ToString();
                                        var registerProtocol = ProtocolManager.GetProtocol(213);
                                        strBuffer.WriteString(objString);
                                        var packetData = (WeaponPlayerUserDataStruct)registerProtocol.Read(strBuffer);
                                        packet.weaponPlayerUserDataStructList.Add(packetData);
                                        strBuffer.Clear();
                                    }
                                }
                            }
                        }
                    }
                        break;
                }
            }

            return packet;
        }
    }
}