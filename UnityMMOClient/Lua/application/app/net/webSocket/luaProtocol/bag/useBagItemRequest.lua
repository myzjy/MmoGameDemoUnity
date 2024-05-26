---@class UseBagItemRequest
local UseBagItemRequest = class("UseBagItemRequest")
function UseBagItemRequest:ctor()
    
end


---@return UseBagItemRequest
function UseBagItemRequest:new()
    
    return self
end

---@return number
function UseBagItemRequest:protocolId()
    return 1011
end

---@return string
function UseBagItemRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return UseBagItemRequest
function UseBagItemRequest:read(data)

    local packet = self:new(
            )
    return packet
end



return UseBagItemRequest
