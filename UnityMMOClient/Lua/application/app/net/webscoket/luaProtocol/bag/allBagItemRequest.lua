---@class AllBagItemRequest
local AllBagItemReuqest = class("AllBagItemReuqest")
function AllBagItemReuqest:ctor(type, msgProtocol)
    self.type = type;
    self.msgProtocol = msgProtocol;
end

function AllBagItemReuqest:protocolId()
    return 1007
end
function AllBagItemReuqest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = { type = self.type, msgProtocol = self.msgProtocol }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

return AllBagItemReuqest
