using System;

namespace ZJYFrameWork.Net
{
    public interface INetSendManager
    {
        void Add(Action action);
    }
}