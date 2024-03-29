---@class ServerConfigResponse
local ServerConfigResponse = class("ServerConfigResponse")
function ServerConfigResponse:ctor()
    ---@type  table<integer,ItemBaseData>
    self.bagItemEntityList = nil
    ---@type  table<integer,EquipmentBaseData>
    self.equipmentBaseDataList = nil
    ---@type  table<integer,EquipmentConfigBaseData>
    self.equipmentConfigBaseDataList = nil
    ---@type  table<integer,EquipmentPrimaryConfigBaseData>
    self.equipmentPrimaryConfigBaseDataList = nil
    ---@type  table<integer,EquipmentDesBaseData>
    self.equipmentDesBaseDataList = nil
    ---@type  table<integer,EquipmentGrowthConfigBaseData>
    self.equipmentGrowthConfigBaseDataList = nil
    ---@type  table<integer,EquipmentGrowthViceConfigBaseData>
    self.equipmentGrowthViceConfigBaseDataList = nil
    ---@type  table<integer,WeaponsConfigData>
    self.weaponsConfigDataList = nil
end

---@param bagItemEntityList table<integer,ItemBaseData>
---@param equipmentBaseDataList table<integer,EquipmentBaseData>
---@param equipmentConfigBaseDataList table<integer,EquipmentConfigBaseData>
---@param equipmentPrimaryConfigBaseDataList table<integer,EquipmentPrimaryConfigBaseData>
---@param equipmentDesBaseDataList table<integer,EquipmentDesBaseData>
---@param equipmentGrowthConfigBaseDataList table<integer,EquipmentGrowthConfigBaseData>
---@param equipmentGrowthViceConfigBaseDataList table<integer,EquipmentGrowthViceConfigBaseData>
---@param weaponsConfigDataList table<integer,WeaponsConfigData>
function ServerConfigResponse:new(
    bagItemEntityList,
    equipmentConfigBaseDataList,
    equipmentBaseDataList,
    equipmentPrimaryConfigBaseDataList,
    equipmentDesBaseDataList,
    equipmentGrowthConfigBaseDataList,
    equipmentGrowthViceConfigBaseDataList,
    weaponsConfigDataList
)
    self.bagItemEntityList = bagItemEntityList
    self.equipmentBaseDataList = equipmentBaseDataList
    self.equipmentConfigBaseDataList = equipmentConfigBaseDataList
    self.equipmentPrimaryConfigBaseDataList = equipmentPrimaryConfigBaseDataList
    self.equipmentDesBaseDataList = equipmentDesBaseDataList
    self.equipmentGrowthConfigBaseDataList = equipmentGrowthConfigBaseDataList
    self.equipmentGrowthViceConfigBaseDataList = equipmentGrowthViceConfigBaseDataList
    self.weaponsConfigDataList = weaponsConfigDataList
    return self
end

function ServerConfigResponse:protocolId()
    return 1010
end

function ServerConfigResponse:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or ServerConfigResponse
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
        data.bagItemEntityList,
        data.equipmentConfigBaseDataList,
        data.equipmentBaseDataList,
        data.equipmentPrimaryConfigBaseDataList,
        data.equipmentDesBaseDataList,
        data.equipmentGrowthConfigBaseDataList,
        data.equipmentGrowthViceConfigBaseDataList,
        data.weaponsConfigDataList
    )
end

return ServerConfigResponse
