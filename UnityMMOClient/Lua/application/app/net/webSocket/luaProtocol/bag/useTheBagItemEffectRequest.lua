---@class UseTheBagItemEffectRequest
local UseTheBagItemEffectRequest = class("UseTheBagItemEffectRequest")
function UseTheBagItemEffectRequest:ctor()
    --- 道具id
    ---@type number
    self.itemId = 0
    --- 道具类型
    ---@type number
    self.itemType = 0
    --- 使用玩家id
    ---@type number
    self.userID = 0
    --- 使用数量
    ---@type number
    self.userTheNum = 0
end

---@param itemId number 道具id
---@param itemType number 道具类型
---@param userID number 使用玩家id
---@param userTheNum number 使用数量
---@return UseTheBagItemEffectRequest
function UseTheBagItemEffectRequest:new(itemId, itemType, userID, userTheNum)
    self.itemId = itemId --- int
    self.itemType = itemType --- int
    self.userID = userID --- long
    self.userTheNum = userTheNum --- int
    return self
end

---@return number
function UseTheBagItemEffectRequest:protocolId()
    return 1033
end

---@return string
function UseTheBagItemEffectRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            itemId = self.itemId,
            itemType = self.itemType,
            userID = self.userID,
            userTheNum = self.userTheNum
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return UseTheBagItemEffectRequest
function UseTheBagItemEffectRequest:read(data)

    local packet = self:new(
            data.itemId,
            data.itemType,
            data.userID,
            data.userTheNum)
    return packet
end

--- 道具id
---@return number 道具id
function UseTheBagItemEffectRequest:getItemId()
    return self.itemId
end
--- 道具类型
---@return number 道具类型
function UseTheBagItemEffectRequest:getItemType()
    return self.itemType
end
--- 使用玩家id
---@return number 使用玩家id
function UseTheBagItemEffectRequest:getUserID()
    return self.userID
end
--- 使用数量
---@return number 使用数量
function UseTheBagItemEffectRequest:getUserTheNum()
    return self.userTheNum
end


return UseTheBagItemEffectRequest
