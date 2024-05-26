---@class OpenGameMainUIPanelRequest
local OpenGameMainUIPanelRequest = class("OpenGameMainUIPanelRequest")
function OpenGameMainUIPanelRequest:ctor()
    --- 打开界面名
    ---@type string
    self.panelPath = string.empty
    --- 内嵌协议号希望能返回那些协议
    ---@type string
    self.protocolStr = string.empty
end

---@param panelPath string 打开界面名
---@param protocolStr string 内嵌协议号希望能返回那些协议
---@return OpenGameMainUIPanelRequest
function OpenGameMainUIPanelRequest:new(panelPath, protocolStr)
    self.panelPath = panelPath --- java.lang.String
    self.protocolStr = protocolStr --- java.lang.String
    return self
end

---@return number
function OpenGameMainUIPanelRequest:protocolId()
    return 1051
end

---@return string
function OpenGameMainUIPanelRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            panelPath = self.panelPath,
            protocolStr = self.protocolStr
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return OpenGameMainUIPanelRequest
function OpenGameMainUIPanelRequest:read(data)

    local packet = self:new(
            data.panelPath,
            data.protocolStr)
    return packet
end

--- 打开界面名
---@type  string 打开界面名
function OpenGameMainUIPanelRequest:getPanelPath()
    return self.panelPath
end

--- 内嵌协议号希望能返回那些协议
---@type  string 内嵌协议号希望能返回那些协议
function OpenGameMainUIPanelRequest:getProtocolStr()
    return self.protocolStr
end



return OpenGameMainUIPanelRequest
