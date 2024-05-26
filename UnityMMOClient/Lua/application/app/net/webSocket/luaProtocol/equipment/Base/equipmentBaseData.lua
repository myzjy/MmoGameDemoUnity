---@class EquipmentBaseData
local EquipmentBaseData = class("EquipmentBaseData")
function EquipmentBaseData:ctor()
    --- 介绍id
    ---@type number
    self.desId = 0
    --- 圣遗物的名字
    ---@type string
    self.equipmentName = string.empty
    --- 装备只能装配在什么位置
    ---@type number
    self.equipmentPosType = 0
    --- 主属性集合可以获取那些属性
    ---@type string
    self.mainAttributes = string.empty
    --- 品阶
    ---@type number
    self.quality = 0
end

---@param desId number 介绍id
---@param equipmentName string 圣遗物的名字
---@param equipmentPosType number 装备只能装配在什么位置
---@param mainAttributes string 主属性集合可以获取那些属性
---@param quality number 品阶
---@return EquipmentBaseData
function EquipmentBaseData:new(desId, equipmentName, equipmentPosType, mainAttributes, quality)
    self.desId = desId --- int
    self.equipmentName = equipmentName --- java.lang.String
    self.equipmentPosType = equipmentPosType --- int
    self.mainAttributes = mainAttributes --- java.lang.String
    self.quality = quality --- int
    return self
end

---@return number
function EquipmentBaseData:protocolId()
    return 209
end

---@return string
function EquipmentBaseData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            desId = self.desId,
            equipmentName = self.equipmentName,
            equipmentPosType = self.equipmentPosType,
            mainAttributes = self.mainAttributes,
            quality = self.quality
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return EquipmentBaseData
function EquipmentBaseData:read(data)

    local packet = self:new(
            data.desId,
            data.equipmentName,
            data.equipmentPosType,
            data.mainAttributes,
            data.quality)
    return packet
end

--- 介绍id
---@return number 介绍id
function EquipmentBaseData:getDesId()
    return self.desId
end
--- 圣遗物的名字
---@type  string 圣遗物的名字
function EquipmentBaseData:getEquipmentName()
    return self.equipmentName
end

--- 装备只能装配在什么位置
---@return number 装备只能装配在什么位置
function EquipmentBaseData:getEquipmentPosType()
    return self.equipmentPosType
end
--- 主属性集合可以获取那些属性
---@type  string 主属性集合可以获取那些属性
function EquipmentBaseData:getMainAttributes()
    return self.mainAttributes
end

--- 品阶
---@return number 品阶
function EquipmentBaseData:getQuality()
    return self.quality
end


return EquipmentBaseData
