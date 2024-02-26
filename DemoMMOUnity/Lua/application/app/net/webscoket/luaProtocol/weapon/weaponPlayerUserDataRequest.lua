---@class WeaponPlayerUserDataRequest
local WeaponPlayerUserDataRequest = class("WeaponPlayerUserDataRequest")
function WeaponPlayerUserDataRequest:ctor()
    ---@type number
    self.findUserId = 0   ---java.lang.long
    ---@type number
    self.findWeaponId = 0 ---java.lang.long
end

---@param findUserId number
---@param findWeaponId number
function WeaponPlayerUserDataRequest:new(findUserId, findWeaponId)
    self.findUserId = findUserId     ---java.lang.long
    self.findWeaponId = findWeaponId ---java.lang.long

    return self
end

function WeaponPlayerUserDataRequest:protocolId()
    return 1039
end
---@return string
function WeaponPlayerUserDataRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = { findUserId = self.findUserId, findWeaponId = self.findWeaponId }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

function WeaponPlayerUserDataRequest:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    local data = JSON.decode(jsonString)
    return WeaponPlayerUserDataRequest:new(data.packet.findUserId, data.packet.findWeaponId)
end

return WeaponPlayerUserDataRequest
