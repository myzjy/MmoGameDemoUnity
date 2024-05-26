---@class BagUserItemData
local BagUserItemData = class("BagUserItemData")
function BagUserItemData:ctor()
    --- 当前道具在数据库中的唯一id
    ---@type number
    self._id = 0
    --- Icon
    ---@type string
    self.icon = string.empty
    --- 道具id
    ---@type number
    self.itemId = 0
    --- 是否为新
    ---@type boolean
    self.itemNew = 0
    --- 这个背包道具所属者是谁
    ---@type number
    self.masterUserId = 0
    --- 道具的数量
    ---@type number
    self.nowItemNum = 0
    --- 道具装备品质
    ---@type number
    self.quality = 0
    --- 资源路径
    ---@type string
    self.resourcePath = string.empty
    --- 这个道具如果是武器/圣遗物的话
    ---@type number
    self.userPlayerId = 0
end

---@param _id number 当前道具在数据库中的唯一id
---@param icon string Icon
---@param itemId number 道具id
---@param boolean boolean 是否为新
---@param masterUserId number 这个背包道具所属者是谁
---@param nowItemNum number 道具的数量
---@param quality number 道具装备品质
---@param resourcePath string 资源路径
---@param userPlayerId number 这个道具如果是武器/圣遗物的话
---@return BagUserItemData
function BagUserItemData:new(_id, icon, itemId, itemNew, masterUserId, nowItemNum, quality, resourcePath, userPlayerId)
    self._id = _id --- long
    self.icon = icon --- java.lang.String
    self.itemId = itemId --- int
    self.itemNew = itemNew --- boolean
    self.masterUserId = masterUserId --- long
    self.nowItemNum = nowItemNum --- int
    self.quality = quality --- int
    self.resourcePath = resourcePath --- java.lang.String
    self.userPlayerId = userPlayerId --- int
    return self
end

---@return number
function BagUserItemData:protocolId()
    return 200
end

---@return string
function BagUserItemData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            _id = self._id,
            icon = self.icon,
            itemId = self.itemId,
            itemNew = self.itemNew,
            masterUserId = self.masterUserId,
            nowItemNum = self.nowItemNum,
            quality = self.quality,
            resourcePath = self.resourcePath,
            userPlayerId = self.userPlayerId
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return BagUserItemData
function BagUserItemData:read(data)

    local packet = self:new(
            data._id,
            data.icon,
            data.itemId,
            data.itemNew,
            data.masterUserId,
            data.nowItemNum,
            data.quality,
            data.resourcePath,
            data.userPlayerId)
    return packet
end

--- 当前道具在数据库中的唯一id
---@return number 当前道具在数据库中的唯一id
function BagUserItemData:get_id()
    return self._id
end
--- Icon
---@type  string Icon
function BagUserItemData:getIcon()
    return self.icon
end

--- 道具id
---@return number 道具id
function BagUserItemData:getItemId()
    return self.itemId
end
--- 是否为新
---@return boolean 是否为新
function BagUserItemData:getItemNew()
    return self.itemNew
end
--- 这个背包道具所属者是谁
---@return number 这个背包道具所属者是谁
function BagUserItemData:getMasterUserId()
    return self.masterUserId
end
--- 道具的数量
---@return number 道具的数量
function BagUserItemData:getNowItemNum()
    return self.nowItemNum
end
--- 道具装备品质
---@return number 道具装备品质
function BagUserItemData:getQuality()
    return self.quality
end
--- 资源路径
---@type  string 资源路径
function BagUserItemData:getResourcePath()
    return self.resourcePath
end

--- 这个道具如果是武器/圣遗物的话
---@return number 这个道具如果是武器/圣遗物的话
function BagUserItemData:getUserPlayerId()
    return self.userPlayerId
end


return BagUserItemData
