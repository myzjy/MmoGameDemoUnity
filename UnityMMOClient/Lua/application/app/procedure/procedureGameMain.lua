---@class ProcedureGameMain 主场景的状态机
local ProcedureGameMain = class("ProcedureGameMain")

function ProcedureGameMain:OnInit()
    self:AddEvent()
end

function ProcedureGameMain:AddEvent()
    GlobalEventSystem:Bind(ZJYFrameWork.Procedure.Scene.ProcedureGameMainConfig.GameMainEnter, function()
        if ServerConfigRequest == nil then
            printError("当前 ServerConfigRequest 脚本 没有读取到 请检查")
            return
        end
        if ServerConfigResponse == nil then
            printError("当前 ServerConfigResponse 脚本 没有读取到 请检查")
            return
        end
        local id = ServerConfigRequest:protocolId()
        local packetData = ProtocolManager.getProtocol(id)
        if packetData == nil then
            printError("当前 ServerConfigRequest lua 侧没有读取到 检查文件")
            return
        end
        local packet = packetData():new("1")
        local jsonString = packet:write()
        NetManager:SendMessageEvent(jsonString)
    end)
end

return ProcedureGameMain
