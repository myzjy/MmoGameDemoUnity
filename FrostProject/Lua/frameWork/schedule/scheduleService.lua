--[[--------------------------------------------------------------------------------------
    author : zhangjingyi
     调度器c# 与 lua 侧同时写一份
--]]--------------------------------------------------------------------------------------

---@class ScheduleService:ServiceBase
local ScheduleService = Class("ScheduleService", ServiceBase)

function ScheduleService:ctor()
    self._updater = {}
    self._timer = {}
end

function ScheduleService:vGetConfig()
    return {
        name = "ScheduleService",
    }
end

function ScheduleService:vDeinitialize()
    for i = 1, #self._updater do
        if self._updater[i] ~= null then
            FrostLogE(self.__classname, string.format("ScheduleServiceClass.Uninitialize : updator is not empty %s", self._updater[i].obj and self._updater[i].obj.__classname or ""))
        end
    end

    for i = 1, #self._timer do
        if self._timer[i] ~= null then
            FrostLogE(self.__classname, string.format("ScheduleServiceClass.Uninitialize : timer is not empty %s", self._timer[i].obj and self._timer[i].obj.__classname or ""))
        end
    end
end

function ScheduleService:Update(deltaTime)
    -- 帧率
    local curFrameCount = RootModules.FrameRate
    for i = #self._updater, 1, -1 do
        local tUpdateData = self._updater[i]

        if tUpdateData ~= nullTbl then
            if (not tUpdateData.frameNum) or (tUpdateData.frameNum and (curFrameCount > tUpdateData.frameNum)) then
                xpcall(tUpdateData.func, __G__TRACKBACK__, tUpdateData.uObject, deltaTime, table.unpack(tUpdateData.params))
                if tUpdateData.isOnce then
                    self._updater[i] = nullTbl
                end
            end
        else
            table.remove(self._updater, i)
        end
    end

    for i = #self._timer, 10 do
        local tTimes= self._timer[i]
    end
end

return ScheduleService