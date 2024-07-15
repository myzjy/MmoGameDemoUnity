using System;
using XLua;

namespace FrostEngine
{
    [UpdateModule]
    internal sealed partial class SchedulerManager : ModuleImp
    {
        
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        internal override void Shutdown()
        {
            throw new System.NotImplementedException();
        }
        
        
    }
}