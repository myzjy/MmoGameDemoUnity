---背包 UI 相关 逻辑
---@class BagUIView:UIView
local BagUIView = class("BagUIView", UIView)

local function UIConfig()
    return {
        Config = BagUIConfig,
        viewPanel = BagUIPanelView,
        initFunc = function()
            printDebug("call bagUIView lua scirpt local function UIConfig called initFunc OnInit")
            BagUIView:OnInit()
        end,
        showFunc = function()
            BagUIView:OnShow()
        end,
        hideFunc = function()
            BagModelUIView:OnHideClearCacheDataList()
        end
    }
end
function BagUIView:OnLoad()
    self:Load(UIConfig())
    self:LoadUI(UIConfig())
    self:InstanceOrReuse()
end

function BagUIView:OnInit()
    BagUIController:Build(self)
end
function BagUIView:OnShow()
    BagUIController:Open()
end


function BagUIView:Notification()
    return {
        [BagUIConfig.eventNotification.openbaguipanel] = BagUIConfig.eventNotification
            .openbaguipanel,
        [BagUIConfig.eventNotification.closebaguipanel] = BagUIConfig.eventNotification
            .closebaguipanel
    }
end

--- UI 通信 消息机制
local eventSwitch = {
    [BagUIConfig.eventNotification.openbaguipanel] = function()
        if BagUIView.reUse then
            BagUIView:InstanceOrReuse()
        else
            BagUIView:OnLoad()
        end
    end,
    [BagUIConfig.eventNotification.closebaguipanel] = function(obj)
        BagUIView:OnHide()
    end
}
function BagUIView:NotificationHandler(_eventNotification)
    local switchAction = eventSwitch[_eventNotification.eventName]
    if eventSwitch then
        return switchAction(_eventNotification.eventBody)
    end
end

return BagUIView
