using ZJYFrameWork.Game.Framwork;

namespace ZJYFrameWork.UISerializable
{
    public partial class UINotifEnum
    {
        /// <summary>
        /// 打开loading界面
        /// </summary>
        public const string OPEN_LOADINNG_UIPANEL = "OPEN_LOADINNG_UIPANEL";

        /// <summary>
        /// 关闭loading界面
        /// </summary>
        public const string CLOSE_LOADING_UIPAENL = "CLOSE_LOADING_UIPAENL";
    }

    public class LoadingUIModelView : UIBaseModule<LoadingUIView, LoadingPanelView>
    {
        public override string PrefabName()
        {
            return "LoadingPanel";
        }

        public override UICanvasType GetCanvasType()
        {
            return UICanvasType.UI;
        }

        public override UISortType GetSortType()
        {
            return UISortType.Last;
        }

        public override string[] Notification()
        {
            return new[]
            {
                UINotifEnum.OPEN_LOADINNG_UIPANEL,
                UINotifEnum.CLOSE_LOADING_UIPAENL,
            };
        }

        public override void NotificationHandler(UINotification _eventNotification)
        {
            switch (_eventNotification.GetEventName)
            {
                case UINotifEnum.OPEN_LOADINNG_UIPANEL:
                {
                    InstanceOrReuse();
                }
                    break;
                case UINotifEnum.CLOSE_LOADING_UIPAENL:
                {
                    selfView.OnHide();
                }
                    break;
            }
        }
    }
}