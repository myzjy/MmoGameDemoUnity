using ZJYFrameWork.Event;
using ZJYFrameWork.Module.ServerConfig.Service;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Procedure.Scene
{
    public class ProcedureGameMainConfig
    {
        public const string GameMainEnter = "Enter_ProcedureGameMain";
    }

    /// <summary>
    /// 主场景的状态机
    /// </summary>
    [Bean]
    public class ProcedureGameMain : FsmState<IProcedureFsmManager>
    {
        public override void OnEnter(IFsm<IProcedureFsmManager> fsm)
        {
            base.OnEnter(fsm);
            // SpringContext.GetBean<L>()
#if ENABLE_LUA_START
            EventBus.AsyncExecute(ProcedureGameMainConfig.GameMainEnter);
#else
            //初始进入
            //获取配置
            SpringContext.GetBean<IServerConfigService>().SendServerConfigService();
#endif
        }
    }
}