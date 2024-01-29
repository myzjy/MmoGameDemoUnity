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

return ServerConfigResponse
