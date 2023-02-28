using ZJYFrameWork.Module.ServerConfig.Service;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Procedure.Scene
{
    /// <summary>
    /// 主场景的状态机
    /// </summary>
    [Bean]
    public class ProcedureGameMain : FsmState<IProcedureFsmManager>
    {
        public override void OnEnter(IFsm<IProcedureFsmManager> fsm)
        {
            base.OnEnter(fsm);
            //初始进入
            //获取配置
            SpringContext.GetBean<IServerConfigService>().SendServerConfigService();
        }
    }
}