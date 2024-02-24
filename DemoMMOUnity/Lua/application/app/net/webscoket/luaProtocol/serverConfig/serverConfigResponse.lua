---@class ServerConfigResponse
local ServerConfigResponse = {}


---@param bagItemEntityList table<integer,ItemBaseData>
---@param equipmentConfigBaseDataList table<integer,EquipmentConfigBaseData>
---@param equipmentBaseDataList table<integer,EquipmentBaseData>
---@param equipmentPrimaryConfigBaseDataList table<integer,EquipmentPrimaryConfigBaseData>
---@param equipmentDesBaseDataList table<integer,EquipmentDesBaseData>
---@param equipmentGrowthConfigBaseDataList table<integer,EquipmentGrowthConfigBaseData>
---@param equipmentGrowthViceConfigBaseDataList table<integer,EquipmentGrowthViceConfigBaseData>
function ServerConfigResponse:new(
    bagItemEntityList,
    equipmentConfigBaseDataList,
    equipmentBaseDataList,
    equipmentPrimaryConfigBaseDataList,
    equipmentDesBaseDataList,
    equipmentGrowthConfigBaseDataList,
    equipmentGrowthViceConfigBaseDataList
)
    local obj = {
        bagItemEntityList = bagItemEntityList,
        equipmentBaseDataList = equipmentBaseDataList,
        equipmentConfigBaseDataList = equipmentConfigBaseDataList,
        equipmentPrimaryConfigBaseDataList = equipmentPrimaryConfigBaseDataList,
        equipmentDesBaseDataList = equipmentDesBaseDataList,
        equipmentGrowthConfigBaseDataList = equipmentGrowthConfigBaseDataList,
        equipmentGrowthViceConfigBaseDataList = equipmentGrowthViceConfigBaseDataList
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end
function ServerConfigResponse:protocolId()
    return 1010
end
function ServerConfigResponse:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or RegisterResponse
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function ServerConfigResponse:read(data)
    -- local jsonString = buffer:readString()
    -- ---字节读取器中存放字符
    -- ---@type {protocolId:number,packet:{bagItemEntityList:table<integer,ItemBaseData>,equipmentConfigBaseDataList:table<integer,EquipmentConfigBaseData>,equipmentBaseDataList:table<integer,EquipmentBaseData>,equipmentPrimaryConfigBaseDataList:table<integer,EquipmentPrimaryConfigBaseData>,equipmentDesBaseDataList:table<integer,EquipmentDesBaseData>,equipmentGrowthConfigBaseDataList:table<integer,EquipmentGrowthConfigBaseData>,equipmentGrowthViceConfigBaseDataList:table<integer,EquipmentGrowthViceConfigBaseData>}}
    -- local data = JSON.decode(jsonString)
    return ServerConfigResponse:new(
        data.packet.bagItemEntityList,
        data.packet.equipmentConfigBaseDataList,
        data.packet.equipmentBaseDataList,
        data.packet.equipmentPrimaryConfigBaseDataList,
        data.packet.equipmentDesBaseDataList,
        data.packet.equipmentGrowthConfigBaseDataList,
        data.packet.equipmentGrowthViceConfigBaseDataList
    )
end

return ServerConfigResponse
