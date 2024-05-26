---@class WeaponPlayerUserDataStruct
local WeaponPlayerUserDataStruct = class("WeaponPlayerUserDataStruct")
function WeaponPlayerUserDataStruct:ctor()
    --- 背包里面的武器icon
    ---@type string
    self.bagWeaponIcon = string.empty
    --- 武器获取到的第一时间
    ---@type string
    self.haveTimeAt = string.empty
    --- id
    ---@type number
    self.id = 0
    --- 当前武器强化到的等级
    ---@type number
    self.nowLv = 0
    --- 当前 等级 已经强化 到的经验
    ---@type number
    self.nowLvExp = 0
    --- 当前等级已知的可以强化的最大经验
    ---@type number
    self.nowLvMaxExp = 0
    --- 当前武器能强化到的最大等级
    ---@type number
    self.nowMaxLv = 0
    --- 当前武器所属技能
    ---@type number
    self.nowSkills = 0
    --- 武器Icon
    ---@type string
    self.weaponIcons = string.empty
    --- 武器 id
    ---@type number
    self.weaponId = 0
    --- 武器主词条的数值
    ---@type number
    self.weaponMainValue = 0
    --- 武器主词条的所属type
    ---@type number
    self.weaponMainValueType = 0
    --- 武器模型所属资源名
    ---@type string
    self.weaponModelNameIcons = string.empty
    --- 武器名字
    ---@type string
    self.weaponName = string.empty
    --- 武器 type 武器所属类型
    ---@type number
    self.weaponType = 0
end

---@param bagWeaponIcon string 背包里面的武器icon
---@param haveTimeAt string 武器获取到的第一时间
---@param id number id
---@param nowLv number 当前武器强化到的等级
---@param nowLvExp number 当前 等级 已经强化 到的经验
---@param nowLvMaxExp number 当前等级已知的可以强化的最大经验
---@param nowMaxLv number 当前武器能强化到的最大等级
---@param nowSkills number 当前武器所属技能
---@param weaponIcons string 武器Icon
---@param weaponId number 武器 id
---@param weaponMainValue number 武器主词条的数值
---@param weaponMainValueType number 武器主词条的所属type
---@param weaponModelNameIcons string 武器模型所属资源名
---@param weaponName string 武器名字
---@param weaponType number 武器 type 武器所属类型
---@return WeaponPlayerUserDataStruct
function WeaponPlayerUserDataStruct:new(bagWeaponIcon, haveTimeAt, id, nowLv, nowLvExp, nowLvMaxExp, nowMaxLv, nowSkills, weaponIcons, weaponId, weaponMainValue, weaponMainValueType, weaponModelNameIcons, weaponName, weaponType)
    self.bagWeaponIcon = bagWeaponIcon --- java.lang.String
    self.haveTimeAt = haveTimeAt --- java.lang.String
    self.id = id --- long
    self.nowLv = nowLv --- int
    self.nowLvExp = nowLvExp --- int
    self.nowLvMaxExp = nowLvMaxExp --- int
    self.nowMaxLv = nowMaxLv --- int
    self.nowSkills = nowSkills --- int
    self.weaponIcons = weaponIcons --- java.lang.String
    self.weaponId = weaponId --- int
    self.weaponMainValue = weaponMainValue --- int
    self.weaponMainValueType = weaponMainValueType --- int
    self.weaponModelNameIcons = weaponModelNameIcons --- java.lang.String
    self.weaponName = weaponName --- java.lang.String
    self.weaponType = weaponType --- int
    return self
end

---@return number
function WeaponPlayerUserDataStruct:protocolId()
    return 213
end

---@return string
function WeaponPlayerUserDataStruct:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            bagWeaponIcon = self.bagWeaponIcon,
            haveTimeAt = self.haveTimeAt,
            id = self.id,
            nowLv = self.nowLv,
            nowLvExp = self.nowLvExp,
            nowLvMaxExp = self.nowLvMaxExp,
            nowMaxLv = self.nowMaxLv,
            nowSkills = self.nowSkills,
            weaponIcons = self.weaponIcons,
            weaponId = self.weaponId,
            weaponMainValue = self.weaponMainValue,
            weaponMainValueType = self.weaponMainValueType,
            weaponModelNameIcons = self.weaponModelNameIcons,
            weaponName = self.weaponName,
            weaponType = self.weaponType
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return WeaponPlayerUserDataStruct
function WeaponPlayerUserDataStruct:read(data)

    local packet = self:new(
            data.bagWeaponIcon,
            data.haveTimeAt,
            data.id,
            data.nowLv,
            data.nowLvExp,
            data.nowLvMaxExp,
            data.nowMaxLv,
            data.nowSkills,
            data.weaponIcons,
            data.weaponId,
            data.weaponMainValue,
            data.weaponMainValueType,
            data.weaponModelNameIcons,
            data.weaponName,
            data.weaponType)
    return packet
end

--- 背包里面的武器icon
---@type  string 背包里面的武器icon
function WeaponPlayerUserDataStruct:getBagWeaponIcon()
    return self.bagWeaponIcon
end

--- 武器获取到的第一时间
---@type  string 武器获取到的第一时间
function WeaponPlayerUserDataStruct:getHaveTimeAt()
    return self.haveTimeAt
end

--- id
---@return number id
function WeaponPlayerUserDataStruct:getId()
    return self.id
end
--- 当前武器强化到的等级
---@return number 当前武器强化到的等级
function WeaponPlayerUserDataStruct:getNowLv()
    return self.nowLv
end
--- 当前 等级 已经强化 到的经验
---@return number 当前 等级 已经强化 到的经验
function WeaponPlayerUserDataStruct:getNowLvExp()
    return self.nowLvExp
end
--- 当前等级已知的可以强化的最大经验
---@return number 当前等级已知的可以强化的最大经验
function WeaponPlayerUserDataStruct:getNowLvMaxExp()
    return self.nowLvMaxExp
end
--- 当前武器能强化到的最大等级
---@return number 当前武器能强化到的最大等级
function WeaponPlayerUserDataStruct:getNowMaxLv()
    return self.nowMaxLv
end
--- 当前武器所属技能
---@return number 当前武器所属技能
function WeaponPlayerUserDataStruct:getNowSkills()
    return self.nowSkills
end
--- 武器Icon
---@type  string 武器Icon
function WeaponPlayerUserDataStruct:getWeaponIcons()
    return self.weaponIcons
end

--- 武器 id
---@return number 武器 id
function WeaponPlayerUserDataStruct:getWeaponId()
    return self.weaponId
end
--- 武器主词条的数值
---@return number 武器主词条的数值
function WeaponPlayerUserDataStruct:getWeaponMainValue()
    return self.weaponMainValue
end
--- 武器主词条的所属type
---@return number 武器主词条的所属type
function WeaponPlayerUserDataStruct:getWeaponMainValueType()
    return self.weaponMainValueType
end
--- 武器模型所属资源名
---@type  string 武器模型所属资源名
function WeaponPlayerUserDataStruct:getWeaponModelNameIcons()
    return self.weaponModelNameIcons
end

--- 武器名字
---@type  string 武器名字
function WeaponPlayerUserDataStruct:getWeaponName()
    return self.weaponName
end

--- 武器 type 武器所属类型
---@return number 武器 type 武器所属类型
function WeaponPlayerUserDataStruct:getWeaponType()
    return self.weaponType
end


return WeaponPlayerUserDataStruct
