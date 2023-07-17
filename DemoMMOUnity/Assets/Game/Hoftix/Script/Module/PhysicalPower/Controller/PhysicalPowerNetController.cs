using ZJYFrameWork.Event;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Hotfix.UI.GameMain;
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
        [Autowired] private GameMainUIController _gameMainUIController;
        [Autowired] private INetManager _netManager;
        [Autowired] private PhysicalPowerCacheData _cacheData;

        [PacketReceiver]
        public void AtPhysicalPowerUserPropsResponse(PhysicalPowerUserPropsResponse response)
        {
            //设置值
            _cacheData.SetPhysicalPowerUserPropsResponse(response);
        }

        [Autowired]
        public void AtPhysicalPowerSecondsResponse(PhysicalPowerSecondsResponse response)
        {
            //设置值
            _cacheData.SetPhysicalPowerSecondsResponse(response);
        }


        [EventReceiver]
        public void OnPhysicalPowerSecondsEvent(PhysicalPowerSecondsEvent eve)
        {
            _netManager.Send(PhysicalPowerSecondsRequest.ValueOf(eve.nowTime));
        }
    }
}