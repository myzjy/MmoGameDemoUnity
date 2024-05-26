---@class AcquireCharacterRequest
local AcquireCharacterRequest = class("AcquireCharacterRequest")
function AcquireCharacterRequest:ctor()
    
end


---@return AcquireCharacterRequest
function AcquireCharacterRequest:new()
    
    return self
end

---@return number
function AcquireCharacterRequest:protocolId()
    return 1045
end

---@return string
function AcquireCharacterRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return AcquireCharacterRequest
function AcquireCharacterRequest:read(data)

    local packet = self:new(
            )
    return packet
end



return AcquireCharacterRequest
