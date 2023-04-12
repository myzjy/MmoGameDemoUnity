namespace ZJYFrameWork.Module.Login.Service
{
    public interface ILoginService
    {
        /// <summary>
        /// 链接
        /// </summary>
        void ConnectToGateway();

        /// <summary>
        /// 用token登录
        /// </summary>
        void LoginByToken();

        /// <summary>
        /// 账号登录
        /// </summary>
        void LoginByAccount();

        void Logout();

        /// <summary>
        /// 账号登录之后， 点击开始游戏
        /// </summary>
        void LoginTapToStart();
    }
}