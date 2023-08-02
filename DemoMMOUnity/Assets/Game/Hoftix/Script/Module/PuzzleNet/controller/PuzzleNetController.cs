using ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Map;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Module.PuzzleNet.controller
{
    /// <summary>
    /// 地图 Puzzle Net 返回 controller
    /// </summary>
    [Bean]
    public class PuzzleNetController
    {
        [Autowired]
        private ServerDataManager _serverDataManager;
        /// <summary>
        /// 地图 Puzzle 配置表 请求 返回（Response）
        /// </summary>
        /// <param name="response"></param>
        [PacketReceiver]
        public void AtPuzzleAllConfigResponse(PuzzleAllConfigResponse response)
        {
            _serverDataManager.SetPuzzleConfigList(response.PuzzleList);
            //请求结束了 打开界面
        }
    }
}