﻿using System;
using DG.Tweening;
using ZJYFrameWork.Common;
using ZJYFrameWork.Constant;
using ZJYFrameWork.Event;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.Core.Model;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Module.Login.Service
{
    [Bean]
    public class LoginService : ILoginService
    {
        [Autowired] private INetManager netManager;

        [Autowired] private ISettingManager settingManager;
        [Autowired] private LoginClientCacheData LoginCacheData;

        public void ConnectToGateway()
        {
            try
            {
                var webSocketGatewayUrl = SpringContext.GetBean<ISettingManager>().GetWebSocketBase();
                netManager.Connect(webSocketGatewayUrl);
            }
            catch (Exception e)
            {
                Debug.LogError($"链接网络发生未知异常:{e}");
                var sequence = DOTween.Sequence();
                sequence.AppendInterval(3f);
                sequence.AppendCallback(() => { EventBus.SyncSubmit(NetErrorEvent.ValueOf()); });
            }
        }

        /// <summary>
        /// token
        /// </summary>
        public void LoginByToken()
        {
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                netManager.Send(
                    GetPlayerInfoRequest.ValueOf(settingManager.GetString(GameConstant.SETTING_LOGIN_TOKEN)));
            });
            sequence.AppendInterval(8f);
            sequence.AppendCallback(() =>
            {
                if (!LoginCacheData.loginFlag)
                {
                    LoginByToken();
                }
            });
        }

        public void LoginByAccount()
        {
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                LoginCacheData.loginError = false;
                netManager.Send(LoginRequest.ValueOf(LoginCacheData.account, LoginCacheData.password));
            });
            sequence.AppendInterval(3f);
            sequence.AppendCallback(() =>
            {
                if (LoginCacheData.loginFlag)
                {
                    return;
                }
                if (LoginCacheData.loginError)
                {
                    return;
                }

                LoginByAccount();
            });
        }

        public void Logout()
        {
            throw new System.NotImplementedException();
        }
    }
}