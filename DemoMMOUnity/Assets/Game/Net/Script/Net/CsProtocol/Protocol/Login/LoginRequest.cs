using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    public class LoginRequest : Model, IPacket, IReference
    {
        public string account;
        public string password;

        public short ProtocolId()
        {
            return 1000;
        }

        public static LoginRequest ValueOf()
        {
            var packet = ReferenceCache.Acquire<LoginRequest>();
            packet.Clear();
            return packet;
        }

        public static LoginRequest ValueOf(string account, string password)
        {
            var packet = new LoginRequest()
            {
                account = account,
                password = password
            };
            return packet;
        }

        public void Clear()
        {
            account = string.Empty;
            password = string.Empty;
        }

        public override void Unpack(byte[] bytes)
        {
            LoginRequestSerializer.Unpack(this, bytes);
        }
    }

    public class LoginRequestRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1000;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var loginRequest = (LoginRequest)packet;
            var message = new ServerMessageWrite(loginRequest.ProtocolId(), loginRequest);
            var json = JsonConvert.SerializeObject(message);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(json);
#endif
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var data = LoginRequest.ValueOf();
            data.Unpack(buffer.ToBytes());
            return data;
        }
    }

    public abstract class LoginRequestSerializer : Serializer
    {
        /// <summary>
        /// 此为json解析,不直接用json库解析,因为可能json传过来的时候,少字段,这样可以明确是那个字段少
        /// </summary>
        /// <param name="response"></param>
        /// <param name="bytes"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void Unpack(LoginRequest response, byte[] bytes)
        {
            var json = StringUtils.BytesToString(bytes);
            if (string.IsNullOrEmpty(json)) return;
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            foreach (var (key, value) in dict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "account":
                    {
                        response.account = value?.ToString();
                    }
                        break;
                    case "password":
                    {
                        response.password = value?.ToString();
                    }
                        break;
                }
            }
        }
    }
}