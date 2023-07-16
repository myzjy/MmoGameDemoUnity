using ZJYFrameWork.Net;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Core;
using NotImplementedException = System.NotImplementedException;

namespace ZJYFrameWork.Module.PhysicalPower.Service
{
    [Bean]
    public class PhysicalPowerService : IPhysicalPowerService
    {
        [Autowired] private INetManager _netManager;

        public void SendPhysicalPowerUserProps(int userNum)
        {
            var request = PhysicalPowerUserPropsRequest.ValueOf(userNum);
            _netManager.Send(request);
        }
    }
}