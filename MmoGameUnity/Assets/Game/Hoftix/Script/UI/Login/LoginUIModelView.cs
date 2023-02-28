using ZJYFrameWork.Game.Framwork;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.UISerializable
{
    public partial class UINotifEnum
    {
        #region 登录界面

        /// <summary>
        /// 打开登录界面
        /// </summary>
        public const string OpenLoginUI = "OPEN_LOGIN_UI";

        /// <summary>
        /// 关闭登录界面ui
        /// </summary>
        public const string CloseLoginUI = "CLOSE_LOGIN_UI";

        /// <summary>
        /// 关闭登录 注册 UI面板 login Panel 不隐藏
        /// </summary>
        public const string CloseLoginRegisterUI = "CLOSE_LOGIN_REGISTER_UI";

        /// <summary>
        /// 
        /// </summary>
        public const string CloseLoginTapToStartUI = "CLOSE_LOGIN_TAP_TO_START_UI";

        /// <summary>
        /// 打开 登录界面的开始游戏按钮
        /// </summary>
        public const string OpenLoginTapToStartUI = "OPEN_LOGIN_TAP_TO_START_UI";

        /// <summary>
        /// 当我们登录成功之后，闪过登录账号
        /// </summary>
        public const string ShowLoginAccountUI = "Show_Login_Account_UI";

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
                UINotifEnum.OpenLoginUI,
                UINotifEnum.CloseLoginUI,
                UINotifEnum.CloseLoginRegisterUI,
                UINotifEnum.OpenLoginTapToStartUI,
                UINotifEnum.ShowLoginAccountUI
            };
        }


        public override void NotificationHandler(UINotification _eventNotification)
        {
            switch (_eventNotification.GetEventName)
            {
                case UINotifEnum.OpenLoginUI:
                    InstanceOrReuse();
                    break;
                case UINotifEnum.CloseLoginUI:
                {
                    selfView.OnHide();
                }
                    break;
                case UINotifEnum.CloseLoginRegisterUI:
                {
                    // 输入账号 UI 关闭
                    SpringContext.GetBean<LoginController>().OnHide();
                }
                    break;
                case UINotifEnum.OpenLoginTapToStartUI:
                {
                    selfView.viewPanel.LoginTapToStartView.Show();
                }
                    break;
                case UINotifEnum.ShowLoginAccountUI:
                {
                    selfView.LoginTip();
                }
                    break;
            }
        }
    }
}