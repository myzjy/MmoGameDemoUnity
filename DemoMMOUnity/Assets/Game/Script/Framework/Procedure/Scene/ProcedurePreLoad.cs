using ZJYFrameWork.I18n;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Procedure.Scene
{
    [Bean]
    public class ProcedurePreLoad : FsmState<IProcedureFsmManager>
    {
        [Autowired]
        private II18nManager i18NManager;
        public override void OnEnter(IFsm<IProcedureFsmManager> fsm)
        {
            base.OnEnter(fsm);
            Debug.LogError("ProcedurePreLoad");
            i18NManager.ParseData("messagedata_cn");
        }

        public override void OnUpdate(IFsm<IProcedureFsmManager> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);


            // fsm.SetData(SceneConstant.NEXT_SCENE_ENUM, SceneEnum.Login);
            fsm.ChangeState<ProcedureLogin>();
        }
    }
}