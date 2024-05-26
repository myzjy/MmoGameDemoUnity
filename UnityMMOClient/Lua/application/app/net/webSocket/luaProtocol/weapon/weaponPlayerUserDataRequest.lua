---@class WeaponPlayerUserDataRequest
local WeaponPlayerUserDataRequest = class("WeaponPlayerUserDataRequest")
function WeaponPlayerUserDataRequest:ctor()
    --- 需要查找的玩家id
    ---@type number
    self.findUserId = 0
    --- 需要成查找的某件装备 为0 代表 不需要查找特定装备
    ---@type number
    self.findWeaponId = 0
end

---@param findUserId number 需要查找的玩家id
---@param findWeaponId number 需要成查找的某件装备 为0 代表 不需要查找特定装备
---@return WeaponPlayerUserDataRequest
function WeaponPlayerUserDataRequest:new(findUserId, findWeaponId)
    self.findUserId = findUserId --- long
    self.findWeaponId = findWeaponId --- long
    return self
end

---@return number
function WeaponPlayerUserDataRequest:protocolId()
    return 1039
end

---@return string
function WeaponPlayerUserDataRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            findUserId = self.findUserId,
            findWeaponId = self.findWeaponId
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return WeaponPlayerUserDataRequest
function WeaponPlayerUserDataRequest:read(data)

    local packet = self:new(
            data.findUserId,
            data.findWeaponId)
    return packet
end

--- 需要查找的玩家id
---@return number 需要查找的玩家id
function WeaponPlayerUserDataRequest:getFindUserId()
    return self.findUserId
end
--- 需要成查找的某件装备 为0 代表 不需要查找特定装备
---@return number 需要成查找的某件装备 为0 代表 不需要查找特定装备
function WeaponPlayerUserDataRequest:getFindWeaponId()
    return self.findWeaponId
end


return WeaponPlayerUserDataRequest
