---@class CharacterEquipmentIDData
local CharacterEquipmentIDData = class("CharacterEquipmentIDData")
function CharacterEquipmentIDData:ctor()
    --- 在数据库中存放的id
    ---@type number
    self.equipmentFindId = 0
    --- 装备id
    ---@type number
    self.equipmentId = 0
end

---@param equipmentFindId number 在数据库中存放的id
---@param equipmentId number 装备id
---@return CharacterEquipmentIDData
function CharacterEquipmentIDData:new(equipmentFindId, equipmentId)
    self.equipmentFindId = equipmentFindId --- int
    self.equipmentId = equipmentId --- int
    return self
end

---@return number
function CharacterEquipmentIDData:protocolId()
    return 218
end

---@return string
function CharacterEquipmentIDData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            equipmentFindId = self.equipmentFindId,
            equipmentId = self.equipmentId
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return CharacterEquipmentIDData
function CharacterEquipmentIDData:read(data)

    local packet = self:new(
            data.equipmentFindId,
            data.equipmentId)
    return packet
end

--- 在数据库中存放的id
---@return number 在数据库中存放的id
function CharacterEquipmentIDData:getEquipmentFindId()
    return self.equipmentFindId
end
--- 装备id
---@return number 装备id
function CharacterEquipmentIDData:getEquipmentId()
    return self.equipmentId
end


return CharacterEquipmentIDData
