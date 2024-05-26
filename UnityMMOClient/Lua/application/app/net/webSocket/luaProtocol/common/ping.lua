---@class Ping
local Ping = class("Ping")
function Ping:ctor()
    
end


---@return Ping
function Ping:new()
    
    return self
end

---@return number
function Ping:protocolId()
    return 103
end

---@return string
function Ping:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return Ping
function Ping:read(data)

    local packet = self:new(
            )
    return packet
end



return Ping
