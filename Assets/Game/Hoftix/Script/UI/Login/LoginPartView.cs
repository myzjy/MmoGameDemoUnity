using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.UISerializable.Manager;

namespace ZJYFrameWork.UISerializable
{
    /// <summary>
    /// 登录面板
    /// </summary>
    public sealed class LoginPartView : UISerializableKeyObject
    {
        /// <summary>
        /// 登录界面
        /// </summary>
        public GameObject LoginPart;

        /// <summary>
        /// 登录活动
        /// </summary>
        public Button LoginBtn;

        /// <summary>
        /// 注册活动
        /// </summary>
        public Button RegisterBtn;

        public void Bind()
        {
            //登录面板
            LoginPart = GetObjType<GameObject>("LoginPart");
            //登录-->账号密码正确进入游戏
            LoginBtn = GetObjType<Button>("LoginBtn_Button");
            //注册--->进入注册界面，进行账号注册
            RegisterBtn = GetObjType<Button>("RegisterBtn_Button");
        }
        private long clickLoginTime;
        public static readonly long CLICK_INTERVAL = 5 * DateTimeUtil.NANO_PER_SECOND;

        /// <summary>
        /// 登录事件
        /// </summary>
        private void ClickLogin()
        {
            if (DateTimeUtil.CurrentTimeMillis() - clickLoginTime < CLICK_INTERVAL)
            {
                return;
            }
        }

        public void Show()
        {
            LoginPart.SetActive(true);
        }

        public void Hide()
        {
            LoginPart.SetActive(false);
        }
    }
}