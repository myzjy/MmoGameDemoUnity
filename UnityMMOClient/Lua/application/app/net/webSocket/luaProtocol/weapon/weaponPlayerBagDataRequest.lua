---@class WeaponPlayerBagDataRequest
local WeaponPlayerBagDataRequest = class("WeaponPlayerBagDataRequest")
function WeaponPlayerBagDataRequest:ctor()
    --- 服务器 id？
    ---@type number
    self.serverId = 0
    --- 玩家id
    ---@type number
    self.uid = 0
end

---@param serverId number 服务器 id？
---@param uid number 玩家id
---@return WeaponPlayerBagDataRequest
function WeaponPlayerBagDataRequest:new(serverId, uid)
    self.serverId = serverId --- long
    self.uid = uid --- long
    return self
end

---@return number
function WeaponPlayerBagDataRequest:protocolId()
    return 1043
end

---@return string
function WeaponPlayerBagDataRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            serverId = self.serverId,
            uid = self.uid
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return WeaponPlayerBagDataRequest
function WeaponPlayerBagDataRequest:read(data)

    local packet = self:new(
            data.serverId,
            data.uid)
    return packet
end

--- 服务器 id？
---@return number 服务器 id？
function WeaponPlayerBagDataRequest:getServerId()
    return self.serverId
end
--- 玩家id
---@return number 玩家id
function WeaponPlayerBagDataRequest:getUid()
    return self.uid
end


return WeaponPlayerBagDataRequest
