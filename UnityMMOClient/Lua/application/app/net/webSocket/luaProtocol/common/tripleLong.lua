---@class TripleLong
local TripleLong = class("TripleLong")
function TripleLong:ctor()
    ---@type number
    self.left = 0
    ---@type number
    self.middle = 0
    ---@type number
    self.right = 0
end

---@param left number 
---@param middle number 
---@param right number 
---@return TripleLong
function TripleLong:new(left, middle, right)
    self.left = left --- long
    self.middle = middle --- long
    self.right = right --- long
    return self
end

---@return number
function TripleLong:protocolId()
    return 114
end

---@return string
function TripleLong:write()
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

---@return TripleLong
function TripleLong:read(data)

    local packet = self:new(
            data.left,
            data.middle,
            data.right)
    return packet
end

--- 
---@return number 
function TripleLong:getLeft()
    return self.left
end
--- 
---@return number 
function TripleLong:getMiddle()
    return self.middle
end
--- 
---@return number 
function TripleLong:getRight()
    return self.right
end


return TripleLong
