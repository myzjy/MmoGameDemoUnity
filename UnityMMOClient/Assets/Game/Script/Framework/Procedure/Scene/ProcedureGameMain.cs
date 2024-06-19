using FrostEngine;
using ZJYFrameWork.Event;
using ZJYFrameWork.Module.ServerConfig.Service;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.XLuaScript;

namespace ZJYFrameWork.Procedure.Scene
{
    public class ProcedureGameMainConfig
    {
        public const string GameMainEnter = "ProcedureGameMain:GameMainEnter()";
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
            Debug.Log($"调用 {ProcedureGameMainConfig.GameMainEnter}");
            // SpringContext.GetBean<L>()
#if ENABLE_LUA_START
            SpringContext.GetBean<XLuaManager>().CallLuaFunction("ProcedureGameMain.GameMainEnter()");
#else
            //初始进入
            //获取配置
            SpringContext.GetBean<IServerConfigService>().SendServerConfigService();
#endif
        }
    }
}