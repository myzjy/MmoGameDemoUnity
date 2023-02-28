using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Procedure.Scene
{
    [Bean]
    public class ProcedureMain : FsmState<IProcedureFsmManager>
    {
        public override void OnEnter(IFsm<IProcedureFsmManager> fsm)
        {
            base.OnEnter(fsm);
        }
        public override void OnUpdate(IFsm<IProcedureFsmManager> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);

            // 运行一帧即切换到 Splash 展示流程
            fsm.ChangeState<ProcedureSplash>();
        }
    }
}