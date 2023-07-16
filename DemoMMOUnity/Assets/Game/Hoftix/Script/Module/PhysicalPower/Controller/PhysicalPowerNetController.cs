using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Module.PhysicalPower.Controller
{
    [Bean]
    public class PhysicalPowerNetController
    {
        [PacketReceiver]
        public void AtPhysicalPowerUserPropsResponse(PhysicalPowerUserPropsResponse response)
        {
            
        }
    }
}