---@class LoginCreateCharacterAsk
local LoginCreateCharacterAsk = class("LoginCreateCharacterAsk")
function LoginCreateCharacterAsk:ctor()
    ---@type number
    self.playerId = 0
    ---@type number
    self.uid = 0
    ---@type number
    self.weaponIndex = 0
end

---@param playerId number 
---@param uid number 
---@param weaponIndex number 
---@return LoginCreateCharacterAsk
function LoginCreateCharacterAsk:new(playerId, uid, weaponIndex)
    self.playerId = playerId --- int
    self.uid = uid --- long
    self.weaponIndex = weaponIndex --- int
    return self
end

---@return number
function LoginCreateCharacterAsk:protocolId()
    return 6002
end

---@return string
function LoginCreateCharacterAsk:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            playerId = self.playerId,
            uid = self.uid,
            weaponIndex = self.weaponIndex
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return LoginCreateCharacterAsk
function LoginCreateCharacterAsk:read(data)

    local packet = self:new(
            data.playerId,
            data.uid,
            data.weaponIndex)
    return packet
end

--- 
---@return number 
function LoginCreateCharacterAsk:getPlayerId()
    return self.playerId
end
--- 
---@return number 
function LoginCreateCharacterAsk:getUid()
    return self.uid
end
--- 
---@return number 
function LoginCreateCharacterAsk:getWeaponIndex()
    return self.weaponIndex
end


return LoginCreateCharacterAsk
