using System;
using System.Collections.Generic;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.UI.SystemModel
{
    public class UIModelSystem
    {
        public Dictionary<string, Action<UINotification>> models = new Dictionary<string, Action<UINotification>>();
        //注册
        public void RegisterEvent(string view)
        {
            
        }
        /// <summary>
        /// 调度事件
        /// </summary>
        public void DispatchEvent()
        {
            
        }
    }
}