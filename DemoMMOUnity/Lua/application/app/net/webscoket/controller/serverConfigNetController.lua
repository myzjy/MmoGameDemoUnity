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

function ServerConfigNetController:DeleteAllItemBaseDataList()
    ItemBaseDataList = {}
end

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

return ServerConfigNetController
