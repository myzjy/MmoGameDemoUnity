---@class GetPlayerInfoResponse
local GetPlayerInfoResponse = class("GetPlayerInfoResponse")
function GetPlayerInfoResponse:ctor()
    
end


---@return GetPlayerInfoResponse
function GetPlayerInfoResponse:new()
    
    return self
end

---@return number
function GetPlayerInfoResponse:protocolId()
    return 1012
end

---@return string
function GetPlayerInfoResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return GetPlayerInfoResponse
function GetPlayerInfoResponse:read(data)

    local packet = self:new(
            )
    return packet
end



return GetPlayerInfoResponse
