using ZJYFrameWork.Game.Framwork;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UISerializable
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

        /// <summary>
        /// 点击开始游戏之后的相关处理
        /// </summary>
        public const string LoginTapStartGame = "Login_TapStart_Game";

        #endregion
    }

    [Bean]
    public class LoginUIModelView : UIBaseModule<UILoginView, LoginPanelView>
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
                UINotifEnum.ShowLoginAccountUI,
                UINotifEnum.LoginTapStartGame
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
                    SpringContext.GetBean<LoginUIController>().OnHide();
                }
                    break;
                case UINotifEnum.OpenLoginTapToStartUI:
                {
                    selfView.LoginTapToStartView.Show();
                }
                    break;
                case UINotifEnum.ShowLoginAccountUI:
                {
                    // selfView.LoginTip();
                }
                    break;
                case UINotifEnum.LoginTapStartGame:
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.Log("点击开始游戏之后，服务器在开启时间，可以正常进入");
#endif
                    selfView.LoginTapToStartView.LoginStartGame();
                }
                    break;
            }
        }
    }
}