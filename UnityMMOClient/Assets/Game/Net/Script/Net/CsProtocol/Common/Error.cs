using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol
{
    public class Error : Model, IPacket, IReference
    {
        public int errorCode;
        public string errorMessage;
        public int module;

        public static Error ValueOf()
        {
            var packet = ReferenceCache.Acquire<Error>();
            packet.Clear();
            return packet;
        }

        public static Error ValueOf(int errorCode, string errorMessage, int module)
        {
            var packet = new Error
            {
                errorCode = errorCode,
                errorMessage = errorMessage,
                module = module
            };
            return packet;
        }

        public short ProtocolId()
        {
            return 101;
        }

        public void Clear()
        {
            errorCode = 0;
            errorMessage = "";
            module = 0;
        }
    }

    public class ErrorRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 101;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                buffer.WriteBool(false);
                return;
            }

            var error = (Error)packet;
            var message = new ServerMessageWrite(error.ProtocolId(), error);
            var json = JsonConvert.SerializeObject(message);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var packet = Error.ValueOf();

            return packet;
        }
    }

    public abstract class ErrorSerializer : Serializer
    {
        /// <summary>
        /// 此为json解析,不直接用json库解析,因为可能json传过来的时候,少字段,这样可以明确是那个字段少
        /// </summary>
        /// <param name="response"></param>
        /// <param name="bytes"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void Unpack(Error response, byte[] bytes)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("返回成功,协议:[{}]", response.ProtocolId());
#endif
            var json = StringUtils.BytesToString(bytes);
            if (string.IsNullOrEmpty(json)) return;
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);

            foreach (var (key, value) in dict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "module":
                    {
                        if (value == null) break;
                        var valueString = value.ToString();
                        response.module = int.Parse(valueString);
                    }
                        break;
                    case "errorCode":
                    {
                        if (value == null) break;
                        var valueString = value.ToString();
                        response.errorCode = int.Parse(valueString);
                    }
                        break;
                    case "errorMessage":
                    {
                        if (value == null)
                        {
                            break;
                        }

                        response.errorMessage = value.ToString();
                    }
                        break;
                }
            }
        }
    }
}