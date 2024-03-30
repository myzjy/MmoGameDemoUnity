---@class BagNetContorller
local BagNetController = class("BagNetContorller")
---@type BagNetContorller
local instance = nil
function BagNetController:ctor()
    self.msg = {}
end

function BagNetController.GetInstance()
    if not instance then
        instance = BagNetController()
    end
    return instance
end

--- 注册事件
function BagNetController:RegisterEvent()
    self.msg = {}
    self.msg["c002"] = handle(self.AtBagHeaderWeaponBtnServiceHandler, self)
    UIUtils.AddEventListener(GameEvent.AtBagHeaderBtnService, self.AtBagHeaderBtnHandlerServer, self)
    UIUtils.AddEventListener(GameEvent.ClickBagHeaderBtnHandlerServer, self.ClickBagHeaderBtnHandlerServer, self)
end

function BagNetController:ClickBagHeaderBtnHandlerServer(type, msgProtocol)
    if AllBagItemRequest == nil then
        return
    end
    local packet = AllBagItemRequest(type, msgProtocol)
    local jsonStr = packet:write()
    NetManager:SendMessageEvent(jsonStr)
end

function BagNetController:AtBagHeaderWeaponBtnServiceHandler(packet)
    BagUIController.BagUIView:OpenWeaponPanel(packet)
end

---  头部 按钮 点击事件回调相关
---@param packet AllBagItemResponse
function BagNetController:AtBagHeaderBtnHandlerServer(packet)
    self.msg[packet.protocolStr](packet)
end

return BagNetController
