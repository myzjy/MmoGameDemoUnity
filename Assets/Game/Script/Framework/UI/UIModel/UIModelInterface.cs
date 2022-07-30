using ZJYFrameWork.Game.Framwork;

namespace ZJYFrameWork.UISerializable.UIModel
{
    public interface UIModelInterface
    {
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
    }
}