---@class BagNetContorller
local BagNetContorller = class("BagNetContorller")
---@type BagNetContorller
local instance = nil
function BagNetContorller:ctor()
end

function BagNetContorller.GetInstance()
    if not instance then
        instance = BagNetContorller()
    end
    return instance
end

function BagNetContorller:RegisterEvent()
    UIUtils.AddEventListener(GameEvent.ClickBagHeaderBtnHandlerServer, self.ClickBagHeaderBtnHandlerServer, self)
end

function BagNetContorller:ClickBagHeaderBtnHandlerServer(type, msgProtocol, handler)
    if AllBagItemRequest == nil then
        return
    end
    local packet = AllBagItemRequest(type, msgProtocol)
    PacketDispatcher:AddProtocolConfigEvent(
        packet:protocolId(), handler)
    local jsonStr = packet:write()
    NetManager:SendMessageEvent(jsonStr)
end

return BagNetContorller
