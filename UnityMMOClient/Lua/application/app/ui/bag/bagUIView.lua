---背包 UI 相关 逻辑
---@class BagUIView:UIView
local BagUIView = class("BagUIView", UIView)
local BagHeaderBtnPanel = require("application.app.ui.bag.bagHeaderBtnPanel")
local bagModelUIView = require("application.app.ui.bag.modelView.bagModelUIView")

local function UIConfig()
    return {
        Config = UIConfigEnum.BagUIConfig,
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
    self.BagModelUIView = bagModelUIView()
end

function BagUIView:OnLoad()
    self:Load(UIConfig())
    self:LoadUI(UIConfig())
    self:InstanceOrReuse()
end

function BagUIView:OnInit()
    BagUIController:Build(self)
    ---@type BagUIPanelView
    local view = self.viewPanel
    self.bagHeaderBtnPanel = BagHeaderBtnPanel(self.gameObject);
end

function BagUIView:OnShow()

end

function BagUIView:RegisterEvent()
    UIUtils.AddEventListener(GameEvent.AtBagHeaderWeaponBtnServiceHandler, self.OpenWeaponPanel, self)
end

function BagUIView:CreateWeaponListPanel()
end

---@param weaponInfo AllBagItemResponse
function BagUIView:OpenWeaponPanel(weaponInfo)

end

return BagUIView
