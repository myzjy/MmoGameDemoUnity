using UnityEngine;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Procedure.Scene
{
    [Bean]
    public class ProcedureSplash : FsmState<IProcedureFsmManager>
    {
        private bool checkNetworkComplete;
        public override void OnEnter(IFsm<IProcedureFsmManager> fsm)
        {
            base.OnEnter(fsm);

            // 如果没有网络，则直接跳出弹窗返回
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                checkNetworkComplete = false;
            }
            else
            {
                checkNetworkComplete = true;
            }
        }
        public override void OnUpdate(IFsm<IProcedureFsmManager> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);

            if (!checkNetworkComplete)
            {
                return;
            }

            // if (AssetBundleConfig.IsEditorMode)
            // {
                // 编辑器模式
                Debug.Log("Editor resource mode detected.");
                fsm.ChangeState<ProcedurePreLoad>();
            // }
            // else 
            // {
            //     // 单机模式
            //     Debug.Log("Package resource mode detected.");
            //     fsm.ChangeState<ProcedureInitResources>();
            // }
        }
    }
}