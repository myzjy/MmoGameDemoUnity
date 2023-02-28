using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Common
{
    /// <summary>
    /// 登录的时候缓存数据
    /// </summary>
    [Bean]
    public class LoginClientCacheData
    {
        /// <summary>
        /// 账号名
        /// </summary>
        public string account;

        /// <summary>
        /// 密码
        /// </summary>
        public string password;

        /// <summary>
        /// 登录成功了吗
        /// </summary>
        public bool loginFlag;

        /// <summary>
        /// 登录的时候是否报错了
        /// </summary>
        public bool loginError;
    }
}