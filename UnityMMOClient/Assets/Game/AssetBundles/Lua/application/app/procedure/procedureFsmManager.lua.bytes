---@class ProcedureFsmManager
local ProcedureFsmManager = {}


function ProcedureFsmManager:OnInit()
    local eventInits = {
        "application.app.procedure.procedureGameMain"
    }
    for key, value in pairs(eventInits) do
        local v = require(value)
        v:OnInit()
    end
end

return ProcedureFsmManager
