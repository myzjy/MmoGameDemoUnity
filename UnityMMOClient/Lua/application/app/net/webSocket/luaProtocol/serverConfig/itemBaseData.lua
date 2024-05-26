---@class ItemBaseData
local ItemBaseData = class("ItemBaseData")
function ItemBaseData:ctor()
    --- 介绍
    ---@type string
    self.des = string.empty
    --- 道具Icon
    ---@type string
    self.icon = string.empty
    --- 道具id
    ---@type number
    self.id = 0
    --- 最大数量
    ---@type number
    self.maxNum = 0
    --- 最小数量
    ---@type number
    self.minNum = 0
    --- 道具名字
    ---@type string
    self.name = string.empty
    --- item类型
    ---@type number
    self.type = 0
end

---@param des string 介绍
---@param icon string 道具Icon
---@param id number 道具id
---@param maxNum number 最大数量
---@param minNum number 最小数量
---@param name string 道具名字
---@param type number item类型
---@return ItemBaseData
function ItemBaseData:new(des, icon, id, maxNum, minNum, name, type)
    self.des = des --- java.lang.String
    self.icon = icon --- java.lang.String
    self.id = id --- int
    self.maxNum = maxNum --- int
    self.minNum = minNum --- int
    self.name = name --- java.lang.String
    self.type = type --- int
    return self
end

---@return number
function ItemBaseData:protocolId()
    return 201
end

---@return string
function ItemBaseData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            des = self.des,
            icon = self.icon,
            id = self.id,
            maxNum = self.maxNum,
            minNum = self.minNum,
            name = self.name,
            type = self.type
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return ItemBaseData
function ItemBaseData:read(data)

    local packet = self:new(
            data.des,
            data.icon,
            data.id,
            data.maxNum,
            data.minNum,
            data.name,
            data.type)
    return packet
end

--- 介绍
---@type  string 介绍
function ItemBaseData:getDes()
    return self.des
end

--- 道具Icon
---@type  string 道具Icon
function ItemBaseData:getIcon()
    return self.icon
end

--- 道具id
---@return number 道具id
function ItemBaseData:getId()
    return self.id
end
--- 最大数量
---@return number 最大数量
function ItemBaseData:getMaxNum()
    return self.maxNum
end
--- 最小数量
---@return number 最小数量
function ItemBaseData:getMinNum()
    return self.minNum
end
--- 道具名字
---@type  string 道具名字
function ItemBaseData:getName()
    return self.name
end

--- item类型
---@return number item类型
function ItemBaseData:getType()
    return self.type
end


return ItemBaseData
