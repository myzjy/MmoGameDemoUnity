using System;
using System.Collections.Generic;
using System.Linq;
using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.UISerializable
{
    public class UIComponentManager
    {
        private static readonly List<UIModelInterface> UIModelInterfaces = new List<UIModelInterface>();

        // private static readonly Dictionary<string, UIModelInterface> UIEventNotificationStrDict =
        //     new Dictionary<string, UIModelInterface>();
        private static readonly Dictionary<string, Action<UINotification>> UIEventNotificationDict =
            new Dictionary<string, Action<UINotification>>();

        public static void InitUIModelComponent()
        {
            var loginUI = SpringContext.GetBean<LoginUIModelView>();
            var gameUIModelView = SpringContext.GetBean<GameMainModelView>();
            foreach (var str in loginUI.Notification())
            {
                UIEventNotificationDict.TryGetValue(str, out var action);
                if (action == null)
                {
                    UIEventNotificationDict.Add(str, loginUI.NotificationHandler);
                }
                else
                {
                    throw new Exception(StringUtils.Format("[class:{}] [Notification:{}] 有重复的事件id",
                        loginUI.GetType().Name, str));
                }
            }

            foreach (var str in gameUIModelView.Notification())
            {
                UIEventNotificationDict.TryGetValue(str, out var action);
                if (action == null)
                {
                    UIEventNotificationDict.Add(str, gameUIModelView.NotificationHandler);
                }
                else
                {
                    throw new Exception(StringUtils.Format("[class:{}] [Notification:{}] 有重复的事件id",
                        gameUIModelView.GetType().Name, str));
                }
            }
        }
        // public List<UIModelInterface> TimeModelInterfaces=>UIModelInterfaces.Where(a=>a.Notification().Where(s=>s.))

        public static List<UIModelInterface> GetList()
        {
            return UIModelInterfaces;
        }

        public static void DispatchEvent(string name, object body = null)
        {
            //通知器,反复从一个队列中拿出来 反复使用
            var eventUI = UINotificationHelp.NewUINotification(name, body);
            UIEventNotificationDict.TryGetValue(name, out var eventAction);
            eventAction?.Invoke(eventUI);
            //使用完需要放回池子里
            UINotificationHelp.ResUse(eventUI);
        }
    }
}