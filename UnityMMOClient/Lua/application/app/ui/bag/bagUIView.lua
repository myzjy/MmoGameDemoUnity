---背包 UI 相关 逻辑
---@class BagUIView:UIView
local BagUIView = class("BagUIView", UIView)
local BagHeaderBtnPanel = require("application.app.ui.bag.bagHeaderBtnPanel")
local WeaponUIPanel = require("application.app.ui.bag.bagWeaponUIPanelView")
local bagModelUIView = require("application.app.ui.bag.modelView.bagModelUIView")

local function UIConfig()
    return {
        Config = UIConfigEnum.FishConfig.BagUIConfig,
        viewPanel = BagUIPanelView,
        initFunc = function()
            PrintDebug("call bagUIView lua scirpt local function UIConfig called initFunc OnInit")
            BagUIView:OnInit()
        end,
        showFunc = function()
            PrintDebug("call bagUIView lua scritpt lcoal func show")
            BagUIView:OnShow()
        end,
        hideFunc = function()
            BagUIView.BagModelUIView:OnHideClearCacheDataList()
        end
    }
end
function BagUIView:ctor()
    self.bagHeaderBtnPanel = nil
    self:RegisterEvent()
end

function BagUIView:OnLoad()
    self:Load(UIConfig())
    self:LoadUI(UIConfig())
    self:InstanceOrReuse()
end

function BagUIView:OnInit()
    ---@type BagUIPanelView
    local view = self.viewPanel
    self.BagModelUIView = bagModelUIView()
    self.weaponUIView = WeaponUIPanel(view)
    self.bagHeaderBtnPanel = BagHeaderBtnPanel(self.gameObject);
    BagUIController:InitSendServerMessage()
end

function BagUIView:OnShow()

end

function BagUIView:RegisterEvent()
    UIUtils.AddEventListener(UIGameEvent.BagHeaderWeaponBtnHandler, self.OpenWeaponPanel, self)
    -- UIUtils.AddEventListener(GameEvent.AtBagHeaderWeaponBtnServiceHandler, self.OpenWeaponPanel, self)
end

function BagUIView:CreateWeaponListPanel()
end

function BagUIView:OpenWeaponPanel()
    local weaponInfo = BagNetController:GetWeaponUserEntityList()


end

return BagUIView
