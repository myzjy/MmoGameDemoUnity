---@class PairLS
local PairLS = class("PairLS")
function PairLS:ctor()
    ---@type number
    self.key = 0
    ---@type string
    self.value = string.empty
end

---@param key number 
---@param value string 
---@return PairLS
function PairLS:new(key, value)
    self.key = key --- long
    self.value = value --- java.lang.String
    return self
end

---@return number
function PairLS:protocolId()
    return 113
end

---@return string
function PairLS:write()
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

---@return PairLS
function PairLS:read(data)

    local packet = self:new(
            data.key,
            data.value)
    return packet
end

--- 
---@return number 
function PairLS:getKey()
    return self.key
end
--- 
---@type  string 
function PairLS:getValue()
    return self.value
end



return PairLS
