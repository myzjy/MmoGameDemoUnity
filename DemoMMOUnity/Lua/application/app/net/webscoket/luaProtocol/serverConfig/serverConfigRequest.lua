---@class ServerConfigRequest
local ServerConfigRequest = {}

---@param panel string
function ServerConfigRequest:new(panel)
    local obj = {
        panel = panel
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end

function ServerConfigRequest:protocolId()
    return 1009
end

---@param buffer ByteBuffer
---@param packet any|nil
function ServerConfigRequest:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or ServerConfigRequest
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function ServerConfigRequest:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    local data = JSON.decode(jsonString)
    return ServerConfigRequest:new(data.packet.panel)
end

return ServerConfigRequest
