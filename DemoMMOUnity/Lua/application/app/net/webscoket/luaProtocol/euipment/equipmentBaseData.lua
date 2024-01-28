---@class EquipmentBaseData
local EquipmentBaseData = {}


---@param desId number
---@param quality number
---@param equipmentPosType number
---@param equipmentName string
---@param mainAttributes string
function EquipmentBaseData:new(desId, quality, equipmentPosType, equipmentName, mainAttributes)
    local obj = {
        desId = desId,
        quality = quality,
        equipmentPosType = equipmentPosType,
        equipmentName = equipmentName,
        mainAttributes = mainAttributes
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end

function EquipmentBaseData:protocolId()
    return 209
end

function EquipmentBaseData:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or EquipmentBaseData
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function EquipmentBaseData:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {protocolId:number,packet:{desId:number, quality:number, equipmentPosType:number,equipmentName:string,mainAttributes:string}}
    local data = JSON.decode(jsonString)
    return EquipmentBaseData:new(
        data.packet.desId, 
        data.packet.quality, 
        data.packet.equipmentPosType,
        data.packet.equipmentName, 
        data.packet.mainAttributes)
end

return EquipmentBaseData
