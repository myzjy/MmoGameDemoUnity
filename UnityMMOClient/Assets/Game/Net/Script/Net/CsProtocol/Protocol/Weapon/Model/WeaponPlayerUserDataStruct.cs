using System;
using System.Collections.Generic;
using FrostEngine;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Module.Weapon.Model
{
    public class WeaponPlayerUserDataStruct : Net.Model, IPacket, IReference
    {
        /**
        * 武器id
        */
        public long id { get; set; }

        /**
         * 武器名字
         */
        public string weaponName { get; set; }

        /**
         * 武器 type 武器所属类型
         */
        public int weaponType { get; set; }

        /**
         * 当前武器所属技能
         */
        public int nowSkills { get; set; }

        /**
         * 武器主词条的数值
         */
        public int weaponMainValue { get; set; }

        /**
         * 武器主词条的所属type
         */
        public int weaponMainValueType { get; set; }

        /**
         * 武器获取到的第一时间
         */
        public string haveTimeAt { get; set; }

        /**
         * 当前武器强化到的等级
         */
        public int nowLv { get; set; }

        /**
         * 当前武器能强化到的最大等级
         * 强化到特定等级会有突破
         * 突破之后这个最大等级就会变化
         * 直到没有可以突破等级
         */
        public int nowMaxLv { get; set; }

        /**
         * 当前 等级 已经强化 到的经验
         */
        public int nowLvExp { get; set; }

        /**
         * 当前等级已知的可以强化的最大经验
         */
        public int nowLvMaxExp { get; set; }

        /**
         * 武器Icon
         */
        public string weaponIcons { get; set; }

        /**
         * 武器模型所属资源名
         */
        public string weaponModelNameIcons { get; set; }

        public short ProtocolId()
        {
            return 213;
        }

        public void Clear()
        {
            id = 0;
            weaponName = string.Empty;
            weaponType = -1;
            nowSkills = 0;
            weaponMainValue = 0;
            weaponMainValueType = 0;
            haveTimeAt = string.Empty;
            nowLv = 0;
            nowMaxLv = 0;
            nowLvExp = 0;
            nowLvMaxExp = 0;
            weaponIcons = string.Empty;
            weaponModelNameIcons = string.Empty;
        }

        /**
         * 无参构造 调用复用
         */
        public static WeaponPlayerUserDataStruct ValueOf()
        {
            var data = ReferenceCache.Acquire<WeaponPlayerUserDataStruct>();
            data.Clear();
            return data;
        }

        /**
         * 有参构造 调用复用
         */
        public static WeaponPlayerUserDataStruct ValueOf(long id, string weaponName, int weaponType, int nowSkills,
            int weaponMainValue, int weaponMainValueType, string haveTimeAt, int nowLv, int nowMaxLv, int nowLvExp,
            int nowLvMaxExp, string weaponIcons, string weaponModelNameIcons)
        {
            var data = ReferenceCache.Acquire<WeaponPlayerUserDataStruct>();
            data.Clear();
            data.id = id;
            data.weaponName = weaponName;
            data.weaponType = weaponType;
            data.nowSkills = nowSkills;
            data.weaponMainValue = weaponMainValue;
            data.weaponMainValueType = weaponMainValueType;
            data.haveTimeAt = haveTimeAt;
            data.nowLv = nowLv;
            data.nowMaxLv = nowMaxLv;
            data.nowLvExp = nowLvExp;
            data.nowLvMaxExp = nowLvMaxExp;
            data.weaponIcons = weaponIcons;
            data.weaponModelNameIcons = weaponModelNameIcons;
            return data;
        }
    }

    public class WeaponPlayerUserDataStructRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 213;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var request = (WeaponPlayerUserDataStruct)packet;
            var packetData = new ServerMessageWrite(request.ProtocolId(), request);
            var jsonData = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(jsonData);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var packet = WeaponPlayerUserDataStruct.ValueOf();
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

            foreach (var (key, value) in dictBase)
            {
                var keyString = key != null ? key.ToString() : string.Empty;
                var valueString = value != null ? value.ToString() : string.Empty;
                if (string.IsNullOrEmpty(valueString))
                {
                    continue;
                }

                switch (keyString)
                {
                    case "id":
                    {
                        packet.id = long.Parse(valueString);
                    }
                        break;
                    case "weaponName":
                    {
                        packet.weaponName = valueString;
                    }
                        break;
                    case "weaponTyp":
                    {
                        packet.weaponType = int.Parse(valueString);
                    }
                        break;
                    case "nowSkills":
                    {
                        packet.nowSkills = int.Parse(valueString);
                    }
                        break;
                    case "weaponMainValue":
                    {
                        packet.weaponMainValue = int.Parse(valueString);
                    }
                        break;
                    case "weaponMainValueType":
                    {
                        packet.weaponMainValueType = int.Parse(valueString);
                    }
                        break;
                    case "haveTimeAt":
                    {
                        packet.haveTimeAt = valueString;
                    }
                        break;
                    case "nowLv":
                    {
                        packet.nowLv = int.Parse(valueString);
                    }
                        break;
                    case "nowMaxLv":
                    {
                        packet.nowMaxLv = int.Parse(valueString);
                    }
                        break;
                    case "nowLvExp":
                    {
                        packet.nowLvExp = int.Parse(valueString);
                    }
                        break;
                    case "nowLvMaxExp":
                    {
                        packet.nowLvMaxExp = int.Parse(valueString);
                    }
                        break;
                    case "weaponIcons":
                    {
                        packet.weaponIcons = valueString;
                    }
                        break;
                    case "weaponModelNameIcons":
                    {
                        packet.weaponModelNameIcons = valueString;
                    }
                        break;
                }
            }

            return packet;
        }
    }
}