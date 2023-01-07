using ZJYFrameWork.Game.Framwork;

namespace ZJYFrameWork.UISerializable
{
    public partial class UINotifEnum
    {
        #region 登录界面

        /// <summary>
        /// 打开登录界面
        /// </summary>
        public const string OPEN_LOGIN_UI = "OPEN_LOGIN_UI";

        /// <summary>
        /// 关闭登录界面ui
        /// </summary>
        public const string CLOSE_LOGIN_UI = "CLOSE_LOGIN_UI";

        #endregion
    }

    public class LoginUIModelView : UIBaseModule<LoginView, LoginPanelView>
    {
        public override string PrefabName()
        {
            return "LoginPanel";
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
            return new string[]
            {
                UINotifEnum.OPEN_LOGIN_UI,
                UINotifEnum.CLOSE_LOGIN_UI,
            };
        }


        public override void NotificationHandler(UINotification _eventNotification)
        {
            switch (_eventNotification.GetEventName)
            {
                case UINotifEnum.OPEN_LOGIN_UI:
                    InstanceOrReuse();
                    break;
                case UINotifEnum.CLOSE_LOGIN_UI:
                {
                    selfView.OnHide();
                }
                    break;
            }
        }
    }
}