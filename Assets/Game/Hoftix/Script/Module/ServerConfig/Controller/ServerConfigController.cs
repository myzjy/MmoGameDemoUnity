using ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.ServerConfig;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Module.ServerConfig.Controller
{
    [Bean]
    public class ServerConfigController : IServerConfigController
    {
        [PacketReceiver]
        public void AtServerConfigResponse(ServerConfigResponse response)
        {
            SpringContext.GetBean<ServerDataManager>().SetItemBaseDataList(response.bagItemEntityList);
            //设置为1
            SpringContext.GetBean<LoadUIController>().SetNowProgressNum(1);
            UIComponentManager.DispatchEvent(UINotifEnum.CLOSE_LOADING_UIPAENL);

            UIComponentManager.DispatchEvent(UINotifEnum.OPEN_GAMEMAIN_PANEL);
        }
    }
}