using System.Collections;
using UnityEngine;
using ZJYFrameWork.Execution;
using ZJYFrameWork.I18n;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Common;
using ZJYFrameWork.XLuaScript;

// using ZJYFrameWork.XLuaScript;

namespace ZJYFrameWork.Procedure.Scene
{
    [Bean]
    public class ProcedurePreLoad : FsmState<IProcedureFsmManager>
    {
        [Autowired] private II18nManager i18NManager;
        private bool isLoad = false;

        public override void OnEnter(IFsm<IProcedureFsmManager> fsm)
        {
            base.OnEnter(fsm);
            Debug.LogError("ProcedurePreLoad");
            i18NManager.ParseData("messagedata_cn");
            Executors.RunOnCoroutine(StartEnumeratorAwake());
        }

        public IEnumerator StartEnumeratorAwake()
        {
            yield return new WaitUntil(() => CommonController.Instance != null);
#if ENABLE_LUA_START
            SpringContext.GetBean<XLuaManager>().InitLuaEnv();
#endif
            isLoad = true;
        }

        public override void OnUpdate(IFsm<IProcedureFsmManager> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);

            if (isLoad)
            {
                // fsm.SetData(SceneConstant.NEXT_SCENE_ENUM, SceneEnum.Login);
                fsm.ChangeState<ProcedureLogin>();
            }
        }
    }
}