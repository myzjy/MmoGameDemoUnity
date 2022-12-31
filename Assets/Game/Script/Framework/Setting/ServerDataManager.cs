using ZJYFrameWork.Common;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Setting
{
    [Bean]
    public sealed class ServerDataManager
    {
        [Autowired] private LoginClientCacheData LoginCacheData;
        [Autowired] private RegisterPartClientCacheData RegisterPartClientCacheData;

        [AfterPostConstruct]
        public void Init()
        {
        }

        public LoginClientCacheData GetLoginClientCacheData => LoginCacheData;
        public RegisterPartClientCacheData GetRegisterPartClientCacheData => RegisterPartClientCacheData;

        /// <summary>
        /// 保存缓存账号密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        public void SetCacheAccountAndPassword(string account, string password)
        {
            LoginCacheData.account = account;
            LoginCacheData.password = password;
        }

        /// <summary>
        /// 保存缓存账号密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="affirmPassword"></param>
        public void SetCacheRegisterAccountAndPassword(string account, string password, string affirmPassword)
        {
            RegisterPartClientCacheData.Account = account;
            RegisterPartClientCacheData.Password = password;
            RegisterPartClientCacheData.AffirmPassword = affirmPassword;
        }
    }
}