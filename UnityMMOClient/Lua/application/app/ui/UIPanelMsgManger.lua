---@class UIPanelManager
local UIPanelManager = class("UIPanelManager")

function UIPanelManager:ctor()

end

function UIPanelManager:InstancePrefab(viewConfig)
    CS.ZJYFrameWork.UISerializable.Common.CommonController.Instance.loadingRotate:OnShow()
    CS.ZJYFrameWork.Common.CommonManager.Instance:LoadAsset(viewConfig.prefabName, function(res)
        self:InstantiateGameObject(res)
        CS.ZJYFrameWork.UISerializable.Common.CommonController.Instance.loadingRotate:OnClose()
        local viewPanel = require(viewConfig.scriptPath)
        viewPanel:Init(self.gameObject)
        --PrintDebug(self.viewPanel.name)
        self:OnInit()
        self:OnShow()
    end)
end

return UIPanelManager
