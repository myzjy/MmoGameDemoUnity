---@class ServerConfigResponse
local ServerConfigResponse = class("ServerConfigResponse")
function ServerConfigResponse:ctor()
    --- 背包基础类list
    ---@type  table<number,ItemBaseData>
    self.bagItemEntityList = {}
    --- 圣遗物基础配置相关
    ---@type  table<number,EquipmentBaseData>
    self.equipmentBaseDataList = {}
    --- 装备基础相关根据品节卡等级强化获取副属性条
    ---@type  table<number,EquipmentConfigBaseData>
    self.equipmentConfigBaseDataList = {}
    --- 圣遗物介绍
    ---@type  table<number,EquipmentDesBaseData>
    self.equipmentDesBaseDataList = {}
    --- 圣遗物位置名字
    ---@type  table<number,EquipmentGrowthConfigBaseData>
    self.equipmentGrowthConfigBaseDataList = {}
    --- 圣遗物副属性
    ---@type  table<number,EquipmentGrowthViceConfigBaseData>
    self.equipmentGrowthViceConfigBaseDataList = {}
    --- 圣遗物位置信息
    ---@type  table<number,EquipmentPrimaryConfigBaseData>
    self.equipmentPrimaryConfigBaseDataList = {}
    --- 武器配置
    ---@type  table<number,WeaponsConfigData>
    self.weaponsConfigDataList = {}
end

---@param bagItemEntityList table<number,ItemBaseData> 背包基础类list
---@param equipmentBaseDataList table<number,EquipmentBaseData> 圣遗物基础配置相关
---@param equipmentConfigBaseDataList table<number,EquipmentConfigBaseData> 装备基础相关根据品节卡等级强化获取副属性条
---@param equipmentDesBaseDataList table<number,EquipmentDesBaseData> 圣遗物介绍
---@param equipmentGrowthConfigBaseDataList table<number,EquipmentGrowthConfigBaseData> 圣遗物位置名字
---@param equipmentGrowthViceConfigBaseDataList table<number,EquipmentGrowthViceConfigBaseData> 圣遗物副属性
---@param equipmentPrimaryConfigBaseDataList table<number,EquipmentPrimaryConfigBaseData> 圣遗物位置信息
---@param weaponsConfigDataList table<number,WeaponsConfigData> 武器配置
---@return ServerConfigResponse
function ServerConfigResponse:new(bagItemEntityList, equipmentBaseDataList, equipmentConfigBaseDataList, equipmentDesBaseDataList, equipmentGrowthConfigBaseDataList, equipmentGrowthViceConfigBaseDataList, equipmentPrimaryConfigBaseDataList, weaponsConfigDataList)
    self.bagItemEntityList = bagItemEntityList --- java.util.List<com.gameServer.common.protocol.serverConfig.ItemBaseData>
    self.equipmentBaseDataList = equipmentBaseDataList --- java.util.List<com.gameServer.common.protocol.equipment.base.EquipmentBaseData>
    self.equipmentConfigBaseDataList = equipmentConfigBaseDataList --- java.util.List<com.gameServer.common.protocol.equipment.base.EquipmentConfigBaseData>
    self.equipmentDesBaseDataList = equipmentDesBaseDataList --- java.util.List<com.gameServer.common.protocol.equipment.base.EquipmentDesBaseData>
    self.equipmentGrowthConfigBaseDataList = equipmentGrowthConfigBaseDataList --- java.util.List<com.gameServer.common.protocol.equipment.base.EquipmentGrowthConfigBaseData>
    self.equipmentGrowthViceConfigBaseDataList = equipmentGrowthViceConfigBaseDataList --- java.util.List<com.gameServer.common.protocol.equipment.base.EquipmentGrowthViceConfigBaseData>
    self.equipmentPrimaryConfigBaseDataList = equipmentPrimaryConfigBaseDataList --- java.util.List<com.gameServer.common.protocol.equipment.base.EquipmentPrimaryConfigBaseData>
    self.weaponsConfigDataList = weaponsConfigDataList --- java.util.List<com.gameServer.common.protocol.weapon.WeaponsConfigData>
    return self
end

---@return number
function ServerConfigResponse:protocolId()
    return 1010
end

---@return string
function ServerConfigResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            bagItemEntityList = self.bagItemEntityList,
            equipmentBaseDataList = self.equipmentBaseDataList,
            equipmentConfigBaseDataList = self.equipmentConfigBaseDataList,
            equipmentDesBaseDataList = self.equipmentDesBaseDataList,
            equipmentGrowthConfigBaseDataList = self.equipmentGrowthConfigBaseDataList,
            equipmentGrowthViceConfigBaseDataList = self.equipmentGrowthViceConfigBaseDataList,
            equipmentPrimaryConfigBaseDataList = self.equipmentPrimaryConfigBaseDataList,
            weaponsConfigDataList = self.weaponsConfigDataList
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return ServerConfigResponse
function ServerConfigResponse:read(data)
    local bagItemEntityList = {}
    for index, value in ipairs(data.bagItemEntityList) do
        local bagItemEntityListPacket = ItemBaseData()
        local packetData = bagItemEntityListPacket:read(value)
        table.insert(bagItemEntityList,packetData)
    end
    local equipmentBaseDataList = {}
    for index, value in ipairs(data.equipmentBaseDataList) do
        local equipmentBaseDataListPacket = EquipmentBaseData()
        local packetData = equipmentBaseDataListPacket:read(value)
        table.insert(equipmentBaseDataList,packetData)
    end
    local equipmentConfigBaseDataList = {}
    for index, value in ipairs(data.equipmentConfigBaseDataList) do
        local equipmentConfigBaseDataListPacket = EquipmentConfigBaseData()
        local packetData = equipmentConfigBaseDataListPacket:read(value)
        table.insert(equipmentConfigBaseDataList,packetData)
    end
    local equipmentDesBaseDataList = {}
    for index, value in ipairs(data.equipmentDesBaseDataList) do
        local equipmentDesBaseDataListPacket = EquipmentDesBaseData()
        local packetData = equipmentDesBaseDataListPacket:read(value)
        table.insert(equipmentDesBaseDataList,packetData)
    end
    local equipmentGrowthConfigBaseDataList = {}
    for index, value in ipairs(data.equipmentGrowthConfigBaseDataList) do
        local equipmentGrowthConfigBaseDataListPacket = EquipmentGrowthConfigBaseData()
        local packetData = equipmentGrowthConfigBaseDataListPacket:read(value)
        table.insert(equipmentGrowthConfigBaseDataList,packetData)
    end
    local equipmentGrowthViceConfigBaseDataList = {}
    for index, value in ipairs(data.equipmentGrowthViceConfigBaseDataList) do
        local equipmentGrowthViceConfigBaseDataListPacket = EquipmentGrowthViceConfigBaseData()
        local packetData = equipmentGrowthViceConfigBaseDataListPacket:read(value)
        table.insert(equipmentGrowthViceConfigBaseDataList,packetData)
    end
    local equipmentPrimaryConfigBaseDataList = {}
    for index, value in ipairs(data.equipmentPrimaryConfigBaseDataList) do
        local equipmentPrimaryConfigBaseDataListPacket = EquipmentPrimaryConfigBaseData()
        local packetData = equipmentPrimaryConfigBaseDataListPacket:read(value)
        table.insert(equipmentPrimaryConfigBaseDataList,packetData)
    end
    local weaponsConfigDataList = {}
    for index, value in ipairs(data.weaponsConfigDataList) do
        local weaponsConfigDataListPacket = WeaponsConfigData()
        local packetData = weaponsConfigDataListPacket:read(value)
        table.insert(weaponsConfigDataList,packetData)
    end

    local packet = self:new(
            bagItemEntityList,
            equipmentBaseDataList,
            equipmentConfigBaseDataList,
            equipmentDesBaseDataList,
            equipmentGrowthConfigBaseDataList,
            equipmentGrowthViceConfigBaseDataList,
            equipmentPrimaryConfigBaseDataList,
            weaponsConfigDataList)
    return packet
end

--- 背包基础类list
---@type  table<number,ItemBaseData> 背包基础类list
function ServerConfigResponse:getBagItemEntityList()
    return self.bagItemEntityList
end
--- 圣遗物基础配置相关
---@type  table<number,EquipmentBaseData> 圣遗物基础配置相关
function ServerConfigResponse:getEquipmentBaseDataList()
    return self.equipmentBaseDataList
end
--- 装备基础相关根据品节卡等级强化获取副属性条
---@type  table<number,EquipmentConfigBaseData> 装备基础相关根据品节卡等级强化获取副属性条
function ServerConfigResponse:getEquipmentConfigBaseDataList()
    return self.equipmentConfigBaseDataList
end
--- 圣遗物介绍
---@type  table<number,EquipmentDesBaseData> 圣遗物介绍
function ServerConfigResponse:getEquipmentDesBaseDataList()
    return self.equipmentDesBaseDataList
end
--- 圣遗物位置名字
---@type  table<number,EquipmentGrowthConfigBaseData> 圣遗物位置名字
function ServerConfigResponse:getEquipmentGrowthConfigBaseDataList()
    return self.equipmentGrowthConfigBaseDataList
end
--- 圣遗物副属性
---@type  table<number,EquipmentGrowthViceConfigBaseData> 圣遗物副属性
function ServerConfigResponse:getEquipmentGrowthViceConfigBaseDataList()
    return self.equipmentGrowthViceConfigBaseDataList
end
--- 圣遗物位置信息
---@type  table<number,EquipmentPrimaryConfigBaseData> 圣遗物位置信息
function ServerConfigResponse:getEquipmentPrimaryConfigBaseDataList()
    return self.equipmentPrimaryConfigBaseDataList
end
--- 武器配置
---@type  table<number,WeaponsConfigData> 武器配置
function ServerConfigResponse:getWeaponsConfigDataList()
    return self.weaponsConfigDataList
end


return ServerConfigResponse
