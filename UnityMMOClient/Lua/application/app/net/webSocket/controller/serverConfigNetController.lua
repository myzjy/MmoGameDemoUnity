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
    for _, value in ipairs(itemBaseDataList) do
        ---@type {id:number,name:string,icon:string,minNum:number,maxNum:number,type:number,des:string}
        local item = value
        ---@type ItemBaseData
        local data = ItemBaseData()
        data = data:read(item)
        self.itemBaseDataList[item.id] = data
    end
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
    self.equipmentConfigBaseDataList = {}
    for _, value in ipairs(equipmentConfigBaseDataList) do
        ---@type EquipmentConfigBaseData
        local item = value
        ---@type EquipmentConfigBaseData
        local data = EquipmentConfigBaseData()
        data = data:readData(item)
        self.equipmentConfigBaseDataList[item.quality] = data
        table.insert()
    end
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
    self.equipmentBaseDataList = {}
    for index, value in ipairs(equipmentBaseDataList) do
        ---@type {desId:number, quality:number, equipmentPosType:number,equipmentName:string,mainAttributes:string}
        local item = value
        ---@type EquipmentBaseData
        local data = EquipmentBaseData()
        data = data:readData(item)
        self.equipmentBaseDataList[item.desId] = data
    end
end

---@param equipmentPrimaryConfigBaseDataList table<number,EquipmentPrimaryConfigBaseData>
function ServerConfigNetController:SetEquipmentPrimaryConfigBaseDataList(equipmentPrimaryConfigBaseDataList)
    self.equipmentPrimaryConfigBaseDataList = {}
    for index, value in ipairs(equipmentPrimaryConfigBaseDataList) do
        ---@type {id:number,primaryQuality:number,growthPosInt:number,primaryGrowthName:string,primaryGrowthInts:string,primaryGrowthMaxInt:string,growthPosName:string}
        local item = value
        ---@type EquipmentPrimaryConfigBaseData
        local data = EquipmentPrimaryConfigBaseData()
        data = data:readData(item)
        self.equipmentPrimaryConfigBaseDataList[item.id] = data
    end
end

---@param equipmentDesBaseDataList table<number,EquipmentDesBaseData>
function ServerConfigNetController:SetEquipmentDesBaseDataList(equipmentDesBaseDataList)
    self.equipmentDesBaseDataList = {}
    for index, value in ipairs(equipmentDesBaseDataList) do
        ---@type {desId:number,name:string,desStr:string,toryDesStr:string}
        local item = value
        ---@type EquipmentDesBaseData
        local data = EquipmentDesBaseData()
        data = data:readData(item)
        self.equipmentDesBaseDataList[item.desId] = data
    end
end

---@param equipmentGrowthConfigBaseDataList table<number,EquipmentGrowthConfigBaseData>
function ServerConfigNetController:SetEquipmentGrowthConfigBaseDataList(equipmentGrowthConfigBaseDataList)
    self.equipmentGrowthConfigBaseDataList = {}
    for index, value in ipairs(equipmentGrowthConfigBaseDataList) do
        ---@type {id:number, locationOfEquipmentType:number, posName:string}
        local item = value
        ---@type EquipmentGrowthConfigBaseData
        local data = EquipmentGrowthConfigBaseData()
        data = data:readData(item)
        self.equipmentGrowthConfigBaseDataList[item.id] = data
    end
end

---@param equipmentGrowthViceConfigBaseDataList table<number,EquipmentGrowthViceConfigBaseData>
function ServerConfigNetController:SetEquipmentGrowthViceConfigBaseDataList(equipmentGrowthViceConfigBaseDataList)
    self.equipmentGrowthViceConfigBaseDataList = {}
    for index, value in ipairs(equipmentGrowthViceConfigBaseDataList) do
        ---@type {viceId:number, viceName:string, posGrowthType:number,initNums:table<number,string>}
        local item = value
        ---@type EquipmentGrowthViceConfigBaseData
        local data = EquipmentGrowthViceConfigBaseData()
        data = data:readData(item)
        self.equipmentGrowthViceConfigBaseDataList[item.viceId] = data
    end
end

---@param weaponsConfigDataList table<number,WeaponsConfigData>
function ServerConfigNetController:SetWeaponsConfigDataList(weaponsConfigDataList)
    self.weaponsConfigDataList = {}
    for index, value in ipairs(weaponsConfigDataList) do
        local item = value
        ---@type WeaponsConfigData
        local data = WeaponsConfigData()
        data = data:readData(item)
        self.weaponsConfigDataList[item.id] = data
    end
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