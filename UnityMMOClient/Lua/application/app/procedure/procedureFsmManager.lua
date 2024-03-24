---@class ProcedureFsmManager
local ProcedureFsmManager = {}
ProcedureGameMain = require("application.app.procedure.procedureGameMain")

function ProcedureFsmManager:OnInit()
    ProcedureGameMain:OnInit();
end

return ProcedureFsmManager
