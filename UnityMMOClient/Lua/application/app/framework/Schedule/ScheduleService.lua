---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2024/7/1 23:27
---

---@class ScheduleService:ServiceBase
local ScheduleService = class("ScheduleService",ServiceBase)

function ScheduleService:ctor()
    self.updater = {}
    self.timer = {}
end

-------------------------------------------------------------------------------------------
-- 子类覆盖，进行返回类的静态配置数据
-------------------------------------------------------------------------------------------
function ScheduleService:vGetConfig()
    return
    {
        name = "ScheduleService",
    }
end

function ScheduleService:vDeinitialize()

return ScheduleService
