---@class WeaponsConfigData
local WeaponsConfigData = class("WeaponsConfigData")

function WeaponsConfigData:ctor()
    self.id = -1;

    self.weaponNam = string.empty;

    self.weaponType = 1;

    self.weaponSkills = 1;

    self.weaponInitValue = string.empty;

    self.weaponInitProgress = string.empty;

    self.iconResource = string.empty;

    self.weaponBreakthrough = string.empty;

    self.maxLv = 1;
end

---@param id integer
---@param weaponName string
---@param weaponType integer
---@param weaponSkills integer
---@param weaponInitValue string
---@param weaponInitProgress string
---@param iconResource string
---@param weaponBreakthrough string
---@param maxLv integer
function WeaponsConfigData:new(id, weaponName, weaponType, weaponSkills, weaponInitValue, weaponInitProgress,
                               iconResource,
                               weaponBreakthrough, maxLv)
    self.id = id;
    self.weaponName = weaponName
    self.weaponType = weaponType
    self.weaponSkills = weaponSkills
    self.weaponInitValue = weaponInitValue
    self.iconResource = iconResource
    self.weaponInitProgress = weaponInitProgress
    self.weaponBreakthrough = weaponBreakthrough
    self.maxLv = maxLv
    return self;
end

return WeaponsConfigData
