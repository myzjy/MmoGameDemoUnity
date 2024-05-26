---@class UdpAttachment
local UdpAttachment = class("UdpAttachment")
function UdpAttachment:ctor()
    ---@type string
    self.host = string.empty
    ---@type number
    self.port = 0
end

---@param host string 
---@param port number 
---@return UdpAttachment
function UdpAttachment:new(host, port)
    self.host = host --- java.lang.String
    self.port = port --- int
    return self
end

---@return number
function UdpAttachment:protocolId()
    return 3
end

---@return string
function UdpAttachment:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            host = self.host,
            port = self.port
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return UdpAttachment
function UdpAttachment:read(data)

    local packet = self:new(
            data.host,
            data.port)
    return packet
end

--- 
---@type  string 
function UdpAttachment:getHost()
    return self.host
end

--- 
---@return number 
function UdpAttachment:getPort()
    return self.port
end


return UdpAttachment
