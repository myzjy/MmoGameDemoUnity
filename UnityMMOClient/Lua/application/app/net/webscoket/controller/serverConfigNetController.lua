---@class ServerConfigNetController
local ServerConfigNetController = class("ServerConfigNetController")

local ItemBaseDataList = {}
---@type ServerConfigNetController
local instance = nil

function ServerConfigNetController.GetInstance()
    if not instance then
        -- body
        instance = ServerConfigNetController()
    end
    return instance
end

function ServerConfigNetController:RegisterEvent()
    PacketDispatcher:AddProtocolConfigEvent(ServerConfigResponse:protocolId(), handle(self.SetServerConfigDataList, self))
end

function ServerConfigNetController:SetServerConfigDataList(response)
    local packet=ServerConfigResponse()
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

---@param itemBaseDataList table<integer,ItemBaseData>
function ServerConfigNetController:SetItemBaseDataList(itemBaseDataList)
    for index, value in ipairs(itemBaseDataList) do
        ---@type {id:integer,name:string,icon:string,minNum:integer,maxNum:integer,type:integer,des:string}
        local item = value
        ---@type ItemBaseData
        local data = ItemBaseData()
        data = data:read(item)
        ItemBaseDataList[item.id] = data
    end
end

---@param key integer
---@return ItemBaseData
function ServerConfigNetController:GetItemBaseDataConfigKey(key)
    local keyData = ItemBaseDataList[key]
    if keyData == null then
        return null
    end
    return keyData
end

---清空 ItemBaseDataList 数据库
function ServerConfigNetController:DeleteAllItemBaseDataList()
    ItemBaseDataList = {}
end

---更具 key 删除 对应 制
---@param key number
---@return boolean
function ServerConfigNetController:DeleteKeyItemBaseDataList(key)
    local keyData = ItemBaseDataList[key]
    if keyData == null then
        return false
    end
    ItemBaseDataList[key] = null
    return true
end

local EquipmentConfigBaseDataList = {}

---@param equipmentConfigBaseDataList table<integer,EquipmentConfigBaseData>
function ServerConfigNetController:SetEquipmentConfigBaseDataList(equipmentConfigBaseDataList)
    EquipmentConfigBaseDataList = {}
    for index, value in ipairs(equipmentConfigBaseDataList) do
        ---@type {quality:number, lv1:number, lv2:number, lv3:number, lv4:number}
        local item = value
        ---@type EquipmentConfigBaseData
        local data = EquipmentConfigBaseData()
        data = data:readData(item)
        EquipmentConfigBaseDataList[item.quality] = data
    end
end

---清空 EquipmentConfigBaseDataList 数据库
function ServerConfigNetController:DeleteAllEquipmentConfigBaseDataList()
    table.Clear(EquipmentConfigBaseDataList)
end

---@param key integer
---@return EquipmentConfigBaseData
function ServerConfigNetController:DeleteKeyEquipmentConfigBaseDataList(key)
    local keyData = EquipmentConfigBaseDataList[key]
    if keyData == null then
        return null
    end
    return keyData
end

local EquipmentBaseDataList = {}
local EquipmentPrimaryConfigBaseDataList = {}
local EquipmentDesBaseDataList = {}
local EquipmentGrowthConfigBaseDataList = {}
local EquipmentGrowthViceConfigBaseDataList = {}
local WeaponsConfigDataList = {}

---@param equipmentBaseDataList table<integer,EquipmentBaseData>
function ServerConfigNetController:SetEquipmentBaseDataList(equipmentBaseDataList)
    EquipmentBaseDataList = {}
    for index, value in ipairs(equipmentBaseDataList) do
        ---@type {desId:number, quality:number, equipmentPosType:number,equipmentName:string,mainAttributes:string}
        local item = value
        ---@type EquipmentBaseData
        local data = EquipmentBaseData()
        data = data:readData(item)
        EquipmentBaseDataList[item.desId] = data
    end
end

---@param equipmentPrimaryConfigBaseDataList table<integer,EquipmentPrimaryConfigBaseData>
function ServerConfigNetController:SetEquipmentPrimaryConfigBaseDataList(equipmentPrimaryConfigBaseDataList)
    EquipmentPrimaryConfigBaseDataList = {}
    for index, value in ipairs(equipmentPrimaryConfigBaseDataList) do
        ---@type {id:integer,primaryQuality:integer,growthPosInt:integer,primaryGrowthName:string,primaryGrowthInts:string,primaryGrowthMaxInt:string,growthPosName:string}
        local item = value
        ---@type EquipmentPrimaryConfigBaseData
        local data = EquipmentPrimaryConfigBaseData()
        data = data:readData(item)
        EquipmentPrimaryConfigBaseDataList[item.id] = data
    end
end

---@param equipmentDesBaseDataList table<integer,EquipmentDesBaseData>
function ServerConfigNetController:SetEquipmentDesBaseDataList(equipmentDesBaseDataList)
    EquipmentDesBaseDataList = {}
    for index, value in ipairs(equipmentDesBaseDataList) do
        ---@type {desId:number,name:string,desStr:string,toryDesStr:string}
        local item = value
        ---@type EquipmentDesBaseData
        local data = EquipmentDesBaseData()
        data = data:readData(item)
        EquipmentDesBaseDataList[item.desId] = data
    end
end

---@param equipmentGrowthConfigBaseDataList table<integer,EquipmentGrowthConfigBaseData>
function ServerConfigNetController:SetEquipmentGrowthConfigBaseDataList(equipmentGrowthConfigBaseDataList)
    EquipmentGrowthConfigBaseDataList = {}
    for index, value in ipairs(equipmentGrowthConfigBaseDataList) do
        ---@type {id:number, locationOfEquipmentType:number, posName:string}
        local item = value
        ---@type EquipmentGrowthConfigBaseData
        local data = EquipmentGrowthConfigBaseData()
        data = data:readData(item)
        EquipmentGrowthConfigBaseDataList[item.id] = data
    end
end

---@param equipmentGrowthViceConfigBaseDataList table<integer,EquipmentGrowthViceConfigBaseData>
function ServerConfigNetController:SetEquipmentGrowthViceConfigBaseDataList(equipmentGrowthViceConfigBaseDataList)
    EquipmentGrowthViceConfigBaseDataList = {}
    for index, value in ipairs(equipmentGrowthViceConfigBaseDataList) do
        ---@type {viceId:number, viceName:string, posGrowthType:integer,initNums:table<integer,string>}
        local item = value
        ---@type EquipmentGrowthViceConfigBaseData
        local data = EquipmentGrowthViceConfigBaseData()
        data = data:readData(item)
        EquipmentGrowthViceConfigBaseDataList[item.viceId] = data
    end
end

---@param weaponsConfigDataList table<integer,WeaponsConfigData>
function ServerConfigNetController:SetWeaponsConfigDataList(weaponsConfigDataList)
    WeaponsConfigDataList = {}
    for index, value in ipairs(weaponsConfigDataList) do
        local item = value
        ---@type WeaponsConfigData
        local data = WeaponsConfigData()
        data = data:readData(item)
        WeaponsConfigDataList[item.id] = data
    end
end

return ServerConfigNetController
