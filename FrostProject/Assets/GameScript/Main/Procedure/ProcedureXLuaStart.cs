using FrostEngine;
using ProcedureOwner = FrostEngine.IFsm<FrostEngine.IProcedureManager>;

namespace GameMain
{
    public class ProcedureXLuaStart : ProcedureBase
    {
        public override bool UseNativeDialog => true;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            var xluaModelue = GameModule.XLuaModule;
            xluaModelue.InitLuaEnv();
        }
    }
}