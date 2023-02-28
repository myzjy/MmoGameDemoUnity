using ZJYFrameWork.Net;
using ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.ServerConfig;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Module.ServerConfig.Service
{
    [Bean]
    public class ServerConfigService : IServerConfigService
    {
        [Autowired] private INetManager netManager;

        public void SendServerConfigService()
        {
            netManager.Send(ServerConfigRequest.ValueOf());
        }
    }
}