---@class TripleLSS
local TripleLSS = class("TripleLSS")
function TripleLSS:ctor()
    ---@type number
    self.left = 0
    ---@type string
    self.middle = string.empty
    ---@type string
    self.right = string.empty
end

---@param left number 
---@param middle string 
---@param right string 
---@return TripleLSS
function TripleLSS:new(left, middle, right)
    self.left = left --- long
    self.middle = middle --- java.lang.String
    self.right = right --- java.lang.String
    return self
end

---@return number
function TripleLSS:protocolId()
    return 116
end

---@return string
function TripleLSS:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            left = self.left,
            middle = self.middle,
            right = self.right
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return TripleLSS
function TripleLSS:read(data)

    local packet = self:new(
            data.left,
            data.middle,
            data.right)
    return packet
end

--- 
---@return number 
function TripleLSS:getLeft()
    return self.left
end
--- 
---@type  string 
function TripleLSS:getMiddle()
    return self.middle
end

--- 
---@type  string 
function TripleLSS:getRight()
    return self.right
end



return TripleLSS
