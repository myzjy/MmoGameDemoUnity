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
            // CommonController.Instance.snackbar.OpenUIDataLoadingPanel("", 0, 1);

            loginService.ConnectToGateway();
        }
    }
}