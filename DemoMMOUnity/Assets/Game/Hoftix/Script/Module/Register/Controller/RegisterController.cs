using ZJYFrameWork.Common;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.I18n;
using ZJYFrameWork.Net.CsProtocol;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;

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
    }
}