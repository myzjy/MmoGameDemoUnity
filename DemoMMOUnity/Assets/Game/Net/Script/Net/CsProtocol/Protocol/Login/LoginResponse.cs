using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    public class LoginResponse : Model, IPacket, IReference
    {
        /**
        * 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
        */
        public long DiamondNum;

        /**
        * 金币
        */
        public long goldNum { get; set; }

        /**
        * 付费钻石 一般充值才有，付费钻石转换成普通钻石
        */
        public long PremiumDiamondNum;

        public string token { get; set; }
        public long uid { get; set; }
        public string userName { get; set; }
        public int lv { get; set; }
        public int maxLv { get; set; }
        public int exp { get; set; }
        public int maxExp { get; set; }

        public short ProtocolId()
        {
            return 1001;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="uid"></param>
        /// <param name="userName"></param>
        /// <param name="goldNum"></param>
        /// <param name="premiumDiamondNum"></param>
        /// <param name="diamondNum"></param>
        /// <returns></returns>
        public static LoginResponse ValueOf(string token, long uid, string userName, long goldNum,
            long premiumDiamondNum, long diamondNum)
        {
            var packet = new LoginResponse
            {
                token = token,
                uid = uid,
                userName = userName,
                goldNum = goldNum,
                PremiumDiamondNum = premiumDiamondNum,
                DiamondNum = diamondNum
            };
            return packet;
        }

        public void Clear()
        {
            token = "";
            uid = 0;
            userName = String.Empty;
            goldNum = 0;
            PremiumDiamondNum = 0;
            DiamondNum = 0;
        }
    }

    public class LoginResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1001;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var loginResponse = (LoginResponse)packet;
            var message = new ServerMessageWrite(loginResponse.ProtocolId(), loginResponse);
            var json = JsonConvert.SerializeObject(message);
            buffer.WriteString(json);
        }

        public IPacket Read( string json)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = ReferenceCache.Acquire<LoginResponse>();
            packet.Clear();
            foreach (var (key, value) in dict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "token":
                    {
                        packet.token = value?.ToString();
                    }
                        break;
                    case "uid":
                    {
                        var valueString = value.ToString();
                        packet.uid = long.Parse(valueString);
                    }
                        break;
                    case "userName":
                    {
                        packet.userName = value.ToString();
                    }
                        break;
                    case "goldNum":
                    {
                        var valueString = value.ToString();
                        packet.goldNum = long.Parse(valueString);
                    }
                        break;
                    case "DiamondNum":
                    {
                        var valueString = value.ToString();
                        packet.DiamondNum = long.Parse(valueString);
                    }
                        break;
                    case "PremiumDiamondNum":
                    {
                        var valueString = value.ToString();
                        packet.PremiumDiamondNum = long.Parse(valueString);
                    }
                        break;
                    case "lv":
                    {
                        var valueString = value.ToString();
                        packet.lv = int.Parse(valueString);
                    }
                        break;
                    case "exp":
                    {
                        var valueString = value.ToString();
                        packet.exp = int.Parse(valueString);
                    }
                        break;
                    case "maxLv":
                    {
                        var valueString = value.ToString();
                        packet.maxLv = int.Parse(valueString);
                    }
                        break;
                    case "maxExp":
                    {
                        var valueString = value.ToString();
                        packet.maxExp = int.Parse(valueString);
                    }
                        break;
                }
            }

            return packet;
        }
    }
}