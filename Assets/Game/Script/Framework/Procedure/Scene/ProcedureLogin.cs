using ZJYFrameWork.Constant;
using ZJYFrameWork.Module.Login.Service;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

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
            // if (settingManager.HasSetting(GameConstant.SETTING_LOGIN_TOKEN))
            // {
            //先链接服务器
                loginService.ConnectToGateway();
            // }
            // else
            // {
                // UIComponentManager.DispatchEvent(UINotifEnum.OPEN_LOGIN_UI);
            // }
        }
    }
}