﻿using DG.Tweening;
using ZJYFrameWork.Common;
using ZJYFrameWork.Constant;
using ZJYFrameWork.Event;
using ZJYFrameWork.Module.Login.Service;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.Core.Model;
using ZJYFrameWork.Net.CsProtocol;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Scheduler.Model;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Manager;
using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.Script.Module.Login.Controller
{
    /// <summary>
    /// 登录控制器
    /// </summary>
    [Bean]
    public class LoginController
    {
        [Autowired] private ISettingManager settingManager;

        [Autowired] private INetManager netManager;
        [Autowired] private ILoginService loginService;
        [Autowired] private LoginClientCacheData LoginCacheData;
        private int reconnectCount = 0;

        [PacketReceiver]
        public void AtLoginResponse(LoginResponse response)
        {
            var token = response.token;
            LoginCacheData.loginFlag = true;
            Debug.Log("登录返回[token:{}]", token);
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
            if (settingManager.HasSetting(GameConstant.SETTING_LOGIN_TOKEN))
            {
                Debug.Log("连接成功事件，通过Token登录服务器");
                loginService.LoginByToken();
            }
            else
            {
                Debug.Log("连接成功事件，通过账号密码登录服务器");
                RegisterApi registerApi = new RegisterApi
                {
                    Param =
                    {
                        // registerApi.Param. = getUniqueDeviceType();
                        version = "1.00.001",
                        channelCode = SpringContext.GetBean<NetworkManager>().aUserFromAttr.channelCode,
                        platformId = SpringContext.GetBean<NetworkManager>().aUserFromAttr.platformId,
                        platfromToken = SpringContext.GetBean<NetworkManager>().aUserFromAttr.sdkToken
                    },
                    onBeforeSend = () =>
                    {
                        // CommonUIManager.Instance.UINetLoading.OnShow();
                    },
                    onSuccess = res =>
                    {
                
                    },
                    onComplete = () =>
                    {
                        // CommonUIManager.Instance.UINetLoading.OnClose();
                    },
                    onError = res =>
                    {
                
                    }
                };

                SpringContext.GetBean<NetworkManager>().Request(registerApi);
                // loginService.LoginByAccount();
                //没有登录过，没有记录
                // UIComponentManager.DispatchEvent(UINotifEnum.OPEN_LOGIN_UI);
            }
        }

        [EventReceiver]
        public void OnNetErrorEvent(NetErrorEvent errorEvent)
        {
            reconnectCount++;
            var sequence = DOTween.Sequence();
            sequence.OnComplete(() =>
            {
                Debug.LogError($"无法链接网络，正在重试");
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