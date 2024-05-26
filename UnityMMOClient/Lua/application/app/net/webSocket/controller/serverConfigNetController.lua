---@class ServerConfigNetController
local ServerConfigNetController = class("ServerConfigNetController")

---@type ServerConfigNetController
local instance = nil

function ServerConfigNetController.GetInstance()
    if not instance then
        -- body
        instance = ServerConfigNetController()
    end
    return instance
end

function ServerConfigNetController:ctor()
    PrintDebug("init config and serverData controller")
    ---@type  table<number,ItemBaseData>
    self.itemBaseDataList = {}
    ---@type  table<number,EquipmentConfigBaseData>
    self.equipmentConfigBaseDataList = {}
    ---@type  table<number,EquipmentBaseData>
    self.equipmentBaseDataList = {}
    ---@type  table<number,EquipmentPrimaryConfigBaseData>
    self.equipmentPrimaryConfigBaseDataList = {}
    ---@type  table<number,EquipmentDesBaseData>
    self.equipmentDesBaseDataList = {}
    ---@type  table<number,EquipmentGrowthConfigBaseData>
    self.equipmentGrowthConfigBaseDataList = {}
    ---@type  table<number,EquipmentGrowthViceConfigBaseData>
    self.equipmentGrowthViceConfigBaseDataList = {}
    ---@type  table<number,WeaponsConfigData>
    self.weaponsConfigDataList = {}
    ---@type PlayerSceneInfoData
    self.playerSceneInfoData = nil;
    ---@type UserMsgInfoData
    self.userMsgInfoData = nil;
    ---@type {token:string,uid:number}
    self.playerInfo = {}
    ---@type PhysicalPowerInfoData
    self.physicalPowerInfo = nil;
end

function ServerConfigNetController:RegisterEvent()
    PacketDispatcher:AddProtocolConfigEvent(ServerConfigResponse:protocolId(), handle(self.SetServerConfigDataList, self))
end

function ServerConfigNetController:SetServerConfigDataList(response)
    local packet = ServerConfigResponse()
    ---@type ServerConfigResponse
    local protocolData = packet:read(response)

    self:SetItemBaseDataList(protocolData.bagItemEntityList)
    self:SetEquipmentConfigBaseDataList(protocolData.equipmentConfigBaseDataList)
    self:SetEquipmentDesBaseDataList(protocolData.equipmentDesBaseDataList)
    self:SetEquipmentBaseDataList(protocolData.equipmentBaseDataList)
    self:SetEquipmentGrowthConfigBaseDataList(protocolData.equipmentGrowthConfigBaseDataList)
    self:SetEquipmentPrimaryConfigBaseDataList(protocolData.equipmentPrimaryConfigBaseDataList)
    self:SetEquipmentGrowthViceConfigBaseDataList(protocolData
            .equipmentGrowthViceConfigBaseDataList)
    self:SetWeaponsConfigDataList(protocolData.weaponsConfigDataList)
    GameMainUIViewController:GetInstance():Open()
end

---@param itemBaseDataList table<number,ItemBaseData>
function ServerConfigNetController:SetItemBaseDataList(itemBaseDataList)
    self.itemBaseDataList = itemBaseDataList
end

---@param key number
---@return ItemBaseData
function ServerConfigNetController:GetItemBaseDataConfigKey(key)
    local keyData = self.itemBaseDataList[key]
    if keyData == null then
        return null
    end
    return keyData
end

---清空 ItemBaseDataList 数据库
function ServerConfigNetController:DeleteAllItemBaseDataList()
    self.itemBaseDataList = {}
end

---更具 key 删除 对应 制
---@param key number
---@return boolean
function ServerConfigNetController:DeleteKeyItemBaseDataList(key)
    local keyData = self.itemBaseDataList[key]
    if keyData == null then
        return false
    end
    self.itemBaseDataList[key] = null
    return true
end

---@param equipmentConfigBaseDataList table<number,EquipmentConfigBaseData>
function ServerConfigNetController:SetEquipmentConfigBaseDataList(equipmentConfigBaseDataList)
    self.equipmentConfigBaseDataList = equipmentConfigBaseDataList
end

---清空 EquipmentConfigBaseDataList 数据库
function ServerConfigNetController:DeleteAllEquipmentConfigBaseDataList()
    table.Clear(self.equipmentConfigBaseDataList)
end

---@param key number
---@return EquipmentConfigBaseData
function ServerConfigNetController:DeleteKeyEquipmentConfigBaseDataList(key)
    local keyData = self.equipmentConfigBaseDataList[key]
    if keyData == null then
        return null
    end
    return keyData
end

---@param equipmentBaseDataList table<number,EquipmentBaseData>
function ServerConfigNetController:SetEquipmentBaseDataList(equipmentBaseDataList)
    self.equipmentBaseDataList = equipmentBaseDataList
end

---@param equipmentPrimaryConfigBaseDataList table<number,EquipmentPrimaryConfigBaseData>
function ServerConfigNetController:SetEquipmentPrimaryConfigBaseDataList(equipmentPrimaryConfigBaseDataList)
    self.equipmentPrimaryConfigBaseDataList = equipmentPrimaryConfigBaseDataList
end

---@param equipmentDesBaseDataList table<number,EquipmentDesBaseData>
function ServerConfigNetController:SetEquipmentDesBaseDataList(equipmentDesBaseDataList)
    self.equipmentDesBaseDataList = equipmentDesBaseDataList
end

---@param equipmentGrowthConfigBaseDataList table<number,EquipmentGrowthConfigBaseData>
function ServerConfigNetController:SetEquipmentGrowthConfigBaseDataList(equipmentGrowthConfigBaseDataList)
    self.equipmentGrowthConfigBaseDataList = equipmentGrowthConfigBaseDataList
end

---@param equipmentGrowthViceConfigBaseDataList table<number,EquipmentGrowthViceConfigBaseData>
function ServerConfigNetController:SetEquipmentGrowthViceConfigBaseDataList(equipmentGrowthViceConfigBaseDataList)
    self.equipmentGrowthViceConfigBaseDataList = equipmentGrowthViceConfigBaseDataList
end

---@param weaponsConfigDataList table<number,WeaponsConfigData>
function ServerConfigNetController:SetWeaponsConfigDataList(weaponsConfigDataList)
    self.weaponsConfigDataList = weaponsConfigDataList
end

---@param playerSceneInfoData PlayerSceneInfoData
function ServerConfigNetController:SetPlayerSceneInfoData(playerSceneInfoData)
    self.playerSceneInfoData = playerSceneInfoData
end

---@return PlayerSceneInfoData
function ServerConfigNetController:GetPlayerSceneInfoData()
    return self.playerSceneInfoData
end

---@param userMsgInfoData UserMsgInfoData
function ServerConfigNetController:SetUserMsgInfoData(userMsgInfoData)
    self.userMsgInfoData = userMsgInfoData
end

---@return UserMsgInfoData
function ServerConfigNetController:GetUserMsgInfoData()
    return self.userMsgInfoData;
end

function ServerConfigNetController:SetPlayerInfo(infoData)
    self.playerInfo = infoData;
end

---@return  {token:string,uid:number}
function ServerConfigNetController:GetPlayerInfo()
    return self.playerInfo;
end

--- comment
--- @param physicalPowerInfoData PhysicalPowerInfoData
function ServerConfigNetController:SetPhysicalPowerInfoData(physicalPowerInfoData)
    self.physicalPowerInfo = physicalPowerInfoData
end

--- 体力数据
---@return PhysicalPowerInfoData
function ServerConfigNetController:GetPhysicalPowerInfoData()
    return self.physicalPowerInfo;
end

return ServerConfigNetController