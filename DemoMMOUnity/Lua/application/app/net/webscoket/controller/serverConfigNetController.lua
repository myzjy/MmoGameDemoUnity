---@class ServerConfigNetController
local ServerConfigNetController = class("ServerConfigNetController")

local ItemBaseDataList = {}

---@param itemBaseDataList table<integer,ItemBaseData>
function ServerConfigNetController:SetItemBaseDataList(itemBaseDataList)
    for index, value in ipairs(itemBaseDataList) do
        ---@type {id:integer,name:string,icon:string,minNum:integer,maxNum:integer,type:integer,des:string}
        local item = value
        ItemBaseDataList[item.id] = value
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
        EquipmentConfigBaseDataList[item.quality] = value
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

---@param equipmentBaseDataList table<integer,EquipmentBaseData>
function ServerConfigNetController:SetEquipmentBaseDataList(equipmentBaseDataList)
    EquipmentBaseDataList = {}
    for index, value in ipairs(equipmentBaseDataList) do
        ---@type {desId:number, quality:number, equipmentPosType:number,equipmentName:string,mainAttributes:string}
        local item = value
        EquipmentBaseDataList[item.desId] = value
    end
end

---@param equipmentPrimaryConfigBaseDataList table<integer,EquipmentPrimaryConfigBaseData>
function ServerConfigNetController:SetEquipmentPrimaryConfigBaseDataList(equipmentPrimaryConfigBaseDataList)
    EquipmentPrimaryConfigBaseDataList = {}
    for index, value in ipairs(equipmentPrimaryConfigBaseDataList) do
        ---@type {id:integer,primaryQuality:integer,growthPosInt:integer,primaryGrowthName:string,primaryGrowthInts:string,primaryGrowthMaxInt:string,growthPosName:string}
        local item = value
        EquipmentPrimaryConfigBaseDataList[item.id] = value
    end
end

---@param equipmentDesBaseDataList table<integer,EquipmentDesBaseData>
function ServerConfigNetController:SetEquipmentDesBaseDataList(equipmentDesBaseDataList)
    EquipmentDesBaseDataList = {}
    for index, value in ipairs(equipmentDesBaseDataList) do
        ---@type {desId:number,name:string,desStr:string,toryDesStr:string}
        local item = value
        EquipmentDesBaseDataList[item.desId] = value
    end
end

---@param equipmentGrowthConfigBaseDataList table<integer,EquipmentGrowthConfigBaseData>
function ServerConfigNetController:SetEquipmentGrowthConfigBaseDataList(equipmentGrowthConfigBaseDataList)
    EquipmentGrowthConfigBaseDataList = {}
    for index, value in ipairs(equipmentGrowthConfigBaseDataList) do
        ---@type {id:number, locationOfEquipmentType:number, posName:string}
        local item = value
        EquipmentGrowthConfigBaseDataList[item.id] = value
    end
end
---@param equipmentGrowthViceConfigBaseDataList table<integer,EquipmentGrowthViceConfigBaseData>
function ServerConfigNetController:SetEquipmentGrowthViceConfigBaseDataList(equipmentGrowthViceConfigBaseDataList)
    EquipmentGrowthViceConfigBaseDataList = {}
    for index, value in ipairs(equipmentGrowthViceConfigBaseDataList) do
        ---@type {viceId:number, viceName:string, posGrowthType:integer,initNums:table<integer,string>}
        local item = value
        EquipmentGrowthViceConfigBaseDataList[item.viceId] = value
    end
end
return ServerConfigNetController
