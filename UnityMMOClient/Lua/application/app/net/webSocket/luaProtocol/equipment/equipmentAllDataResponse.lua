---@class EquipmentAllDataResponse
local EquipmentAllDataResponse = class("EquipmentAllDataResponse")
function EquipmentAllDataResponse:ctor()
    --- 圣遗物List
    ---@type  table<number,EquipmentData>
    self.equipmentDataList = {}
end

---@param equipmentDataList table<number,EquipmentData> 圣遗物List
---@return EquipmentAllDataResponse
function EquipmentAllDataResponse:new(equipmentDataList)
    self.equipmentDataList = equipmentDataList --- java.util.List<com.gameServer.common.protocol.equipment.EquipmentData>
    return self
end

---@return number
function EquipmentAllDataResponse:protocolId()
    return 1038
end

---@return string
function EquipmentAllDataResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            equipmentDataList = self.equipmentDataList
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return EquipmentAllDataResponse
function EquipmentAllDataResponse:read(data)
    local equipmentDataList = {}
    for index, value in ipairs(data.equipmentDataList) do
        local equipmentDataListPacket = EquipmentData()
        local packetData = equipmentDataListPacket:read(value)
        table.insert(equipmentDataList,packetData)
    end

    local packet = self:new(
            equipmentDataList)
    return packet
end

--- 圣遗物List
---@type  table<number,EquipmentData> 圣遗物List
function EquipmentAllDataResponse:getEquipmentDataList()
    return self.equipmentDataList
end


return EquipmentAllDataResponse
