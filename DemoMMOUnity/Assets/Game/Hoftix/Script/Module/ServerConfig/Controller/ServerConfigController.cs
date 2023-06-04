using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.ServerConfig;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Common;

namespace ZJYFrameWork.Module.ServerConfig.Controller
{
    [Bean]
    public class ServerConfigController : IServerConfigController
    {
        [PacketReceiver]
        public void AtServerConfigResponse(ServerConfigResponse response)
        {
            //获取
            SpringContext.GetBean<ServerDataManager>().SetItemBaseDataList(response.bagItemEntityList);
            //设置为1
            CommonController.Instance.snackbar.OpenUIDataScenePanel(1, 1);


            // UIComponentManager.CSDispatchEvent(UINotifEnum.OPEN_GAMEMAIN_PANEL);
        }
    }
}