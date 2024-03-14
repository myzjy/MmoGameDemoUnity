---@class ServerConfigRequest
local ServerConfigRequest = class("ServerConfigRequest")
function ServerConfigRequest:ctor()
    ---@type string
    self.panel = string.empty
end

---@param panel string
function ServerConfigRequest:new(panel)
    self.panel = panel
    return self
end

function ServerConfigRequest:protocolId()
    return 1009
end

function ServerConfigRequest:write()

    local message = {
        protocolId = self:protocolId(),
        packet =
        { panel = self.panel }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

function ServerConfigRequest:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    local data = JSON.decode(jsonString)
    return ServerConfigRequest:new(data.packet.panel)
end

return ServerConfigRequest
