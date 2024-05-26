---@class Heartbeat
local Heartbeat = class("Heartbeat")
function Heartbeat:ctor()
    
end


---@return Heartbeat
function Heartbeat:new()
    
    return self
end

---@return number
function Heartbeat:protocolId()
    return 102
end

---@return string
function Heartbeat:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return Heartbeat
function Heartbeat:read(data)

    local packet = self:new(
            )
    return packet
end



return Heartbeat
