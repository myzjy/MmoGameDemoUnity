using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Common
{
    [Bean]
    public class RegisterPartClientCacheData
    {
        /// <summary>
        /// 输入账号名
        /// </summary>
        public string Account;

        /// <summary>
        /// 输入密码
        /// </summary>
        public string Password;

        /// <summary>
        /// 确认密码
        /// </summary>
        public string AffirmPassword;

        /// <summary>
        /// 注册成功了吗
        /// </summary>
        public bool RegisterFlag;

        /// <summary>
        /// 注册的时候是否报错了
        /// </summary>
        public bool RegisterError;
    }
}