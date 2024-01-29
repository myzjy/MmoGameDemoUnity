---@class EquipmentGrowthConfigBaseData
local EquipmentGrowthConfigBaseData = {}


---@param id number id
---@param locationOfEquipmentType number 圣遗物位置
---@param posName string 位置的名字
function EquipmentGrowthConfigBaseData:new(id, locationOfEquipmentType, posName)
    local obj = {
        id = id,
        locationOfEquipmentType = locationOfEquipmentType,
        posName = posName,

    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end

function EquipmentGrowthConfigBaseData:protocolId()
    return 211
end

function EquipmentGrowthConfigBaseData:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or EquipmentGrowthConfigBaseData
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function EquipmentGrowthConfigBaseData:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {protocolId:number,packet:{id:number, locationOfEquipmentType:number, posName:string}}
    local data = JSON.decode(jsonString)
    return EquipmentGrowthConfigBaseData:new(
        data.packet.id,
        data.packet.locationOfEquipmentType,
        data.packet.posName
    )
end

return EquipmentGrowthConfigBaseData
