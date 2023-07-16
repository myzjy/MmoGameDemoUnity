﻿using ZJYFrameWork.Hotfix.UI.GameMain;
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
            CommonController.Instance.snackbar.OpenUIDataScenePanel(1, 1);
            SpringContext.GetBean<LoginUIController>().OnHide();
            SpringContext.GetBean<GameMainUIController>().OnShow();
        }
    }
}