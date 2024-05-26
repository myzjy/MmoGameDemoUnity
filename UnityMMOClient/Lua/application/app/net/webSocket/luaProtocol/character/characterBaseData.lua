---@class CharacterBaseData
local CharacterBaseData = class("CharacterBaseData")
function CharacterBaseData:ctor()
    --- 角色装备武器
    ---@type CharacterWeaponIDData
    self.characterWeaponIDData = {}
    --- 元素伤害数据百分比的
    ---@type number
    self.elementNum = 0
    --- 角色元素伤害类型
    ---@type number
    self.elementType = 0
    --- 装备圣遗物id数据库中的id
    ---@type  table<number,CharacterEquipmentIDData>
    self.equipmentList = {}
    --- 这个角色index
    ---@type number
    self.id = 0
    --- 等级
    ---@type number
    self.lv = 0
    --- 等级星级数量
    ---@type number
    self.lvQuantity = 0
    --- 等级最大星级数量
    ---@type number
    self.maxLvQuantity = 0
    --- 经验
    ---@type number
    self.nowExp = 0
    --- 当前最大经验
    ---@type number
    self.nowMaxExp = 0
    --- 当前最大的等级
    ---@type number
    self.nowMaxLv = 0
    --- 角色品质
    ---@type number
    self.quantity = 0
    --- 角色id获取数据库中得基础信息
    ---@type number
    self.roleID = 0
end

---@param characterWeaponIDData CharacterWeaponIDData 角色装备武器
---@param elementNum number 元素伤害数据百分比的
---@param elementType number 角色元素伤害类型
---@param equipmentList table<number,CharacterEquipmentIDData> 装备圣遗物id数据库中的id
---@param id number 这个角色index
---@param lv number 等级
---@param lvQuantity number 等级星级数量
---@param maxLvQuantity number 等级最大星级数量
---@param nowExp number 经验
---@param nowMaxExp number 当前最大经验
---@param nowMaxLv number 当前最大的等级
---@param quantity number 角色品质
---@param roleID number 角色id获取数据库中得基础信息
---@return CharacterBaseData
function CharacterBaseData:new(characterWeaponIDData, elementNum, elementType, equipmentList, id, lv, lvQuantity, maxLvQuantity, nowExp, nowMaxExp, nowMaxLv, quantity, roleID)
    self.characterWeaponIDData = characterWeaponIDData --- com.gameServer.common.protocol.character.CharacterWeaponIDData
    self.elementNum = elementNum --- float
    self.elementType = elementType --- int
    self.equipmentList = equipmentList --- java.util.List<com.gameServer.common.protocol.character.CharacterEquipmentIDData>
    self.id = id --- long
    self.lv = lv --- int
    self.lvQuantity = lvQuantity --- int
    self.maxLvQuantity = maxLvQuantity --- int
    self.nowExp = nowExp --- int
    self.nowMaxExp = nowMaxExp --- int
    self.nowMaxLv = nowMaxLv --- int
    self.quantity = quantity --- int
    self.roleID = roleID --- long
    return self
end

---@return number
function CharacterBaseData:protocolId()
    return 216
end

---@return string
function CharacterBaseData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            characterWeaponIDData = self.characterWeaponIDData,
            elementNum = self.elementNum,
            elementType = self.elementType,
            equipmentList = self.equipmentList,
            id = self.id,
            lv = self.lv,
            lvQuantity = self.lvQuantity,
            maxLvQuantity = self.maxLvQuantity,
            nowExp = self.nowExp,
            nowMaxExp = self.nowMaxExp,
            nowMaxLv = self.nowMaxLv,
            quantity = self.quantity,
            roleID = self.roleID
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return CharacterBaseData
function CharacterBaseData:read(data)
    local characterWeaponIDDataPacket = CharacterWeaponIDData()
    local characterWeaponIDData = characterWeaponIDDataPacket:read(data.characterWeaponIDData)
    local equipmentList = {}
    for index, value in ipairs(data.equipmentList) do
        local equipmentListPacket = CharacterEquipmentIDData()
        local packetData = equipmentListPacket:read(value)
        table.insert(equipmentList,packetData)
    end

    local packet = self:new(
            characterWeaponIDData,
            data.elementNum,
            data.elementType,
            equipmentList,
            data.id,
            data.lv,
            data.lvQuantity,
            data.maxLvQuantity,
            data.nowExp,
            data.nowMaxExp,
            data.nowMaxLv,
            data.quantity,
            data.roleID)
    return packet
end

--- 角色装备武器
---@return CharacterWeaponIDData 角色装备武器
function CharacterBaseData:getCharacterWeaponIDData()
    return self.characterWeaponIDData
end
--- 元素伤害数据百分比的
---@return number 元素伤害数据百分比的
function CharacterBaseData:getElementNum()
    return self.elementNum
end
--- 角色元素伤害类型
---@return number 角色元素伤害类型
function CharacterBaseData:getElementType()
    return self.elementType
end
--- 装备圣遗物id数据库中的id
---@type  table<number,CharacterEquipmentIDData> 装备圣遗物id数据库中的id
function CharacterBaseData:getEquipmentList()
    return self.equipmentList
end
--- 这个角色index
---@return number 这个角色index
function CharacterBaseData:getId()
    return self.id
end
--- 等级
---@return number 等级
function CharacterBaseData:getLv()
    return self.lv
end
--- 等级星级数量
---@return number 等级星级数量
function CharacterBaseData:getLvQuantity()
    return self.lvQuantity
end
--- 等级最大星级数量
---@return number 等级最大星级数量
function CharacterBaseData:getMaxLvQuantity()
    return self.maxLvQuantity
end
--- 经验
---@return number 经验
function CharacterBaseData:getNowExp()
    return self.nowExp
end
--- 当前最大经验
---@return number 当前最大经验
function CharacterBaseData:getNowMaxExp()
    return self.nowMaxExp
end
--- 当前最大的等级
---@return number 当前最大的等级
function CharacterBaseData:getNowMaxLv()
    return self.nowMaxLv
end
--- 角色品质
---@return number 角色品质
function CharacterBaseData:getQuantity()
    return self.quantity
end
--- 角色id获取数据库中得基础信息
---@return number 角色id获取数据库中得基础信息
function CharacterBaseData:getRoleID()
    return self.roleID
end


return CharacterBaseData
