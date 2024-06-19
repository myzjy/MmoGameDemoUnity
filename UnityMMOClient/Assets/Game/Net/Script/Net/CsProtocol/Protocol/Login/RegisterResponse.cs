using System.Collections.Generic;
using FrostEngine;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Protocol.Login
{
    public class RegisterResponse : Model, IPacket, IReference
    {
        public bool mRegister;

        public static RegisterResponse ValueOf(bool mRegister)
        {
            var packet = new RegisterResponse
            {
                mRegister = mRegister
            };
            return packet;
        }

        public static RegisterResponse ValueOf()
        {
            var packet = ReferenceCache.Acquire<RegisterResponse>();
            packet.Clear();
            return packet;
        }

        public short ProtocolId()
        {
            return 1006;
        }

        public void Clear()
        {
            mRegister = false;
        }

        public override void Unpack(byte[] bytes)
        {
            RegisterResponseSerializer.Unpack(this, bytes);
        }
    }

    public class RegisterResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1006;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            RegisterResponse message = (RegisterResponse)packet;
            var messageWrite = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(messageWrite);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var packet = RegisterResponse.ValueOf();
            packet.Unpack(buffer.ToBytes());
            return packet;
        }
    }

    internal abstract class RegisterResponseSerializer
    {
        public static void Unpack(RegisterResponse response, byte[] bytes)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("返回成功,协议:[{}]", response.ProtocolId());
#endif
            var message = StringUtils.BytesToString(bytes);
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(message);

            dict.TryGetValue("mRegister", out var mRegister);
            if (mRegister != null)
            {
                response.mRegister = bool.Parse(mRegister.ToString());
            }
        }
    }
}