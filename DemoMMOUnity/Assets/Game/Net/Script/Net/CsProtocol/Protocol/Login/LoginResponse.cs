using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

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
        /// <returns></returns>
        public static LoginResponse ValueOf()
        {
            var packet = ReferenceCache.Acquire<LoginResponse>();
            return packet;
        }

        public void Clear()
        {
            DiamondNum = -1;
            goldNum = -1;
            PremiumDiamondNum = -1;
            token = string.Empty;
            uid = -1;
            userName = string.Empty;
            lv = -1;
            maxLv = -1;
            exp = -1;
            maxExp = -1;
        }

        public override void Unpack(byte[] bytes)
        {
            LoginResponseSerializer.Unpack(this, bytes);
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

        public IPacket Read(ByteBuffer buffer)
        {
            var packet = LoginResponse.ValueOf();
            packet.Unpack(buffer.ToBytes());
            return packet;
        }
    }

    public abstract class LoginResponseSerializer : Serializer
    {
        /// <summary>
        /// 此为json解析,不直接用json库解析,因为可能json传过来的时候,少字段,这样可以明确是那个字段少
        /// </summary>
        /// <param name="response"></param>
        /// <param name="bytes"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void Unpack(LoginResponse response, byte[] bytes)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("返回成功,协议:[{}]", response.ProtocolId());
#endif
            var message = StringUtils.BytesToString(bytes);
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(message);
           
            foreach (var (key, value) in dict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "token":
                    {
                        response.token = value?.ToString();
                    }
                        break;
                    case "uid":
                    {
                        var valueString = value.ToString();
                        response.uid = long.Parse(valueString);
                    }
                        break;
                    case "userName":
                    {
                        response.userName = value.ToString();
                    }
                        break;
                    case "goldNum":
                    {
                        var valueString = value.ToString();
                        response.goldNum = long.Parse(valueString);
                    }
                        break;
                    case "DiamondNum":
                    {
                        var valueString = value.ToString();
                        response.DiamondNum = long.Parse(valueString);
                    }
                        break;
                    case "PremiumDiamondNum":
                    {
                        var valueString = value.ToString();
                        response.PremiumDiamondNum = long.Parse(valueString);
                    }
                        break;
                    case "lv":
                    {
                        var valueString = value.ToString();
                        response.lv = int.Parse(valueString);
                    }
                        break;
                    case "exp":
                    {
                        var valueString = value.ToString();
                        response.exp = int.Parse(valueString);
                    }
                        break;
                    case "maxLv":
                    {
                        var valueString = value.ToString();
                        response.maxLv = int.Parse(valueString);
                    }
                        break;
                    case "maxExp":
                    {
                        var valueString = value.ToString();
                        response.maxExp = int.Parse(valueString);
                    }
                        break;
                }
            }
        }
    }
}