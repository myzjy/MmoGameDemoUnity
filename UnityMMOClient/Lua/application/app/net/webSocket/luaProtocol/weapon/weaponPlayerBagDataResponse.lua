---@class WeaponPlayerBagDataResponse
local WeaponPlayerBagDataResponse = class("WeaponPlayerBagDataResponse")
function WeaponPlayerBagDataResponse:ctor()
    --- 玩家id
    ---@type number
    self.uid = 0
end

---@param uid number 玩家id
---@return WeaponPlayerBagDataResponse
function WeaponPlayerBagDataResponse:new(uid)
    self.uid = uid --- long
    return self
end

---@return number
function WeaponPlayerBagDataResponse:protocolId()
    return 1044
end

---@return string
function WeaponPlayerBagDataResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            uid = self.uid
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return WeaponPlayerBagDataResponse
function WeaponPlayerBagDataResponse:read(data)

    local packet = self:new(
            data.uid)
    return packet
end

--- 玩家id
---@return number 玩家id
function WeaponPlayerBagDataResponse:getUid()
    return self.uid
end


return WeaponPlayerBagDataResponse
