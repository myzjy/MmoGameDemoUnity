---@class PairString
local PairString = class("PairString")
function PairString:ctor()
    ---@type string
    self.key = string.empty
    ---@type string
    self.value = string.empty
end

---@param key string 
---@param value string 
---@return PairString
function PairString:new(key, value)
    self.key = key --- java.lang.String
    self.value = value --- java.lang.String
    return self
end

---@return number
function PairString:protocolId()
    return 112
end

---@return string
function PairString:write()
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

---@return PairString
function PairString:read(data)

    local packet = self:new(
            data.key,
            data.value)
    return packet
end

--- 
---@type  string 
function PairString:getKey()
    return self.key
end

--- 
---@type  string 
function PairString:getValue()
    return self.value
end



return PairString
