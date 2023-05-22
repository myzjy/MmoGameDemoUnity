﻿using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net
{
    public interface INetManager
    {
        void Connect(string url);

        void Close();

        void Send(IPacket packet);
        void SendMessage(byte[] bytes);
    }
}