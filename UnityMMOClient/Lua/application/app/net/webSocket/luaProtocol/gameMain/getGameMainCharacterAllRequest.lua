---@class GetGameMainCharacterAllRequest
local GetGameMainCharacterAllRequest = class("GetGameMainCharacterAllRequest")
function GetGameMainCharacterAllRequest:ctor()
    
end


---@return GetGameMainCharacterAllRequest
function GetGameMainCharacterAllRequest:new()
    
    return self
end

---@return number
function GetGameMainCharacterAllRequest:protocolId()
    return 1049
end

---@return string
function GetGameMainCharacterAllRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return GetGameMainCharacterAllRequest
function GetGameMainCharacterAllRequest:read(data)

    local packet = self:new(
            )
    return packet
end



return GetGameMainCharacterAllRequest
