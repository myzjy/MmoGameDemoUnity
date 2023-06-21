---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2023/6/14 23:26
---

local ItemBaseData = class("ItemBaseData")

local id
local name
local icon
local minNum
local maxNum
local typeNum
local des

function ItemBaseData:new()
    local obj = {}
    setmetatable(obj, self)
    obj.__index = self
    return obj
end

function ItemBaseData:ValueOf(_id, _name, _icon, _minNum, _maxNum, _type, _des)
    self:SetIDValue(_id)
    self:SetNameValue(_name)
    --self:Set
end

function ItemBaseData:SetIDValue(_id)
    id = _id
end

function ItemBaseData:GetIDValue()
    return id
end

function ItemBaseData:SetNameValue(Name)
    name = Name
end

function ItemBaseData:GetNameValue()
    return name
end

function ItemBaseData:SetMinNumValue(_minNum)
    minNum = _minNum
end

function ItemBaseData:GetMinNumValue()
    return minNum
end

function ItemBaseData:SetMaxNumValue(MaxNum)
    maxNum = MaxNum
end

function ItemBaseData:GetMaxNumValue()
    return maxNum
end

--- 道具类型
---@param TypeNum number 道具类型
function ItemBaseData:SetTypeValue(TypeNum)
    typeNum = TypeNum
end

--- 返回道具类型 int 类型
---@return number 道具类型
function ItemBaseData:GetTypeValue()
    return typeNum
end

---设置item 介绍
---@return string 介绍
function ItemBaseData:SetDesValue(Des)
    des = Des
end

---介绍 string
---@return string
function ItemBaseData:GetDesValue()
    return des
end

--- 设置 item Icon 名
---@param iconString string icon名
function ItemBaseData:SetIconStringValue(iconString)
    icon = iconString
end

--- 返回item icon 名
---@return string
function ItemBaseData:GetIconStringValue()
    return icon
end

return ItemBaseData

