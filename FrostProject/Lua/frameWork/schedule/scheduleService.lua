--[[--------------------------------------------------------------------------------------
    author : zhangjingyi
     调度器c# 与 lua 侧同时写一份
--]]--------------------------------------------------------------------------------------

---@class ScheduleService:ServiceBase
local ScheduleService = Class("ScheduleService", ClassLibraryMap.ServiceBase)

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

    for i = #self._timer, 1, -1 do
        local tTimes = self._timer[i]

        if tTimes == null then
            table.remove(self._timer, i)
        elseif tTimes.frameNum then
            if curFrameCount > tTimes.frameNum then
                self._timer[i] = null
                xpcall(tTimes.func,__G__TRACKBACK__, tTimes.obj, table.unpack(tTimes.params))
            end
        else
            tTimes.remain = tTimes.remain - deltaTime

            if tTimes.remain <= 0 then
                xpcall(tTimes.func, __G__TRACKBACK__, tTimes.obj, table.unpack(tTimes.params))

                tTimes = self._timer[i]
                if tTimes ~= null then
                    if tTimes.repetition then
                        tTimes.remain = tTimes.remain + tTimes.interval
                    else
                        self._timer[i] = null
                    end
                end
            end
        end
    end
end

--- 添加更新
--- @param inObj Unit 接收更新回调对象
--- @param inFunc function  接收更新回调对象的函数
--- @param inIsOnce boolean 是否只Update一次, 如果为true,执行一次后自动RemoveUpdater
--- @param inDelayFrameNum number  延迟n帧执行 delayFrameNum > 0 有效
--- @param ... table 自定义参数，触发回调时按顺序传入
function ScheduleService:AddUpdater(inObj, inFunc, inIsOnce, inDelayFrameNum, ...)
    if not inObj or not inFunc then
        FrostLogD(self.__classname, "ScheduleServiceClass.AddUpdator : invald parameter")
        return
    end

    for i = 1, #self._updater do
        local data = self._updater[i]

        if data ~= null and data.obj == inObj and data.func == inFunc then
            local tObjName = inObj.__classname or inObj.__name
            FrostLogD(self.__classname, "ScheduleServiceClass.AddUpdater : duplicate add updater", tObjName)
            return
        end
    end

    local t = { }
    t.obj = inObj
    t.func = inFunc
    if inDelayFrameNum and inDelayFrameNum > 0 then
        t.frameNum = RootModules.FrameRate + inDelayFrameNum - 1
    else
        t.frameNum = false
    end
    t.isOnce = inIsOnce or false
    t.params = table.pack(...)

    self._updater[#self._updater + 1] = t
end

--- 移除更新
--- @param obj Unit 接收更新回调的对象
--- @param func function 接收更新回调的对象中的函数（为nil或者没有该参数标识移除所有obj关联的更新）
function ScheduleService:RemoveUpdater(obj, func)
    if not obj then
        FrostLogD(self.__classname, "ScheduleServiceClass.RemoveUpdater : invalid parameter")
        return
    end

    for i = 1, #self._updater do
        local data = self._updater[i]

        if data ~= null and data.obj == obj then
            if not func then
                self._updater[i] = null
            elseif data.func == func then
                self._updater[i] = null
                return
            end
        end
    end
end

-- 添加定时器
-- @param obj 接收定时回调的对象
-- @param func 接收回调的对象中的函数
-- @param interval 定时间隔时间，单位：秒
-- @param repetition 是否重复（默认不重复）
-- @param execFuncAtOnce 在重复的情况下（不重复此参数无效），是否立刻执行一次函数（否则将在interval时间后第一次执行函数）（默认否）
-- @param ... 可选，自定义参数，触发回调时按顺序传入
function ScheduleService:AddTimer(obj, func, interval, repetition, execFuncAtOnce, ...)
    if not obj or not func or not interval or interval < 0 then
        FrostLogE(self.__classname, "AddTimer : invalid parameter")
        return
    end
    self:_internalAddTimer(obj, func, interval, repetition, execFuncAtOnce, nil, ...)
end

-- 移除定时器
-- @param tbl 接收回调的对象
-- @param fun 接收回调的对象中的函数（为nil或者没有该参数删除跟该对象关联的所有定时器）
function ScheduleService:RemoveTimer(obj, func)
    if not obj then
        FrostLogE(self.__classname, "ScheduleServiceClass.RemoveTimer : invalid parameter")
        return
    end

    for i = 1, #self._timer do
        local data = self._timer[i]

        if data ~= null and data.obj == obj then
            if not func then
                self._timer[i] = null
            elseif data.func == func then
                self._timer[i] = null
                return
            end
        end
    end
end

function ScheduleService:DelayTime(obj, func, interval)
    self:RemoveTimer(obj, func)
    self:AddTimer(obj, func, interval)
end

---在下一渲染帧执行 保证不在当前帧
---@param obj Unit 接收定时回调的对象
---@param func function 接收回调的对象中的函数
---@param ... any 可选，自定义参数，触发回调时按顺序传入
function ScheduleService:AddTimerOnNextFrame(obj, func, ...)
    if not obj or not func then
        FrostLogE(self.__classname, "AddTimerOnNextFrame : invalid parameter")
        return
    end

    self:_internalAddTimer(obj, func, nil, nil, nil, CS.UnityEngine.Time.frameCount, ...)
end

function ScheduleService:_internalAddTimer(obj, func, interval, repetition, execFuncAtOnce, frameNum, ...)
    for i = 1, #self._timer do
        local data = self._timer[i]

        if data ~= null and data.obj == obj and data.func == func then
            local tObjName = obj.__classname or obj.__name
            FrostLogE(self.__classname, "_internalAddTimer : duplicate add timer", tObjName)
            return
        end
    end

    local t = { }
    t.obj = obj
    t.func = func
    t.interval = interval
    t.repetition = repetition or false
    t.frameNum = frameNum or false
    t.remain = interval
    t.params = table.pack(...)

    self._timer[#self._timer+1] = t

    if repetition and execFuncAtOnce then
        xpcall(func, __G__TRACKBACK__, obj, table.unpack(t.params))
    end
end


return ScheduleService