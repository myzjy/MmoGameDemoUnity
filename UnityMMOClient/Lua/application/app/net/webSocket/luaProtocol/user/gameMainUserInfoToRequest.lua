---@class GameMainUserInfoToRequest
local GameMainUserInfoToRequest = class("GameMainUserInfoToRequest")
function GameMainUserInfoToRequest:ctor()
    --- 玩家id
    ---@type number
    self.uid = 0
end

---@param uid number 玩家id
---@return GameMainUserInfoToRequest
function GameMainUserInfoToRequest:new(uid)
    self.uid = uid --- long
    return self
end

---@return number
function GameMainUserInfoToRequest:protocolId()
    return 1031
end

---@return string
function GameMainUserInfoToRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            uid = self.uid
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return GameMainUserInfoToRequest
function GameMainUserInfoToRequest:read(data)

    local packet = self:new(
            data.uid)
    return packet
end

--- 玩家id
---@return number 玩家id
function GameMainUserInfoToRequest:getUid()
    return self.uid
end


return GameMainUserInfoToRequest
