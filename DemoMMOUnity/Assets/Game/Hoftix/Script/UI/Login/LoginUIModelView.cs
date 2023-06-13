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
}