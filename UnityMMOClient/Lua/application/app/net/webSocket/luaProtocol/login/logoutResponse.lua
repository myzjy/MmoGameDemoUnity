---@class LogoutResponse
local LogoutResponse = class("LogoutResponse")
function LogoutResponse:ctor()
    --- sid
    ---@type number
    self.sid = 0
    --- 玩家id
    ---@type number
    self.uid = 0
end

---@param sid number sid
---@param uid number 玩家id
---@return LogoutResponse
function LogoutResponse:new(sid, uid)
    self.sid = sid --- long
    self.uid = uid --- long
    return self
end

---@return number
function LogoutResponse:protocolId()
    return 1003
end

---@return string
function LogoutResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            sid = self.sid,
            uid = self.uid
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return LogoutResponse
function LogoutResponse:read(data)

    local packet = self:new(
            data.sid,
            data.uid)
    return packet
end

--- sid
---@return number sid
function LogoutResponse:getSid()
    return self.sid
end
--- 玩家id
---@return number 玩家id
function LogoutResponse:getUid()
    return self.uid
end


return LogoutResponse
