using ZJYFrameWork.Common;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.CsProtocol.Protocol.ServerConfig;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Common;

namespace ZJYFrameWork.Module.ServerConfig.Service
{
    [Bean]
    public class ServerConfigService : IServerConfigService
    {
        [Autowired] private INetManager _netManager;

        public void SendServerConfigService()
        {
            CommonController.Instance.loadingRotate.OnShow();
            _netManager.Send(ServerConfigRequest.ValueOf());
        }
    }
}