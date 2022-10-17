using ZJYFrameWork.Game.Framwork;

namespace ZJYFrameWork.UISerializable
{
    public interface UIModelInterface
    {
        string PrefabName();
        /// <summary>
        /// UI层级类型
        /// </summary>
        /// <returns></returns>
        UICanvasType GetCanvasType();

        UISortType GetSortType();
        /// <summary>
        /// 定义UI通知字符
        /// </summary>
        /// <returns></returns>
        string[] Notification();
        void NotificationHandler(UINotification _eventNotification);
        UIView GetUIView();

        void Refresh();
    }
}