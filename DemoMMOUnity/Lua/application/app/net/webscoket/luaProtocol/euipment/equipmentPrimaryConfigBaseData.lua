---@class EquipmentPrimaryConfigBaseData
local EquipmentPrimaryConfigBaseData = {}


---@param id integer
---@param primaryQuality integer
---@param growthPosInt integer
---@param primaryGrowthName string
---@param primaryGrowthInts string
---@param primaryGrowthMaxInt string
---@param growthPosName string
function EquipmentPrimaryConfigBaseData:new(
    id,
    primaryQuality,
    growthPosInt,
    primaryGrowthName,
    primaryGrowthInts,
    primaryGrowthMaxInt,
    growthPosName)
    local obj = {
        id = id,
        primaryQuality = primaryQuality,
        growthPosInt = growthPosInt,
        primaryGrowthName = primaryGrowthName,
        primaryGrowthInts = primaryGrowthInts,
        primaryGrowthMaxInt = primaryGrowthMaxInt,
        growthPosName = growthPosName
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end

function EquipmentPrimaryConfigBaseData:protocolId()
    return 208
end

function EquipmentPrimaryConfigBaseData:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or EquipmentPrimaryConfigBaseData
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function EquipmentPrimaryConfigBaseData:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {protocolId:number,packet:{id:integer,primaryQuality:integer,growthPosInt:integer,primaryGrowthName:string,primaryGrowthInts:string,primaryGrowthMaxInt:string,growthPosName:string}}
    local data = JSON.decode(jsonString)
    return EquipmentPrimaryConfigBaseData:new(
        data.packet.id, data.packet.primaryQuality, data.packet.growthPosInt, data.packet.primaryGrowthName, data.packet.primaryGrowthInts,
        data.packet.primaryGrowthMaxInt, data.packet.growthPosName
    )
end

function EquipmentPrimaryConfigBaseData:readData(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {id:integer,primaryQuality:integer,growthPosInt:integer,primaryGrowthName:string,primaryGrowthInts:string,primaryGrowthMaxInt:string,growthPosName:string}
    local data = JSON.decode(jsonString)
    return EquipmentPrimaryConfigBaseData:new(
        data.id, data.primaryQuality, data.growthPosInt, data.primaryGrowthName, data.primaryGrowthInts,
        data.primaryGrowthMaxInt, data.growthPosName
    )
end

return EquipmentPrimaryConfigBaseData
