using ZJYFrameWork.Constant;
using ZJYFrameWork.Event;
using ZJYFrameWork.Execution;
using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.Net.Core.Model;
using ZJYFrameWork.Scenes;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Procedure.Scene
{
    public class SceneValue
    {
        public string assetName;
        public SceneEnum SceneEnum;

        public static SceneValue valueOf(SceneEnum sceneEnum, string assetName)
        {
            SceneValue value = new SceneValue
            {
                assetName = assetName,
                SceneEnum = sceneEnum
            };
            return value;
        }
    }

    [Bean]
    public class ProcedureChangeScene : FsmState<IProcedureFsmManager>
    {
        private bool changeSceneComplete;
        private SceneValue sceneEnum;
        [Autowired] private SceneManager sceneManager;

        public override void OnEnter(IFsm<IProcedureFsmManager> fsm)
        {
            base.OnEnter(fsm);
            changeSceneComplete = false;
            sceneEnum = fsm.GetData<SceneValue>(SceneConstant.NEXT_SCENE_ENUM);
            Executors.RunOnCoroutine(sceneManager.FadeAndLoadSceneAsyncNew(sceneEnum.assetName));
        }

        public void ChangeScene(SceneEnum sceneEnum, string assetName)
        {
            changeSceneComplete = false;
            var fsm = SpringContext.GetBean<IProcedureFsmManager>().ProcedureFsm;
            fsm.SetData(SceneConstant.NEXT_SCENE_ENUM, SceneValue.valueOf(sceneEnum, assetName));
            fsm.ChangeState<ProcedureChangeScene>();
        }

        public override void OnUpdate(IFsm<IProcedureFsmManager> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
            if (!changeSceneComplete)
            {
                return;
            }

            fsm.ChangeState(SceneConstant.SCENE_MAP[sceneEnum.SceneEnum]);
        }

        [EventReceiver]
        private void OnLoadSceneSuccessEvent(LoadSceneSuccessEvent eve)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("Load Scene '{}' OK", eve.sceneAssetName);
#endif
            changeSceneComplete = true;
            UIComponentManager.DispatchEvent(UINotifEnum.CloseLoginUI);
            // CommonUIManager.Instance.Snackbar.OpenUIDataScenePanel(1, 1);
        }
    }
}