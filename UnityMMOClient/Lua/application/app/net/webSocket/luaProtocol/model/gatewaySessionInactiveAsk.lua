---@class GatewaySessionInactiveAsk
local GatewaySessionInactiveAsk = class("GatewaySessionInactiveAsk")
function GatewaySessionInactiveAsk:ctor()
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
---@return GatewaySessionInactiveAsk
function GatewaySessionInactiveAsk:new(gatewayHostAndPort, sid, uid)
    self.gatewayHostAndPort = gatewayHostAndPort --- java.lang.String
    self.sid = sid --- long
    self.uid = uid --- long
    return self
end

---@return number
function GatewaySessionInactiveAsk:protocolId()
    return 23
end

---@return string
function GatewaySessionInactiveAsk:write()
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

---@return GatewaySessionInactiveAsk
function GatewaySessionInactiveAsk:read(data)

    local packet = self:new(
            data.gatewayHostAndPort,
            data.sid,
            data.uid)
    return packet
end

--- 
---@type  string 
function GatewaySessionInactiveAsk:getGatewayHostAndPort()
    return self.gatewayHostAndPort
end

--- 
---@return number 
function GatewaySessionInactiveAsk:getSid()
    return self.sid
end
--- 
---@return number 
function GatewaySessionInactiveAsk:getUid()
    return self.uid
end


return GatewaySessionInactiveAsk
