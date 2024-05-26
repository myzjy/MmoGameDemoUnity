---@class CreateWeaponDefaultAnswer
local CreateWeaponDefaultAnswer = class("CreateWeaponDefaultAnswer")
function CreateWeaponDefaultAnswer:ctor()
    ---@type number
    self.weaponIndex = 0
end

---@param weaponIndex number 
---@return CreateWeaponDefaultAnswer
function CreateWeaponDefaultAnswer:new(weaponIndex)
    self.weaponIndex = weaponIndex --- int
    return self
end

---@return number
function CreateWeaponDefaultAnswer:protocolId()
    return 6001
end

---@return string
function CreateWeaponDefaultAnswer:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            weaponIndex = self.weaponIndex
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return CreateWeaponDefaultAnswer
function CreateWeaponDefaultAnswer:read(data)

    local packet = self:new(
            data.weaponIndex)
    return packet
end

--- 
---@return number 
function CreateWeaponDefaultAnswer:getWeaponIndex()
    return self.weaponIndex
end


return CreateWeaponDefaultAnswer
