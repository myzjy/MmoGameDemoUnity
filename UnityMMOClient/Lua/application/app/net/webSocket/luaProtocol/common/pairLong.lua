---@class PairLong
local PairLong = class("PairLong")
function PairLong:ctor()
    ---@type number
    self.key = 0
    ---@type number
    self.value = 0
end

---@param key number 
---@param value number 
---@return PairLong
function PairLong:new(key, value)
    self.key = key --- long
    self.value = value --- long
    return self
end

---@return number
function PairLong:protocolId()
    return 111
end

---@return string
function PairLong:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            key = self.key,
            value = self.value
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return PairLong
function PairLong:read(data)

    local packet = self:new(
            data.key,
            data.value)
    return packet
end

--- 
---@return number 
function PairLong:getKey()
    return self.key
end
--- 
---@return number 
function PairLong:getValue()
    return self.value
end


return PairLong
