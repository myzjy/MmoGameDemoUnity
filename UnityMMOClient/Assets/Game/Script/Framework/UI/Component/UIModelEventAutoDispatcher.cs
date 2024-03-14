using System;
using System.Collections.Generic;

namespace ZJYFrameWork.UISerializable
{
    public abstract class UIModelEventAutoDispatcher
    {
        private static readonly Dictionary<string, Action<UINotification>> UIEventNotificationDict =
            new Dictionary<string, Action<UINotification>>();
        public static void Scan()
        {
            UIComponentManager.InitUIModelComponent();
            var all = UIComponentManager.GetList();
            foreach (var item in all)
            {
                
            }
        }
    }
}