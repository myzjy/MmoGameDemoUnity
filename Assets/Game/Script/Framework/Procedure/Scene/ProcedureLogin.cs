using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Procedure.Scene
{
    [Bean]
    public class ProcedureLogin:FsmState<IProcedureFsmManager>
    {
        public override void OnEnter(IFsm<IProcedureFsmManager> fsm)
        {
            base.OnEnter(fsm);
        }
    }
}