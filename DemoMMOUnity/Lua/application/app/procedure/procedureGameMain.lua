---@class ProcedureGameMain 主场景的状态机
local ProcedureGameMain = class("ProcedureGameMain")

function ProcedureGameMain:OnInit()

end

function ProcedureGameMain:AddEvent()
    GlobalEventSystem:Bind(ZJYFrameWork.Procedure.Scene.ProcedureGameMainConfig.GameMainEnter,function ()
        
    end)
end

return ProcedureGameMain
