using ZJYFrameWork.Hotfix.UI.BagUI;

namespace ZJYFrameWork.Hotfix.Script.Module.Bag.Service
{
    /// <summary>
    /// 背包相关Service 接口
    /// </summary>
    public interface IBagServerService
    {
        /// <summary>
        /// 获取背包所有数据
        /// </summary>
        void GetBagServerData(OpenBagType bagType);
    }
}