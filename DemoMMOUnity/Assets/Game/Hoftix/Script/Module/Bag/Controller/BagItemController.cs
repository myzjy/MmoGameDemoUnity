using ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Bag.BagServer;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Hoftix.Script.Bag.Controller
{
    [Bean]
    public class BagItemController
    {
        [PacketReceiver]
        public void AtAllBagItemResponse(AllBagItemResponse response)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[BagItemController] [AtAllBagItemResponse] 协议号:{response.ProtocolId()}");
#endif
            //发过来消息就代表UI打开了
            
        }
    }
}