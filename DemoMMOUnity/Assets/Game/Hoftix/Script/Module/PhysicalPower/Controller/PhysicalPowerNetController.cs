using ZJYFrameWork.Event;
using ZJYFrameWork.Module.PhysicalPower.Model;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Module.PhysicalPower.Controller
{
    [Bean]
    public class PhysicalPowerNetController
    {
        private INetManager netManager;
        [PacketReceiver]
        public void AtPhysicalPowerUserPropsResponse(PhysicalPowerUserPropsResponse response)
        {
            
        }
        [EventReceiver]
        public void OnPhysicalPowerSecondsEvent(PhysicalPowerSecondsEvent eve)
        {
            // netManager.Send(PhysicalPowerSecondsRequ.ValueOf());
        }
    }
}