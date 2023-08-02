using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

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
            account = String.Empty;
            password = String.Empty;
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

        public IPacket Read(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            var packet = ReferenceCache.Acquire<LoginRequest>();
            foreach (var (key, value) in dict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "account":
                    {
                        packet.account = value?.ToString();
                    }
                        break;
                    case "password":
                    {
                        packet.password = value?.ToString();
                    }
                        break;
                }
            }

            return packet;
        }
    }
}