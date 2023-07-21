using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Hotfix.Common
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

        private int lv { get; set; }
        private int maxLv { get; set; }
        private int exp { get; set; }
        private int maxExp { get; set; }

        public void SetMaxExp(int gMaxExp)
        {
            maxExp = gMaxExp;
        }

        public int GetMaxExp()
        {
            return maxExp;
        }

        public void SetLv(int mLv)
        {
            lv = mLv;
        }

        public int GetLv()
        {
            return lv;
        }

        public void SetMaxLv(int gMaxLv)
        {
            this.maxLv = gMaxLv;
        }

        public int GetMaxLv()
        {
            return maxLv;
        }

        public void SetExp(int mExp)
        {
            exp = mExp;
        }

        public int GetExp()
        {
            return exp;
        }
    }
}