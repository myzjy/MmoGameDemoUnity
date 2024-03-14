using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.Module.Login.Service;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Common;

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
#if HTTP_SEND_OPEN
            CommonController.Instance.snackbar.CloseUIDataLoadingPanel();
            UIComponentManager.DispatchEvent(UINotifEnum.OpenLoginUI);
#else
            CommonController.Instance.snackbar.OpenUIDataScenePanel(0, 1);
            loginService.ConnectToGateway();
#endif
        }
    }
}