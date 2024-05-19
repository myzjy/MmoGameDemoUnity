---@class GameMainUIPanelRequest gameMain UIPanel 打开发送协议
local GameMainUIPanelRequest = class("GameMainUIPanelRequest")

function GameMainUIPanelRequest:ctor()
    ---@type string 打开界面名
    self.panelPath = ""
    ---@type string 内嵌协议号 希望 能返回那些协议
    self.protocolStr = ""
end

function GameMainUIPanelRequest:protocolId()
    return 1051
end

function GameMainUIPanelRequest:write()
    local data = {
        protocolId = self:protocolId(),
        packet = { panelPath = self.panelPath, protocolStr = self.protocolStr }
    }
    local jsonStr = JSON.encode(data)
    return jsonStr
end

return GameMainUIPanelRequest
