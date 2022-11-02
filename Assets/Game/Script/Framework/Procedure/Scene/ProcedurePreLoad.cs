using ZJYFrameWork.I18n;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Procedure.Scene
{
    [Bean]
    public class ProcedurePreLoad : FsmState<IProcedureFsmManager>
    {
        [Autowired]
        private I18nManager i18NManager;
        public override void OnEnter(IFsm<IProcedureFsmManager> fsm)
        {
            base.OnEnter(fsm);
            Debug.LogError("ProcedurePreLoad");
        }

        public override void OnUpdate(IFsm<IProcedureFsmManager> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);


            // fsm.SetData(SceneConstant.NEXT_SCENE_ENUM, SceneEnum.Login);
            fsm.ChangeState<ProcedureLogin>();
        }
    }
}