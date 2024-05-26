---@class CharacterWeaponIDData
local CharacterWeaponIDData = class("CharacterWeaponIDData")
function CharacterWeaponIDData:ctor()
    --- 武器id --> 对应 WeaponUsePlayerDataEntity 中的 id
    ---@type number
    self.weaponFindId = 0
    --- 武器id
    ---@type number
    self.weaponId = 0
end

---@param weaponFindId number 武器id --> 对应 WeaponUsePlayerDataEntity 中的 id
---@param weaponId number 武器id
---@return CharacterWeaponIDData
function CharacterWeaponIDData:new(weaponFindId, weaponId)
    self.weaponFindId = weaponFindId --- long
    self.weaponId = weaponId --- int
    return self
end

---@return number
function CharacterWeaponIDData:protocolId()
    return 217
end

---@return string
function CharacterWeaponIDData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            weaponFindId = self.weaponFindId,
            weaponId = self.weaponId
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return CharacterWeaponIDData
function CharacterWeaponIDData:read(data)

    local packet = self:new(
            data.weaponFindId,
            data.weaponId)
    return packet
end

--- 武器id --> 对应 WeaponUsePlayerDataEntity 中的 id
---@return number 武器id --> 对应 WeaponUsePlayerDataEntity 中的 id
function CharacterWeaponIDData:getWeaponFindId()
    return self.weaponFindId
end
--- 武器id
---@return number 武器id
function CharacterWeaponIDData:getWeaponId()
    return self.weaponId
end


return CharacterWeaponIDData
