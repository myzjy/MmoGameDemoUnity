---@class AuthUidAsk
local AuthUidAsk = class("AuthUidAsk")
function AuthUidAsk:ctor()
    ---@type string
    self.gatewayHostAndPort = string.empty
    ---@type number
    self.sid = 0
    ---@type number
    self.uid = 0
end

---@param gatewayHostAndPort string 
---@param sid number 
---@param uid number 
---@return AuthUidAsk
function AuthUidAsk:new(gatewayHostAndPort, sid, uid)
    self.gatewayHostAndPort = gatewayHostAndPort --- java.lang.String
    self.sid = sid --- long
    self.uid = uid --- long
    return self
end

---@return number
function AuthUidAsk:protocolId()
    return 22
end

---@return string
function AuthUidAsk:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            gatewayHostAndPort = self.gatewayHostAndPort,
            sid = self.sid,
            uid = self.uid
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return AuthUidAsk
function AuthUidAsk:read(data)

    local packet = self:new(
            data.gatewayHostAndPort,
            data.sid,
            data.uid)
    return packet
end

--- 
---@type  string 
function AuthUidAsk:getGatewayHostAndPort()
    return self.gatewayHostAndPort
end

--- 
---@return number 
function AuthUidAsk:getSid()
    return self.sid
end
--- 
---@return number 
function AuthUidAsk:getUid()
    return self.uid
end


return AuthUidAsk
