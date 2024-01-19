using System;
using DG.Tweening;
using ZJYFrameWork.Common;
using ZJYFrameWork.Constant;
using ZJYFrameWork.Event;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Hotfix.UI.GameMain;
using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.Module.Login.Service;
using ZJYFrameWork.Module.PhysicalPower.Service;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.Core.Model;
using ZJYFrameWork.Net.CsProtocol;
using ZJYFrameWork.Net.CsProtocol.Protocol.Login;
using ZJYFrameWork.Net.CsProtocol.Protocol.UserInfo;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Scheduler;
using ZJYFrameWork.Scheduler.Model;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UI.UIModel;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Common;
using ZJYFrameWork.UISerializable.Manager;
using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.Hotfix.Module.Login.Controller
{
    /// <summary>
    /// 登录控制器
    /// </summary>
    [Bean]
    public class LoginController
    {
        [Autowired] private LoginClientCacheData LoginCacheData;
        [Autowired] private ILoginService loginService;

        [Autowired] private INetManager netManager;
        private int reconnectCount = 0;
        [Autowired] private ISettingManager settingManager;

        [PacketReceiver]
        public void AtLoginResponse(LoginResponse response)
        {
            var token = response.token;
            var uid = response.uid;
            var userName = response.userName;
            LoginCacheData.loginFlag = true;
            SpringContext.GetBean<PlayerUserCaCheData>().userName = response.userName;
            SpringContext.GetBean<PlayerUserCaCheData>().Uid = response.uid;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("[user:{}]登录[token:{}][uid:{}]", userName, token, uid);
#endif
            settingManager.SetString(GameConstant.SETTING_LOGIN_TOKEN, token);

            //只是关闭 输入账号
            SpringContext.GetBean<LoginUIController>().OnHide();
            SpringContext.GetBean<LoginUIController>().loginTapToStartView.Show();
        }


        [PacketReceiver]
        public void AtPong(Pong pong)
        {
            SpringContext.GetBean<SchedulerManager>().serverTime = pong.time;
            // 设置一下服务器的最新时间
            DateTimeUtil.SetNow(pong.time);
        }

        [EventReceiver]
        public void OnNetOpenEvent(NetOpenEvent openEvent)
        {
            reconnectCount = 0;
            LoginCacheData.loginFlag = false;
            CommonController.Instance.snackbar.OpenUIDataScenePanel(1, 1);
            CommonController.Instance.loadingRotate.OnClose();
            if (settingManager.HasSetting(GameConstant.SETTING_LOGIN_TOKEN))
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log("连接成功事件，通过Token登录服务器");
#endif
                UIComponentManager.DispatchEvent(UINotifEnum.OpenLoginUI);
                //loginService.LoginByToken();
            }
            else
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log("连接成功事件，通过账号密码登录服务器");
#endif
                //没有登录过，没有记录
                UIComponentManager.DispatchEvent(UINotifEnum.OpenLoginUI);
            }
        }

        [EventReceiver]
        public void OnNetErrorEvent(NetErrorEvent errorEvent)
        {
            reconnectCount++;
            var sequence = DOTween.Sequence();
            sequence.OnComplete(() =>
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.LogError($"无法链接网络，正在重试");
#endif
                CommonController.Instance.snackbar.SeverError("网络连接错误");

                // var errorMessage = StringUtils.Format(i18nManager.GetString(I18nEnum.connection_error_and_reconnect.ToString(), reconnectCount));
                // CommonController.GetInstance().snackbar.Error(errorMessage);
            });
            loginService.ConnectToGateway();
        }

        /// <summary>
        /// 请求服务器最新时间
        /// </summary>
        /// <param name="eve"></param>
        [EventReceiver]
        public void OnMinuteSchedulerAsyncEvent(MinuteSchedulerAsyncEvent eve)
        {
            // netManager.Send(Ping.ValueOf());
        }

        [PacketReceiver]
        public void AtLoginTapToStartResponse(LoginTapToStartResponse response)
        {
            if (!string.IsNullOrEmpty(response.message))
            {
#if UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG)
                Debug.Log($"可以登录：{response.accessGame},message:{response.message}");
#endif
            }

            if (!response.accessGame)
            {
                CommonController.Instance.snackbar.OpenCommonUIPanel(Dialog.ButtonType.YesNo, "提示", "当前不在登录时间",
                    res => { }, "确定", "取消");
                return;
            }

            SpringContext.GetBean<LoginUIController>().loginTapToStartView.LoginStartGame();
        }
        
        [PacketReceiver]
        public void AtGameMainUserToInfoResponse(GameMainUserToInfoResponse response)
        {
            LoginCacheData.SetExp(response.GetNowExp());
            LoginCacheData.SetLv(response.GetNowLv());
            LoginCacheData.SetMaxLv(response.GetMaxLv());
            LoginCacheData.SetMaxExp(response.GetMaxExp());
            SpringContext.GetBean<PlayerUserCaCheData>().goldNum = response.GetGoldCoinNum();
            SpringContext.GetBean<PlayerUserCaCheData>().DiamondNum = response.GetDiamondsNum();
            SpringContext.GetBean<PlayerUserCaCheData>().PremiumDiamondNum = response.GetPaidDiamondsNum();
            SpringContext.GetBean<GameMainUIController>().ShowGameMainUserInfoMessage(LoginCacheData);
            SpringContext.GetBean<IPhysicalPowerService>().SendPhysicalPowerRequest();
        }

        public void AtLoginRequest()
        {
            UserAccountLoginApi loginApi = new UserAccountLoginApi
            {
                onBeforeSend = () => { CommonController.Instance.loadingRotate.OnShow(); },
                onComplete = () => { CommonController.Instance.loadingRotate.OnClose(); },
                onSuccess = res =>
                {
                    var token = res.Token;
                    var uid = res.Uid;
                    var userName = res.UserName;
                    SpringContext.GetBean<LoginClientCacheData>().loginFlag = true;
                    SpringContext.GetBean<PlayerUserCaCheData>().userName = res.UserName;
                    SpringContext.GetBean<PlayerUserCaCheData>().Uid = res.Uid;
                    
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.Log("[user:{}]登录[token:{}][uid:{}]", userName, token, uid);
#endif
                    SpringContext.GetBean<SettingManager>().SetString(GameConstant.SETTING_LOGIN_TOKEN, token);
                    SpringContext.GetBean<NetworkManager>().SetAuthToken(token);
                    //只是关闭 输入账号
                    SpringContext.GetBean<LoginUIController>().OnHide();
                    SpringContext.GetBean<LoginUIController>().loginTapToStartView.Show();
                },
                Param =
                {
                    Account = LoginCacheData.account,
                    Password = LoginCacheData.password
                }
            };
            SpringContext.GetBean<NetworkManager>().Request(loginApi);
        }
    }
}