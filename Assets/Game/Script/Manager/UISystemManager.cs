using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZJYFrameWork.UISerializable.UIModel;

namespace ZJYFrameWork.UISerializable.Manager
{
    public class UISystemManager: IUISystemModule
    {
        private Dictionary<string, Action<UINotification>> UIEventNotificationDict =
            new Dictionary<string, Action<UINotification>>();

        public void RegisterEvent(string name, Action<UINotification> eventAction)
        {
            if (UIEventNotificationDict.ContainsKey(name))
            {
                //事件是否为同一种
                if (UIEventNotificationDict[name].GetInvocationList().Contains(eventAction))
                {
                    UnityEngine.Debug.LogError($"Error:多个相同事件通知器,检查{name}");
                }
                else
                {
                    UIEventNotificationDict[name] += eventAction;
                }
            }
            else
            {
                UIEventNotificationDict.Add(name, eventAction);
            }
        }

        public void DispatchEvent(string name, object body = null)
        {
            //通知器,反复从一个队列中拿出来 反复使用
            var evnetUI = UINotificationHelp.NewUINotification(name,body);
            UIEventNotificationDict.TryGetValue(name, out var eventAction);
            eventAction?.Invoke(evnetUI);
            //使用完需要放回池子里
            UINotificationHelp.ResUse(evnetUI);
        }
    }
}