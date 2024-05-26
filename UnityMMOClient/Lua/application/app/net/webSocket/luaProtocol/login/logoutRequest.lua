---@class LogoutRequest
local LogoutRequest = class("LogoutRequest")
function LogoutRequest:ctor()
    
end


---@return LogoutRequest
function LogoutRequest:new()
    
    return self
end

---@return number
function LogoutRequest:protocolId()
    return 1002
end

---@return string
function LogoutRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return LogoutRequest
function LogoutRequest:read(data)

    local packet = self:new(
            )
    return packet
end



return LogoutRequest
