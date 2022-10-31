using UnityEngine;
using ZJYFrameWork.Constant;
using ZJYFrameWork.Module.Login.Service;
using ZJYFrameWork.Net;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Manager;
using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.Procedure.Scene
{
    [Bean]
    public class ProcedureLogin : FsmState<IProcedureFsmManager>
    {
        [Autowired] private ISettingManager settingManager;
        [Autowired] private ILoginService loginService;

        public override void OnEnter(IFsm<IProcedureFsmManager> fsm)
        {
            base.OnEnter(fsm);
            SpringContext.GetBean<NetworkManager>().SetUserFromAttributeData(0,
                SystemInfo.deviceUniqueIdentifier.Substring(0, 8) + UnityEngine.Random.Range(1, 9999999),
                "unityEditor");
            // if (settingManager.HasSetting(GameConstant.SETTING_LOGIN_TOKEN))
            // {
            //先链接服务器
            RegisterApi registerApi = new RegisterApi();
                
            // registerApi.Param. = getUniqueDeviceType();
            registerApi.Param.version = "1.00.001";
            registerApi.Param.channelCode = SpringContext.GetBean<NetworkManager>().aUserFromAttr.channelCode.ToString();
            registerApi.Param.platformId = SpringContext.GetBean<NetworkManager>().aUserFromAttr.platformId;
            registerApi.Param.platfromToken = SpringContext.GetBean<NetworkManager>().aUserFromAttr.sdkToken;
            registerApi.onBeforeSend = () =>
            {
                // CommonUIManager.Instance.UINetLoading.OnShow();
            };
            registerApi.onSuccess = res =>
            {
                //注册成功
                // EventBus.AsyncSubmit(NetLoginHttpApiEvent.ValueOf());
            };
            registerApi.onComplete = () =>
            {
                // CommonUIManager.Instance.UINetLoading.OnClose();
            };
            registerApi.onError = res =>
            {
                
            };
            SpringContext.GetBean<NetworkManager>().Request(registerApi);    
            // loginService.ConnectToGateway();
            // }
            // else
            // {
                // UIComponentManager.DispatchEvent(UINotifEnum.OPEN_LOGIN_UI);
            // }
        }
    }
}