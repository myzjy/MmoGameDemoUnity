---@class Pong
local Pong = class("Pong")
function Pong:ctor()
    ---@type number
    self.time = 0
end

---@param time number 
---@return Pong
function Pong:new(time)
    self.time = time --- long
    return self
end

---@return number
function Pong:protocolId()
    return 104
end

---@return string
function Pong:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            time = self.time
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return Pong
function Pong:read(data)

    local packet = self:new(
            data.time)
    return packet
end

--- 
---@return number 
function Pong:getTime()
    return self.time
end


return Pong
