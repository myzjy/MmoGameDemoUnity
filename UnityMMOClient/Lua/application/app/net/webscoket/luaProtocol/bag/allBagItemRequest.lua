---@class AllBagItemRequest
local AllBagItemRequest = class("AllBagItemReuqest")
function AllBagItemRequest:ctor(type, msgProtocol)
    self.type = type;
    self.msgProtocol = msgProtocol;
end

function AllBagItemRequest:protocolId()
    return 1007
end
function AllBagItemRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = { type = self.type, msgProtocol = self.msgProtocol }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

return AllBagItemRequest
