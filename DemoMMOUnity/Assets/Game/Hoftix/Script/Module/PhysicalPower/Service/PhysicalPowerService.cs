using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Module.PhysicalPower.Controller;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.CsProtocol.Protocol.PhysicalPower;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Module.PhysicalPower.Service
{
    [Bean]
    public class PhysicalPowerService : IPhysicalPowerService
    {
        [Autowired] private PlayerUserCaCheData _caCheData;
        [Autowired] private INetManager _netManager;

        [Autowired]
        private PhysicalPowerNetController _physicalPowerNetController;
        public void SendPhysicalPowerUserProps(int userNum)
        {
            var request = PhysicalPowerUserPropsRequest.ValueOf(userNum);
            _netManager.Send(request);
        }

        public void SendPhysicalPowerSecondsRequest(long nowTime)
        {
            var request = PhysicalPowerSecondsRequest.ValueOf(nowTime);
            _netManager.Send(request);
        }

        public void SendPhysicalPowerRequest()
        {
            var uid = _caCheData.Uid;
            var request = PhysicalPowerRequest.ValueOf(uid);
            _netManager.Send(request);
        }
    }
}