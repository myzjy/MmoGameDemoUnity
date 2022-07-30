using System;
using ZJYFrameWork.Framwork;
// using Game.Scripts.Activity.Model;

namespace ZJYFrameWork.UISerializable.UIModel
{
    public interface IUISystemModule : IUIView
    {
        //注册事件
        void RegisterEvent(string name, Action<UINotification> eventAction);

        //进行调度
        void DispatchEvent(string name, object body = null);
    }
}