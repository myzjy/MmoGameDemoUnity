---@class EquipmentBaseData
local EquipmentBaseData = {}


---@param desId number 介绍id
---@param quality number 品阶
---@param equipmentPosType number 装备只能装配在什么位置
---@param equipmentName string 圣遗物的名字
---@param mainAttributes string 主属性集合可以获取那些属性
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
