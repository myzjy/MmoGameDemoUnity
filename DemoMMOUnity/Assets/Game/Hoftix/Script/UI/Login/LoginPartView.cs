using DG.Tweening;
using GameUtil;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.Common;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Hotfix.UISerializable;
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
            account.text = "";
            //密码输入框
            password = GetObjType<InputField>("password");
            password.text = "";
            RegisterBtn.SetListener(() =>
            {
                Hide();
                SpringContext.GetBean<LoginUIController>().registerPartRegisterPartView.OnShow();
            });
            // LoginBtn.onClick.RemoveAllListeners();
            LoginBtn.onClick.AddListener(() => { ClickLogin(); });
        }

        private long clickLoginTime;
        public static readonly long CLICK_INTERVAL = 2 * DateTimeUtil.NANO_PER_SECOND;

        /// <summary>
        /// 登录事件 第一次登录或者登录其他账号的时候
        /// </summary>
        private void ClickLogin()
        {
            // if (DateTimeUtil.CurrentTimeMillis() - clickLoginTime < CLICK_INTERVAL)
            // {
            //     return;
            // }
            //
            // clickLoginTime = DateTimeUtil.Now() / 1_0000;

            Debug.Log("账号密码登录[account:{}][password:{}]", account.text, password.text);
            var accountString = account.text;
            var passwordString = password.text;
            SpringContext.GetBean<ServerDataManager>().SetCacheAccountAndPassword(accountString, passwordString);
            SpringContext.GetBean<ILoginService>().LoginByAccount();
        }

        public void Show()
        {
            LoginPart.transform.DOKill();
            LoginPart.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.3f).OnComplete(() =>
            {
                account.text = "";
                password.text = "";
                LoginPart.SetActive(true);
            });
            // LoginPart
        }

        //在我注册了，把登录的相关数据都赋值给输入框 只有注册成功之后才会调用
        public void OnRegisterStartLogin()
        {
            account.text = SpringContext.GetBean<RegisterPartClientCacheData>().Account;
            password.text = SpringContext.GetBean<RegisterPartClientCacheData>().Password;
        }

        public void Hide()
        {
            LoginPart.transform.DOKill();
            LoginPart.transform.DOScale(0f, 0f);
            LoginPart.SetActive(false);
        }
    }
}