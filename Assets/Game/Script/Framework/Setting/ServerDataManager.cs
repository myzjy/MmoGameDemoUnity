using ZJYFrameWork.Common;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Setting
{
    [Bean]
    public sealed class ServerDataManager
    {
        [Autowired] private LoginClientCacheData LoginCacheData;


        [AfterPostConstruct]
        public void Init()
        {
          
        }

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
        
        public LoginClientCacheData GetLoginClientCacheData => LoginCacheData;
    }
}