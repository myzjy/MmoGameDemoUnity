using System.Collections.Generic;

namespace ZJYFrameWork.UISerializable
{
    public static class UINotificationHelp
    {
        //事件通知池
        private static Queue<UINotification> EventPool;
        private static int m_MaxDefaultCount = 999;

        public static UINotification NewUINotification(string name, object obj)
        {
            //防止一直new 对象
            UINotification UIBody = null;

            if (EventPool == null)
            {
                EventPool = new Queue<UINotification>();
            }
            //使用池子里面的通知
            if (EventPool.Count > 0)
            {
                UIBody = EventPool.Dequeue();
                UIBody.SetNotification(name, obj);
            }
            else
            {
                UIBody = new UINotification(name, obj);
            }

            return UIBody;
        }

        public static void ResUse(UINotification uiNotification)
        {
            if (EventPool.Count > m_MaxDefaultCount)
            {
                uiNotification = null;
            }
            else
            {
                //将事件通知入列 事件通知池
                EventPool.Enqueue(uiNotification);
            }
        }
    }
}