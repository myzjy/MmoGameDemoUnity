using System.Collections.Generic;
using ZJYFrameWork.Common;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Bag.Item;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Setting
{
    [Bean]
    public class ServerDataManager
    {
        /// <summary>
        /// 基础配置相关
        /// </summary>
        private List<ItemBaseData> _itemBaseDataList = new List<ItemBaseData>();

        [Autowired] private LoginClientCacheData LoginCacheData;
        [Autowired] private RegisterPartClientCacheData RegisterPartClientCacheData;

        /// <summary>
        /// 服务器上面道具相关基础配置表
        /// </summary>
        public List<ItemBaseData> ItemBaseDataList => _itemBaseDataList;

        public LoginClientCacheData GetLoginClientCacheData => LoginCacheData;
        public RegisterPartClientCacheData GetRegisterPartClientCacheData => RegisterPartClientCacheData;


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

        public void SetItemBaseDataList(List<ItemBaseData> itemBaseList)
        {
            _itemBaseDataList = new List<ItemBaseData>();
            _itemBaseDataList.AddRange(itemBaseList);
        }
    }
}