using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    public class LoginResponse : Model, IPacket
    {
        /**
     * 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
     */
        public long DiamondNum;

        /**
     * 金币
     */
        public long goldNum;

        /**
     * 付费钻石 一般充值才有，付费钻石转换成普通钻石
     */
        public long PremiumDiamondNum;

        public string token;
        public long uid;
        public string userName;

        public short ProtocolId()
        {
            return 1001;
        }

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

        public override void Unpack(byte[] bytes)
        {
            base.Unpack(bytes);
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

        public IPacket Read(ByteBuffer buffer, Dictionary<object, object> dict)
        {
            var packet = JsonConvert.DeserializeObject<LoginResponse>(dict["packet"].ToString());

            return packet;
        }
    }

    public abstract class LoginResponseSerializer
    {
        public static void Unpack(LoginResponse response, byte[] bytes)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"返回成功协议号：[{response.ProtocolId()}]");
#endif
            try
            {
                var byteBuff = ByteBuffer.ValueOf();
                var message = byteBuff.GetString(bytes);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log($"解析bytes ---> message:[{message}]");
#endif
                var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(message);

                if (dict == null)
                {
                    throw new AggregateException($"[type:{typeof(LoginResponse)}] [message:{message}] 无法完整解析成字典重新查看");
                }

                foreach (var item in dict)
                {
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log($"解析bytes ---> message:[{e}]");
#endif
                throw new Exception(e.Message);
            }
        }
    }
}