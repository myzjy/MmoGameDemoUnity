---@class CreateWeaponDefaultAsk
local CreateWeaponDefaultAsk = class("CreateWeaponDefaultAsk")
function CreateWeaponDefaultAsk:ctor()
    ---@type number
    self.playerId = 0
    ---@type number
    self.uid = 0
    ---@type number
    self.userPlayerId = 0
    ---@type number
    self.weaponType = 0
end

---@param playerId number 
---@param uid number 
---@param userPlayerId number 
---@param weaponType number 
---@return CreateWeaponDefaultAsk
function CreateWeaponDefaultAsk:new(playerId, uid, userPlayerId, weaponType)
    self.playerId = playerId --- long
    self.uid = uid --- long
    self.userPlayerId = userPlayerId --- int
    self.weaponType = weaponType --- int
    return self
end

---@return number
function CreateWeaponDefaultAsk:protocolId()
    return 6000
end

---@return string
function CreateWeaponDefaultAsk:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            playerId = self.playerId,
            uid = self.uid,
            userPlayerId = self.userPlayerId,
            weaponType = self.weaponType
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return CreateWeaponDefaultAsk
function CreateWeaponDefaultAsk:read(data)

    local packet = self:new(
            data.playerId,
            data.uid,
            data.userPlayerId,
            data.weaponType)
    return packet
end

--- 
---@return number 
function CreateWeaponDefaultAsk:getPlayerId()
    return self.playerId
end
--- 
---@return number 
function CreateWeaponDefaultAsk:getUid()
    return self.uid
end
--- 
---@return number 
function CreateWeaponDefaultAsk:getUserPlayerId()
    return self.userPlayerId
end
--- 
---@return number 
function CreateWeaponDefaultAsk:getWeaponType()
    return self.weaponType
end


return CreateWeaponDefaultAsk
