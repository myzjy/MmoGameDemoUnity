---@class ProcedureGameMain 主场景的状态机
local ProcedureGameMain = class("ProcedureGameMain")

function ProcedureGameMain:OnInit()
    
end
function ProcedureGameMain:GameMainEnter()
    if ServerConfigRequest == nil then
        PrintError("当前 ServerConfigRequest 脚本 没有读取到 请检查")
        return
    end
    if ServerConfigResponse == nil then
        PrintError("当前 ServerConfigResponse 脚本 没有读取到 请检查")
        return
    end
    local packetData = ServerConfigRequest()
    if packetData == nil then
        PrintError("当前 ServerConfigRequest lua 侧没有读取到 检查文件")
        return
    end
    local packet = packetData:new("1")
    ---@type string
    local jsonString = packet:write()
    NetManager:SendMessageEvent(jsonString)
end


return ProcedureGameMain
