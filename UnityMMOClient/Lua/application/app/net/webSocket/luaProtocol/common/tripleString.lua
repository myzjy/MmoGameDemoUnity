---@class TripleString
local TripleString = class("TripleString")
function TripleString:ctor()
    ---@type string
    self.left = string.empty
    ---@type string
    self.middle = string.empty
    ---@type string
    self.right = string.empty
end

---@param left string 
---@param middle string 
---@param right string 
---@return TripleString
function TripleString:new(left, middle, right)
    self.left = left --- java.lang.String
    self.middle = middle --- java.lang.String
    self.right = right --- java.lang.String
    return self
end

---@return number
function TripleString:protocolId()
    return 115
end

---@return string
function TripleString:write()
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

---@return TripleString
function TripleString:read(data)

    local packet = self:new(
            data.left,
            data.middle,
            data.right)
    return packet
end

--- 
---@type  string 
function TripleString:getLeft()
    return self.left
end

--- 
---@type  string 
function TripleString:getMiddle()
    return self.middle
end

--- 
---@type  string 
function TripleString:getRight()
    return self.right
end



return TripleString
