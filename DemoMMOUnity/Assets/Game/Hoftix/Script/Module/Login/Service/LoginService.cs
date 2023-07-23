using System;
using DG.Tweening;
using ZJYFrameWork.Common;
using ZJYFrameWork.Constant;
using ZJYFrameWork.Event;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.Core.Model;
using ZJYFrameWork.Net.CsProtocol;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.UserInfo;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Common;
using ZJYFrameWork.UISerializable.Manager;

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
                CommonController.Instance.loadingRotate.OnShow();
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
            netManager.Send(
                GetPlayerInfoRequest.ValueOf(settingManager.GetString(GameConstant.SETTING_LOGIN_TOKEN)));
        }

        public void LoginByAccount()
        {
            LoginCacheData.loginError = false;
            netManager.Send(LoginRequest.ValueOf(LoginCacheData.account, LoginCacheData.password));
        }

        public void Logout()
        {
            // throw new System.NotImplementedException();
        }

        public void LoginTapToStart()
        {
            var startData = LoginTapToStartRequest.ValueOf();
            netManager.Send(startData);
        }

        public void GetServerGameMainInfo()
        {
            var UID = SpringContext.GetBean<PlayerUserCaCheData>().Uid;
            var service = GameMainUserToInfoRequest.ValueOf(UID);
        }
    }
}