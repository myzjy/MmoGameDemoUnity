using System;
using DG.Tweening;
using ZJYFrameWork.Constant;
using ZJYFrameWork.Event;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.Core.Model;
using ZJYFrameWork.Net.CsProtocol.Protocol.Login;
using ZJYFrameWork.Net.CsProtocol.Protocol.UserInfo;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Common;


namespace ZJYFrameWork.Module.Login.Service
{
    [Bean]
    public class LoginService : ILoginService
    {
        [Autowired] private INetManager _netManager;

        [Autowired] private ISettingManager _settingManager;
        [Autowired] private LoginClientCacheData _loginCacheData;

        public void ConnectToGateway()
        {
            try
            {
                CommonController.Instance.loadingRotate.OnShow();
                var webSocketGatewayUrl = SpringContext.GetBean<ISettingManager>().GetWebSocketBase();
                _netManager.Connect(webSocketGatewayUrl);
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
            _netManager.Send(
                GetPlayerInfoRequest.ValueOf(_settingManager.GetString(GameConstant.SETTING_LOGIN_TOKEN)));
        }

        public void LoginByAccount()
        {
            _loginCacheData.loginError = false;
            _netManager.Send(LoginRequest.ValueOf(_loginCacheData.account, _loginCacheData.password));
        }

        public void Logout()
        {
            // throw new System.NotImplementedException();
        }

        public void LoginTapToStart()
        {
            var startData = LoginTapToStartRequest.ValueOf();
            _netManager.Send(startData);
        }

        public void GetServerGameMainInfo()
        {
            var uid = SpringContext.GetBean<PlayerUserCaCheData>().Uid;
            var service = GameMainUserToInfoRequest.ValueOf(uid);
            _netManager.Send(service);
        }
    }
}
