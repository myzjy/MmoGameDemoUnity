namespace Common.GameChannel
{
    /// <summary>
    /// 基础沟道
    /// </summary>
    public abstract class BaseChannel
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 下载load
        /// </summary>
        /// <param name="paramList">数据</param>
        public abstract void DownloadGame(params object[] paramList);
        /// <summary>
        /// 是否谷歌登录
        /// </summary>
        /// <returns>返回是否谷歌登录状态</returns>
        public virtual bool IsGooglePlay()
        {
            return false;
        }
        /// <summary>
        /// 返回BundleId
        /// </summary>
        /// <returns></returns>
        public abstract string GetBundleID();


        /// <summary>
        /// 安装apk
        /// 这个地方需要调用原生代码去安装，目前来说
        /// 不去做，有能力的可以去做 安卓 ios的原生代码
        /// </summary>
        public abstract void InstallApk();
        /// <summary>
        /// 把产品名称返回出去
        /// </summary>
        /// <returns></returns>
        public abstract string GetProductName();
        /// <summary>
        /// 数据跟踪初始化
        /// </summary>
        public virtual void DataTrackInit()
        {
        }
        
        /// <summary>
        /// 得到apk名称
        /// </summary>
        /// <returns></returns>
        public virtual string GetCompanyName()
        {
            return "chivas";
        }
        
        /// <summary>
        /// 内部通道
        /// </summary>
        /// <returns></returns>
        public virtual bool IsInternalChannel()
        {
            return false;
        }
        public abstract void Login();

        public abstract void Logout();
        
        public abstract void Pay(params object[] paramList);
    }
}