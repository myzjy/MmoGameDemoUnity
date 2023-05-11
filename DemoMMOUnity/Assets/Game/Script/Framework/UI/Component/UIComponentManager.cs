using System;
using System.Collections.Generic;
using System.Linq;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.UISerializable
{
    public class UIComponentManager
    {
        private static readonly List<UIModelInterface> UIModelInterfaces = new List<UIModelInterface>();
        
        private static readonly Dictionary<string, UIModelInterface> UIEventNotificationStrDict =
            new Dictionary<string, UIModelInterface>();
        private static readonly Dictionary<string, Action<UINotification>> UIEventNotificationDict =
            new Dictionary<string, Action<UINotification>>();
        public static void InitUIModelComponent()
        {
            var uiModelRegistrationTypeList = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.Equals(typeof(UIComponentManager).Assembly)) continue;
                var results = new List<Type>();
                results.AddRange(assembly.GetTypes());
                foreach (var type in results.Where(type =>
                             !type.ContainsGenericParameters && type.IsClass && !type.IsAbstract &&
                             typeof(UIModelInterface).IsAssignableFrom(type)))
                {
                    uiModelRegistrationTypeList.Add(type);
                }
            }

            foreach (var uiModelRegistration in uiModelRegistrationTypeList.Select(uiModelRegistrationType => (UIModelInterface)Activator.CreateInstance(uiModelRegistrationType)))
            {
                UIModelInterfaces.Add(uiModelRegistration);
                UIEventNotificationStrDict.Add(uiModelRegistration.PrefabName(),uiModelRegistration);
                var stringList = uiModelRegistration.Notification();
                foreach (var str in uiModelRegistration.Notification())
                {
                    UIEventNotificationDict.TryGetValue(str, out var action);
                    if (action == null)
                    {
                        UIEventNotificationDict.Add(str, uiModelRegistration.NotificationHandler);
                    }
                    else
                    {
                        throw new Exception(StringUtils.Format("[class:{}] [Notification:{}] 有重复的事件id",
                            uiModelRegistration.GetType().Name, str));
                    }
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