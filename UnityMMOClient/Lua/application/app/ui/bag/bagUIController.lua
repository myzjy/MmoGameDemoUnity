---@class BagUIController:LuaUIObject
local BagUIController = class("BagUIController", LuaUIObject())
local BagHeaderBtnPanel = require("application.app.ui.bag.bagHeaderBtnPanel")


---@type BagUIController
local instance = nil
function BagUIController:ctor()
end

function BagUIController.GetInstance()
    if not instance then
        -- body
        instance = BagUIController()
    end
    return instance
end

function BagUIController:Open()
    if self.BagUIView == nil then
        if Debug > 0 then
            printError("BagUIView 并未打开界面 生成")
        end
        -- 去生成界面
        DispatchEvent(BagUIConfig.eventNotification.openbaguipanel)
        return
    end
    self.BagUIView:OnHide()
    self.BagUIView.UIView:OnShow()
    self.BagUIView:OnShow()
end

function BagUIController:Build(bagUIView)
    ---@type BagUIView
    self.BagUIView = bagUIView
end

function BagUIController:RegisterEvent()
end

return BagUIController
