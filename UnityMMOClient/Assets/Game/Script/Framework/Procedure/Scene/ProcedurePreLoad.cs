using System.Collections;
using UnityEngine;
using ZJYFrameWork.AssetBundles.AssetBundlesManager;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Config;
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
            CommonController.Instance.isLuaInit = false;
#if ENABLE_LUA_START
            IBundle bundle = null;
            bundle = SpringContext.GetBean<AssetBundleManager>()
                .LoadXLuaAssetBundle(AppConfig.XLuaAssetBundleName, res => { bundle = res; });
            yield return new WaitUntil(() => bundle != null);
            SpringContext.GetBean<XLuaManager>().bundle = bundle;
            SpringContext.GetBean<XLuaManager>().InitLuaEnv();
#endif
            yield return new WaitUntil(() => CommonController.Instance.isLuaInit);

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