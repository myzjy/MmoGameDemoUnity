---@class GatewaySynchronizeSidAsk
local GatewaySynchronizeSidAsk = class("GatewaySynchronizeSidAsk")
function GatewaySynchronizeSidAsk:ctor()
    ---@type string
    self.gatewayHostAndPort = string.empty
    ---@type Long
    self.sidMap = {}
end

---@param gatewayHostAndPort string 
---@param sidMap Long 
---@return GatewaySynchronizeSidAsk
function GatewaySynchronizeSidAsk:new(gatewayHostAndPort, sidMap)
    self.gatewayHostAndPort = gatewayHostAndPort --- java.lang.String
    self.sidMap = sidMap --- java.util.Map<java.lang.Long, java.lang.Long>
    return self
end

---@return number
function GatewaySynchronizeSidAsk:protocolId()
    return 24
end

---@return string
function GatewaySynchronizeSidAsk:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            gatewayHostAndPort = self.gatewayHostAndPort,
            buffer:writeLongLongMap(sidMap = self.sidMap)
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return GatewaySynchronizeSidAsk
function GatewaySynchronizeSidAsk:read(data)

    local packet = self:new(
            data.gatewayHostAndPort,
            local map1 = buffer:readLongLongMap())
    return packet
end

--- 
---@type  string 
function GatewaySynchronizeSidAsk:getGatewayHostAndPort()
    return self.gatewayHostAndPort
end

--- 
---@return number 
function GatewaySynchronizeSidAsk:getSidMap()
    return self.sidMap
end


return GatewaySynchronizeSidAsk
