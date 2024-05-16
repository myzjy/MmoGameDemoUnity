---@class BagUIController:LuaUIObject
local BagUIController = class("BagUIController", LuaUIObject())
local BagHeaderBtnPanel = require("application.app.ui.bag.bagHeaderBtnPanel")

---@type BagUIController
local instance = nil
function BagUIController:ctor()
    self:RegisterEvent()
end

function BagUIController.GetInstance()
    if not instance then
        -- body
        instance = BagUIController()
    end
    return instance
end

function BagUIController:Open()
    if self.bagUIView == nil then
        if Debug > 0 then
            PrintError("BagUIView 并未打开界面 生成")
        end
        local bagConfig = UIConfig.FishConfig.BagUIConfig
        -- 去生成界面
        local view = require(bagConfig.scriptPath)
        self.bagUIView = view();
        self.bagUIView:OnLoad();
        return
    end
    self.bagUIView:OnHide()
    self.bagUIView.UIView:OnShow()
    self.bagUIView:OnShow()
end

function BagUIController:Build(bagUIView)
    ---@type BagUIView
    self.bagUIView = bagUIView
end

function BagUIController:RegisterEvent()
    UIUtils.AddEventListener(UIGameEvent.BagHeaderWeaponBtnHandler, self.OpenWeaponPanel, self)
end
--- 打开 刷新武器
function BagUIController:OpenWeaponPanel()
    self.bagUIView:OpenWeaponPanel()
end
--- 刚打开界面的时候，发送那些协议
function BagUIController:InitSendServerMessage()
    --- 发送 协议 AllBagItemRequest
    BagNetController:ClickBagHeaderBtnHandlerServer(1, "")

end
return BagUIController