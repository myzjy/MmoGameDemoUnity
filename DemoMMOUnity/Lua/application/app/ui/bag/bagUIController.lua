---@class BagUIController:LuaUIObject
BagUIController = class("BagUIController", LuaUIObject)
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

end


function BagUIController:Build(bagUIView)
    ---@type BagUIView
    self.BagUIView = bagUIView
end

