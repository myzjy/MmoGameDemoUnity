using ZJYFrameWork.Hotfix.UI.BagUI;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.CsProtocol.Protocol.Bag.BagServer;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Hotfix.Script.Module.Bag.Service
{
    /// <summary>
    /// 背包Service 具体实现
    /// </summary>
    [Bean]
    public class BagServerService : IBagServerService
    {
        /// <summary>
        /// 网络相关
        /// </summary>
        [Autowired] private INetManager _netManager;

        /// <summary>
        /// 获取背包
        /// </summary>
        /// <param name="bagType"></param>
        public void GetBagServerData(OpenBagType bagType)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[BagServerService] [GetBagServerData] [bagType:{bagType}]");
#endif
            _netManager.Send(AllBagItemRequest.ValueOf());
        }
    }
}