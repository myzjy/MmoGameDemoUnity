using System;
using XLua;

namespace FrostEngine
{
    [UpdateModule]
    internal sealed partial class SchedulerManager : ModuleImp
    {
        NetManager netManager = null;
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (netManager == null)
            {
                netManager = ModuleImpSystem.GetModule<NetManager>();
            }

            if (!netManager.IsConnect())
            {
                return;
            }
            
        }

        internal override void Shutdown()
        {
            throw new System.NotImplementedException();
        }
        
        
    }
}