---@class ServerConfigRequest
local ServerConfigRequest = class("ServerConfigRequest")
function ServerConfigRequest:ctor()
    --- 打开面板
    ---@type string
    self.panel = string.empty
end

---@param panel string 打开面板
---@return ServerConfigRequest
function ServerConfigRequest:new(panel)
    self.panel = panel --- java.lang.String
    return self
end

---@return number
function ServerConfigRequest:protocolId()
    return 1009
end

---@return string
function ServerConfigRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            panel = self.panel
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return ServerConfigRequest
function ServerConfigRequest:read(data)

    local packet = self:new(
            data.panel)
    return packet
end

--- 打开面板
---@type  string 打开面板
function ServerConfigRequest:getPanel()
    return self.panel
end



return ServerConfigRequest
