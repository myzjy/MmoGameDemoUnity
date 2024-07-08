using System;
using Cysharp.Threading.Tasks;
using FrostEngine;

namespace GameMain
{
    public class ProcedureStartGame : ProcedureBase
    {
        public override bool UseNativeDialog { get; }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            StartGame().Forget();
            ChangeState<ProcedureXLuaStart>(procedureOwner);
        }

        private async UniTaskVoid StartGame()
        {
            await UniTask.Yield();
            UILoadMgr.HideAll();
        }
    }
}