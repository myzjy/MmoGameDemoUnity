using DG.Tweening;
using Tools.Util;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.Module.Login.Service;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
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

        /// <summary>
        /// 账号输入框
        /// </summary>
        public InputField account;

        /// <summary>
        /// 密码输入框
        /// </summary>
        public InputField password;

        public void Build()
        {
            //登录面板
            LoginPart = GetObjType<GameObject>("LoginPart");
            //登录-->账号密码正确进入游戏
            LoginBtn = GetObjType<Button>("LoginBtn_Button");
            //注册--->进入注册界面，进行账号注册
            RegisterBtn = GetObjType<Button>("RegisterBtn_Button");
            //账号输入框
            account = GetObjType<InputField>("account");
            //密码输入框
            password = GetObjType<InputField>("password");
            RegisterBtn.SetListener(() =>
            {
                Hide();
                SpringContext.GetBean<LoginController>().registerPartRegisterPartView.OnShow();
            });
            // LoginBtn.onClick.RemoveAllListeners();
            LoginBtn.onClick.AddListener(() => { ClickLogin(); });
        }

        private long clickLoginTime;
        public static readonly long CLICK_INTERVAL = 5 * DateTimeUtil.NANO_PER_SECOND;

        /// <summary>
        /// 登录事件 第一次登录或者登录其他账号的时候
        /// </summary>
        private void ClickLogin()
        {
            if (DateTimeUtil.CurrentTimeMillis() - clickLoginTime < CLICK_INTERVAL)
            {
                return;
            }

            Debug.Log("账号密码登录[account:{}][password:{}]", account.text, password.text);
            var accountString = account.text;
            var passwordString = password.text;
            SpringContext.GetBean<ServerDataManager>().SetCacheAccountAndPassword(accountString, passwordString);
            SpringContext.GetBean<ILoginService>().LoginByAccount();
        }

        public void Show()
        {
          
            LoginPart.transform.DOKill();
            LoginPart.transform.DOScale(1f, 1f).SetEase(Ease.OutBack).SetDelay(0.2f * 0).OnComplete(() =>
            {
                LoginPart.SetActive(true);
            });
            // LoginPart
        }

        public void Hide()
        {
            LoginPart.transform.DOKill();
            LoginPart.transform.DOScale(0f, 0f);
            LoginPart.SetActive(false);
        }
    }
}