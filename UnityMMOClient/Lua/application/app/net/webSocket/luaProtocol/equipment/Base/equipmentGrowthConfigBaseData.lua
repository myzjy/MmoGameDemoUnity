---@class EquipmentGrowthConfigBaseData
local EquipmentGrowthConfigBaseData = class("EquipmentGrowthConfigBaseData")
function EquipmentGrowthConfigBaseData:ctor()
    --- id
    ---@type number
    self.id = 0
    --- 圣遗物位置
    ---@type number
    self.locationOfEquipmentType = 0
    --- 位置的名字
    ---@type string
    self.posName = string.empty
end

---@param id number id
---@param locationOfEquipmentType number 圣遗物位置
---@param posName string 位置的名字
---@return EquipmentGrowthConfigBaseData
function EquipmentGrowthConfigBaseData:new(id, locationOfEquipmentType, posName)
    self.id = id --- int
    self.locationOfEquipmentType = locationOfEquipmentType --- int
    self.posName = posName --- java.lang.String
    return self
end

---@return number
function EquipmentGrowthConfigBaseData:protocolId()
    return 211
end

---@return string
function EquipmentGrowthConfigBaseData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            id = self.id,
            locationOfEquipmentType = self.locationOfEquipmentType,
            posName = self.posName
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return EquipmentGrowthConfigBaseData
function EquipmentGrowthConfigBaseData:read(data)

    local packet = self:new(
            data.id,
            data.locationOfEquipmentType,
            data.posName)
    return packet
end

--- id
---@return number id
function EquipmentGrowthConfigBaseData:getId()
    return self.id
end
--- 圣遗物位置
---@return number 圣遗物位置
function EquipmentGrowthConfigBaseData:getLocationOfEquipmentType()
    return self.locationOfEquipmentType
end
--- 位置的名字
---@type  string 位置的名字
function EquipmentGrowthConfigBaseData:getPosName()
    return self.posName
end



return EquipmentGrowthConfigBaseData
