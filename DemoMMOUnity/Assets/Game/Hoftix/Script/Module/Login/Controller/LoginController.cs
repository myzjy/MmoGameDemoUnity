using DG.Tweening;
using ZJYFrameWork.Common;
using ZJYFrameWork.Constant;
using ZJYFrameWork.Event;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.Module.Login.Service;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.Core.Model;
using ZJYFrameWork.Net.CsProtocol;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Procedure.Scene;
using ZJYFrameWork.Scheduler.Model;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Common;
using ZJYFrameWork.UISerializable.Manager;

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
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("[user:{}]登录[token:{}][uid:{}]", userName, token, uid);
#endif
            settingManager.SetString(GameConstant.SETTING_LOGIN_TOKEN, token);
            // settingManager.Set(GameConstant.SETTING_LOGIN_TOKEN_USERID, uid);
            SpringContext.GetBean<PlayerUserCaCheData>().Uid = uid;
            SpringContext.GetBean<PlayerUserCaCheData>().userName = userName;
            //只是关闭 输入账号
            SpringContext.GetBean<Hotfix.UISerializable.LoginController>().OnHide();
            SpringContext.GetBean<Hotfix.UISerializable.LoginController>().loginTapToStartView.Show();

            // UIComponentManager.DispatchEvent(UINotifEnum.OpenLoginTapToStartUI);
            // UIComponentManager.DispatchEvent(UINotifEnum.ShowLoginAccountUI);
        }

        [PacketReceiver]
        public void AtPong(Pong pong)
        {
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
                UIComponentManager.CSDispatchEvent(UINotifEnum.OpenLoginUI);
                //loginService.LoginByToken();
            }
            else
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log("连接成功事件，通过账号密码登录服务器");
#endif
                //没有登录过，没有记录
                UIComponentManager.CSDispatchEvent(UINotifEnum.OpenLoginUI);
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
            netManager.Send(Ping.ValueOf());
        }
    }
}