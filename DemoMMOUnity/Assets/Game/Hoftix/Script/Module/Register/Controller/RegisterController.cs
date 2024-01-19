using ZJYFrameWork.Common;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.I18n;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.CsProtocol.Protocol.Login;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Common;
using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.Module.Register.Controller
{
    [Bean]
    public class RegisterController
    {
        [Autowired] private II18nManager i18nManager;

        [Autowired] private ISettingManager settingManager;
        [Autowired] private RegisterPartClientCacheData RegisterCacheData;

        [PacketReceiver]
        public void AtRegisterResponse(RegisterResponse response)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("AtRegisterResponse");
#endif
            //重新打开登录面板
            SpringContext.GetBean<LoginUIController>().OnInit();
        }

        public void AtRegisterRequest()
        {
            UserAccountRegisterApi registerApi = new UserAccountRegisterApi
            {
                onBeforeSend = () => { CommonController.Instance.loadingRotate.OnShow(); },
                onComplete = () => { CommonController.Instance.loadingRotate.OnClose(); },
                onSuccess = res =>
                {
                    //重新打开登录面板
                    SpringContext.GetBean<LoginUIController>().OnInit();
                },
                Param =
                {
                    Account = RegisterCacheData.Account,
                    Password = RegisterCacheData.Password,
                    AffirmPassword = RegisterCacheData.AffirmPassword
                }
            };
            SpringContext.GetBean<NetworkManager>().Request(registerApi);
        }
    }
}