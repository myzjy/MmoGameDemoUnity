---@class EquipmentGlossaryData
local EquipmentGlossaryData = class("EquipmentGlossaryData")
function EquipmentGlossaryData:ctor()
    --- 数值
    ---@type number
    self.equipmentNum = 0
    --- 圣遗物副属性词条type
    ---@type number
    self.equipmentType = 0
end

---@param equipmentNum number 数值
---@param equipmentType number 圣遗物副属性词条type
---@return EquipmentGlossaryData
function EquipmentGlossaryData:new(equipmentNum, equipmentType)
    self.equipmentNum = equipmentNum --- int
    self.equipmentType = equipmentType --- int
    return self
end

---@return number
function EquipmentGlossaryData:protocolId()
    return 205
end

---@return string
function EquipmentGlossaryData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            equipmentNum = self.equipmentNum,
            equipmentType = self.equipmentType
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return EquipmentGlossaryData
function EquipmentGlossaryData:read(data)

    local packet = self:new(
            data.equipmentNum,
            data.equipmentType)
    return packet
end

--- 数值
---@return number 数值
function EquipmentGlossaryData:getEquipmentNum()
    return self.equipmentNum
end
--- 圣遗物副属性词条type
---@return number 圣遗物副属性词条type
function EquipmentGlossaryData:getEquipmentType()
    return self.equipmentType
end


return EquipmentGlossaryData
