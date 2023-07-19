using Unity.VisualScripting;
using UnityEngine.Events;
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
            _gameMainUIController.SetPhysicalPowerText(response.nowPhysicalPower, response.maximumStrength);
        }


        [Autowired]
        public void AtPhysicalPowerSecondsResponse(PhysicalPowerSecondsResponse response)
        {
            //设置值
            _cacheData.SetPhysicalPowerSecondsResponse(response);
            //刷新
            _gameMainUIController.SetPhysicalPowerText(response.nowPhysicalPower, response.maximumStrength);
        }

        private UnityEvent<PhysicalPowerResponse> PhysicalPowerResponseAction;

        public void AddPhysicalPowerResponseAction(UnityAction<PhysicalPowerResponse> res)
        {
            PhysicalPowerResponseAction ??= new UnityEvent<PhysicalPowerResponse>();
            PhysicalPowerResponseAction.AddListener(res);
        }

        public void RemovePhysicalPowerResponseAction(UnityAction<PhysicalPowerResponse> res)
        {
            if (PhysicalPowerResponseAction == null)
            {
                return;
            }
            PhysicalPowerResponseAction.RemoveListener(res);
        }

        public void RemovesPhysicalPowerResponseAction()
        {
            PhysicalPowerResponseAction.RemoveAllListeners();
        }

        /// <summary>
        /// 体力查看 显示
        /// </summary>
        /// <param name="powerResponse"></param>
        [PacketReceiver]
        public void AtPhysicalPowerResponse(PhysicalPowerResponse powerResponse)
        {
            PhysicalPowerResponseAction?.Invoke(powerResponse);
        }


        [EventReceiver]
        public void OnPhysicalPowerSecondsEvent(PhysicalPowerSecondsEvent eve)
        {
            _netManager.Send(PhysicalPowerSecondsRequest.ValueOf(eve.nowTime));
        }
    }
}