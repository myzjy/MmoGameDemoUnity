---@class ItemBaseData
local ItemBaseData = class("ItemBaseData")
local this = ItemBaseData
function ItemBaseData:ctor()
    ---@type number
    self.id = 0
    ---@type string
    self.name = string.empty
    ---@type string
    self.icon = string.empty
    ---@type number
    self.minNum = 0
    ---@type number
    self.maxNum = 0
    ---@type number
    self.type = 0
    ---@type string
    self.des = string.empty
end

---@param id integer
---@param name string
---@param icon string
---@param minNum integer
---@param maxNum integer
---@param type integer
---@param des string
function ItemBaseData:new(id,
                          name,
                          icon,
                          minNum,
                          maxNum,
                          type,
                          des)
    self.id = id
    self.name = name
    self.icon = icon
    self.minNum = minNum
    self.maxNum = maxNum
    self.type = type
    self.des = des
    return this
end

function ItemBaseData:protocolId()
    return 201
end

function ItemBaseData:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or ItemBaseData
    local message = {
        protocolId = data.protocolId(),
        packet = { id = self.id, name = self.name, icon = self.icon, minNum = self.minNum, maxNum = self.maxNum, type = self.type, des = self.des }
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function ItemBaseData:read(data)
    return ItemBaseData:new(
        data.id,
        data.name,
        data.icon,
        data.minNum,
        data.maxNum,
        data.type,
        data.des)
end

return ItemBaseData
