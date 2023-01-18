using ZJYFrameWork.Net;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Hoftix.Script.Module.Bag.Service
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

        public void GetBagServerData()
        {
        }
    }
}