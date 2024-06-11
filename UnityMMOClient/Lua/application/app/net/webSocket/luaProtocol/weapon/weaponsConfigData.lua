---@class WeaponsConfigData
local WeaponsConfigData = class("WeaponsConfigData")
function WeaponsConfigData:ctor()
    --- icon资源
    ---@type string
    self.iconResource = string.empty
    --- id
    ---@type number
    self.id = 0
    --- 当前武器等级最大值
    ---@type number
    self.maxLv = 0
    --- 武器升级到特定等21级会突破在之后会加数值
    ---@type string
    self.weaponBreakthrough = string.empty
    --- 武器强化1-20级每级强化数字
    ---@type string
    self.weaponInitProgress = string.empty
    --- 武器1级初始值
    ---@type string
    self.weaponInitValue = string.empty
    --- 武器名字
    ---@type string
    self.weaponName = string.empty
    --- 武器技能
    ---@type number
    self.weaponSkills = 0
    --- 武器类型
    ---@type number
    self.weaponType = 0
end

---@param iconResource string icon资源
---@param id number id
---@param maxLv number 当前武器等级最大值
---@param weaponBreakthrough string 武器升级到特定等21级会突破在之后会加数值
---@param weaponInitProgress string 武器强化1-20级每级强化数字
---@param weaponInitValue string 武器1级初始值
---@param weaponName string 武器名字
---@param weaponSkills number 武器技能
---@param weaponType number 武器类型
---@return WeaponsConfigData
function WeaponsConfigData:new(iconResource, id, maxLv, weaponBreakthrough, weaponInitProgress, weaponInitValue,
                               weaponName, weaponSkills, weaponType)
    self.iconResource = iconResource             --- java.lang.String
    self.id = id                                 --- int
    self.maxLv = maxLv                           --- int
    self.weaponBreakthrough = weaponBreakthrough --- java.lang.String
    self.weaponInitProgress = weaponInitProgress --- java.lang.String
    self.weaponInitValue = weaponInitValue       --- java.lang.String
    self.weaponName = weaponName                 --- java.lang.String
    self.weaponSkills = weaponSkills             --- int
    self.weaponType = weaponType                 --- int
    return self
end

---@return number
function WeaponsConfigData:protocolId()
    return 214
end

---@return string
function WeaponsConfigData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            iconResource = self.iconResource,
            id = self.id,
            maxLv = self.maxLv,
            weaponBreakthrough = self.weaponBreakthrough,
            weaponInitProgress = self.weaponInitProgress,
            weaponInitValue = self.weaponInitValue,
            weaponName = self.weaponName,
            weaponSkills = self.weaponSkills,
            weaponType = self.weaponType
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return WeaponsConfigData
function WeaponsConfigData:read(data)
    local packet = self:new(
        data.iconResource,
        data.id,
        data.maxLv,
        data.weaponBreakthrough,
        data.weaponInitProgress,
        data.weaponInitValue,
        data.weaponName,
        data.weaponSkills,
        data.weaponType)
    return packet
end

--- icon资源
---@type  string icon资源
function WeaponsConfigData:getIconResource()
    return self.iconResource
end

--- id
---@return number id
function WeaponsConfigData:getId()
    return self.id
end

--- 当前武器等级最大值
---@return number 当前武器等级最大值
function WeaponsConfigData:getMaxLv()
    return self.maxLv
end

--- 武器升级到特定等21级会突破在之后会加数值
---@type  string 武器升级到特定等21级会突破在之后会加数值
function WeaponsConfigData:getWeaponBreakthrough()
    return self.weaponBreakthrough
end

--- 武器强化1-20级每级强化数字
---@type  string 武器强化1-20级每级强化数字
function WeaponsConfigData:getWeaponInitProgress()
    return self.weaponInitProgress
end

--- 武器1级初始值
---@type  string 武器1级初始值
function WeaponsConfigData:getWeaponInitValue()
    return self.weaponInitValue
end

--- 武器名字
---@type  string 武器名字
function WeaponsConfigData:getWeaponName()
    return self.weaponName
end

--- 武器技能
---@return number 武器技能
function WeaponsConfigData:getWeaponSkills()
    return self.weaponSkills
end

--- 武器类型
---@return number 武器类型
function WeaponsConfigData:getWeaponType()
    return self.weaponType
end

return WeaponsConfigData
