---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2023/6/17 13:54
---

---@class UIView:UIBaseView
UIView = class("UIView", UIBaseView)

function UIView:InstanceOrReuse()
    if self.reUse then
        if self.InstanceID == 0 then
            return
        end
        self:ReUse()
    else
        self:SetReUseBool(true)
        self:InstancePrefab()
    end
end

function UIView:LoadUI(viewConfig)
    self:SetViewPanel(viewConfig.viewPanel)
    self:SetConfig(viewConfig)
end

function UIView:InstancePrefab()
    ---打开loading 界面
    CommonController.Instance.loadingRotate:OnShow()
    CS.ZJYFrameWork.Common.CommonManager.Instance:LoadAsset(self.Config.prefabName, function(res)
        self:InstantiateGameObject(res)
        CommonController.Instance.loadingRotate:OnClose()
        self.viewPanel:Init(self.gameObject)
        --printDebug(self.viewPanel.name)
        self:OnInit()
        self:OnShow()
    end)
end
