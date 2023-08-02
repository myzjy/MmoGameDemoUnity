// ReSharper disable once CheckNamespace

using ZJYFrameWork.Net;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Map;
using ZJYFrameWork.Spring.Core;
using NotImplementedException = System.NotImplementedException;

namespace ZJYFrameWork.Module.PuzzleNet.service
{
    /// <summary>
    ///  地图的网络请求配置 相关 service 具体实现
    /// </summary>
    [Bean]
    public class PuzzleNetService : IPuzzleNetService
    {
        [Autowired] private INetManager _netManager;

        public void GetTheMapTotalDataTableService(int eventID)
        {
            var packet = ProtocolManager.GetProtocol(1035);
            var data = (PuzzleAllConfigRequest)packet.Read();
            data.eventId = eventID;
            _netManager.Send(data);
        }
    }
}