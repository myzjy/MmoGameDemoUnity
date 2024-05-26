---@class CharacterConfigData
local CharacterConfigData = class("CharacterConfigData")
function CharacterConfigData:ctor()
    --- 这个角色ConfigIndex
    ---@type number
    self.CId = 0
    --- 1级的攻击属性
    ---@type number
    self.Level1Atk = 0
    --- 1级元素充能效率
    ---@type number
    self.Level1ChargingEfficiencyOfElements = 0
    --- 1级的暴击率
    ---@type number
    self.Level1CriticalHitChance = 0
    --- 1级暴击伤害
    ---@type number
    self.Level1CriticalHitDamage = 0
    --- 1级防御力
    ---@type number
    self.Level1Def = 0
    --- 1级元素精通
    ---@type number
    self.Level1ElementMastery = 0
    --- 1级的生命值
    ---@type number
    self.Level1HpValue = 0
    --- 当前等级没有装备武器攻击属性
    ---@type number
    self.Level1NoWAtk = 0
    --- 头像
    ---@type string
    self.bagClickIcon = string.empty
    --- 缩小头像
    ---@type string
    self.bagSideIcon = string.empty
    --- 角色默认武器id
    ---@type number
    self.characterDefaultWeaponId = 0
    --- 角色id
    ---@type number
    self.characterId = 0
    --- 角色名
    ---@type string
    self.characterName = string.empty
    --- 角色资源名
    ---@type string
    self.characterRes = string.empty
    --- 角色元素伤害类型
    ---@type number
    self.elementType = 0
    --- 角色创建出来初始最大等级
    ---@type number
    self.initLvMax = 0
    --- 角色创建出来的初始等级
    ---@type number
    self.lvInit = 0
    --- 当前强化的星阶最大
    ---@type number
    self.maxReinforcementEqualOrder = 0
    --- 品质
    ---@type number
    self.quality = 0
    --- 角色默认武器是什么类型
    ---@type number
    self.weaponType = 0
end

---@param CId number 这个角色ConfigIndex
---@param Level1Atk number 1级的攻击属性
---@param Level1ChargingEfficiencyOfElements number 1级元素充能效率
---@param Level1CriticalHitChance number 1级的暴击率
---@param Level1CriticalHitDamage number 1级暴击伤害
---@param Level1Def number 1级防御力
---@param Level1ElementMastery number 1级元素精通
---@param Level1HpValue number 1级的生命值
---@param Level1NoWAtk number 当前等级没有装备武器攻击属性
---@param bagClickIcon string 头像
---@param bagSideIcon string 缩小头像
---@param characterDefaultWeaponId number 角色默认武器id
---@param characterId number 角色id
---@param characterName string 角色名
---@param characterRes string 角色资源名
---@param elementType number 角色元素伤害类型
---@param initLvMax number 角色创建出来初始最大等级
---@param lvInit number 角色创建出来的初始等级
---@param maxReinforcementEqualOrder number 当前强化的星阶最大
---@param quality number 品质
---@param weaponType number 角色默认武器是什么类型
---@return CharacterConfigData
function CharacterConfigData:new(CId, Level1Atk, Level1ChargingEfficiencyOfElements, Level1CriticalHitChance, Level1CriticalHitDamage, Level1Def, Level1ElementMastery, Level1HpValue, Level1NoWAtk, bagClickIcon, bagSideIcon, characterDefaultWeaponId, characterId, characterName, characterRes, elementType, initLvMax, lvInit, maxReinforcementEqualOrder, quality, weaponType)
    self.CId = CId --- int
    self.Level1Atk = Level1Atk --- int
    self.Level1ChargingEfficiencyOfElements = Level1ChargingEfficiencyOfElements --- int
    self.Level1CriticalHitChance = Level1CriticalHitChance --- int
    self.Level1CriticalHitDamage = Level1CriticalHitDamage --- int
    self.Level1Def = Level1Def --- int
    self.Level1ElementMastery = Level1ElementMastery --- int
    self.Level1HpValue = Level1HpValue --- int
    self.Level1NoWAtk = Level1NoWAtk --- int
    self.bagClickIcon = bagClickIcon --- java.lang.String
    self.bagSideIcon = bagSideIcon --- java.lang.String
    self.characterDefaultWeaponId = characterDefaultWeaponId --- int
    self.characterId = characterId --- int
    self.characterName = characterName --- java.lang.String
    self.characterRes = characterRes --- java.lang.String
    self.elementType = elementType --- int
    self.initLvMax = initLvMax --- int
    self.lvInit = lvInit --- int
    self.maxReinforcementEqualOrder = maxReinforcementEqualOrder --- int
    self.quality = quality --- int
    self.weaponType = weaponType --- int
    return self
end

---@return number
function CharacterConfigData:protocolId()
    return 224
end

---@return string
function CharacterConfigData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            CId = self.CId,
            Level1Atk = self.Level1Atk,
            Level1ChargingEfficiencyOfElements = self.Level1ChargingEfficiencyOfElements,
            Level1CriticalHitChance = self.Level1CriticalHitChance,
            Level1CriticalHitDamage = self.Level1CriticalHitDamage,
            Level1Def = self.Level1Def,
            Level1ElementMastery = self.Level1ElementMastery,
            Level1HpValue = self.Level1HpValue,
            Level1NoWAtk = self.Level1NoWAtk,
            bagClickIcon = self.bagClickIcon,
            bagSideIcon = self.bagSideIcon,
            characterDefaultWeaponId = self.characterDefaultWeaponId,
            characterId = self.characterId,
            characterName = self.characterName,
            characterRes = self.characterRes,
            elementType = self.elementType,
            initLvMax = self.initLvMax,
            lvInit = self.lvInit,
            maxReinforcementEqualOrder = self.maxReinforcementEqualOrder,
            quality = self.quality,
            weaponType = self.weaponType
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return CharacterConfigData
function CharacterConfigData:read(data)

    local packet = self:new(
            data.CId,
            data.Level1Atk,
            data.Level1ChargingEfficiencyOfElements,
            data.Level1CriticalHitChance,
            data.Level1CriticalHitDamage,
            data.Level1Def,
            data.Level1ElementMastery,
            data.Level1HpValue,
            data.Level1NoWAtk,
            data.bagClickIcon,
            data.bagSideIcon,
            data.characterDefaultWeaponId,
            data.characterId,
            data.characterName,
            data.characterRes,
            data.elementType,
            data.initLvMax,
            data.lvInit,
            data.maxReinforcementEqualOrder,
            data.quality,
            data.weaponType)
    return packet
end

--- 这个角色ConfigIndex
---@return number 这个角色ConfigIndex
function CharacterConfigData:getCId()
    return self.CId
end
--- 1级的攻击属性
---@return number 1级的攻击属性
function CharacterConfigData:getLevel1Atk()
    return self.Level1Atk
end
--- 1级元素充能效率
---@return number 1级元素充能效率
function CharacterConfigData:getLevel1ChargingEfficiencyOfElements()
    return self.Level1ChargingEfficiencyOfElements
end
--- 1级的暴击率
---@return number 1级的暴击率
function CharacterConfigData:getLevel1CriticalHitChance()
    return self.Level1CriticalHitChance
end
--- 1级暴击伤害
---@return number 1级暴击伤害
function CharacterConfigData:getLevel1CriticalHitDamage()
    return self.Level1CriticalHitDamage
end
--- 1级防御力
---@return number 1级防御力
function CharacterConfigData:getLevel1Def()
    return self.Level1Def
end
--- 1级元素精通
---@return number 1级元素精通
function CharacterConfigData:getLevel1ElementMastery()
    return self.Level1ElementMastery
end
--- 1级的生命值
---@return number 1级的生命值
function CharacterConfigData:getLevel1HpValue()
    return self.Level1HpValue
end
--- 当前等级没有装备武器攻击属性
---@return number 当前等级没有装备武器攻击属性
function CharacterConfigData:getLevel1NoWAtk()
    return self.Level1NoWAtk
end
--- 头像
---@type  string 头像
function CharacterConfigData:getBagClickIcon()
    return self.bagClickIcon
end

--- 缩小头像
---@type  string 缩小头像
function CharacterConfigData:getBagSideIcon()
    return self.bagSideIcon
end

--- 角色默认武器id
---@return number 角色默认武器id
function CharacterConfigData:getCharacterDefaultWeaponId()
    return self.characterDefaultWeaponId
end
--- 角色id
---@return number 角色id
function CharacterConfigData:getCharacterId()
    return self.characterId
end
--- 角色名
---@type  string 角色名
function CharacterConfigData:getCharacterName()
    return self.characterName
end

--- 角色资源名
---@type  string 角色资源名
function CharacterConfigData:getCharacterRes()
    return self.characterRes
end

--- 角色元素伤害类型
---@return number 角色元素伤害类型
function CharacterConfigData:getElementType()
    return self.elementType
end
--- 角色创建出来初始最大等级
---@return number 角色创建出来初始最大等级
function CharacterConfigData:getInitLvMax()
    return self.initLvMax
end
--- 角色创建出来的初始等级
---@return number 角色创建出来的初始等级
function CharacterConfigData:getLvInit()
    return self.lvInit
end
--- 当前强化的星阶最大
---@return number 当前强化的星阶最大
function CharacterConfigData:getMaxReinforcementEqualOrder()
    return self.maxReinforcementEqualOrder
end
--- 品质
---@return number 品质
function CharacterConfigData:getQuality()
    return self.quality
end
--- 角色默认武器是什么类型
---@return number 角色默认武器是什么类型
function CharacterConfigData:getWeaponType()
    return self.weaponType
end


return CharacterConfigData
