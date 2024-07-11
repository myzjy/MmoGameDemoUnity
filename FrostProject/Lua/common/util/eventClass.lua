--[[-----------------------------------------------------------------------------------------

*      作者： zhangjingyi
*      时间： 2024/7/10
*      描述： 事件

--]]
-----------------------------------------------------------------------------------------


local EventClass = Class("EventClass")

function EventClass:ctor()
    self._csMaxEventId = 100

    self._argument = { {}, {}, {}, {}, {}, {}, {}, {}, {}, {} }
    self._useArgumentIndex = 1

    self._eventHandler = {}
    self._eventTable = {}
    self._eventRemoveId = {}

    self._eventHook = {}
end

function EventClass:Uninitialize()
    for k, _ in pairs(self._eventTable) do
        FrostLogE(self.__classname, "EventClass:Uninitialize : event is not empty - %s", k.__classname)
    end

    for i = 1, #self._eventHook, 3 do
        local tbl = self._eventHook[i + 1]
        if tbl ~= null then
            FrostLogE(self.__classname, "EventClass:Uninitialize : hook is not empty - %s", tbl.__classname)
        end
    end
end

function EventClass:IsEventHandle(id)
    if not id then
        FrostLogE(self.__classname, "EventClass.IsEventHandle : invalid parameter")
        return false
    end

    for i = 1, #self._eventHook, 3 do
        local d = self._eventHook[i]
        if d ~= null and d == id then
            return true
        end
    end

    local v = self._eventHandler[id]
    if v then
        for i = 1, #v, 2 do
            if v[i] ~= null then
                return true
            end
        end
    end

    return false
end

function EventClass:OnEvent(id, arg, inEventOwner)
    if not id then
        FrostLogE(self.__classname, "EventClass.OnEvent : invalid parameter")
        return false
    end

    for i = 1, #self._eventHook, 3 do
        local d = self._eventHook[i]
        if d ~= null and d == id then
            local t = self._eventHook[i + 1]
            local f = self._eventHook[i + 2]
            if f(t, id, arg) then
                return true
            end
        end
    end

    local v = self._eventHandler[id]
    if not v then
        return false
    end

    for i = #v - 1, 1, -2 do
        local t = v[i]
        local f = v[i + 1]
        if t and t ~= null and f and f ~= null then
            f(t, arg, inEventOwner)
        end
    end

    return false
end

function EventClass:Clear()
    local id = table.remove(self._eventRemoveId)
    while id do
        local v = self._eventHandler[id]
        if v then
            table.removeNullFromArray(v)
            if #v == 0 then
                self._eventHandler[id] = nil
            end
        end

        id = table.remove(self._eventRemoveId)
    end

    table.removeNullFromArray(self._eventHook)
end

function EventClass:RemoveEventHandler(tbl, id)
    local data = self._eventHandler[id]
    if not data then
        return
    end

    for i = 1, #data, 2 do
        if data[i] == tbl then
            data[i] = nullTbl
            data[i + 1] = nullTbl
            self._eventRemoveId[#self._eventRemoveId + 1] = id
            break
        end
    end
end

--- 添加事件接收器
--- @param id number 事件id
--- @param tbl any
--- @param func function
function EventClass:AddEvent(id, tbl, func)
    if not id or not tbl or not func then
        FrostLogE(self.__classname, "EventClass.AddEventHandler : invalid parameter", id, tbl, func)
        return
    end

    local v = self._eventHandler[id]
    if not v then
        v = {}
        self._eventHandler[id] = v
    end

    for i = 1, #v, 2 do
        if v[i] == tbl then
            FrostLogE(self.__classname, "EventClass.AddEventHandler : duplicate add event", id, tbl.__classname)
            return
        end
    end

    v[#v + 1] = tbl
    v[#v + 1] = func
    v = self._eventHandler[tbl]
    if not v then
        v = {}
        self._eventHandler[tbl] = v
    end

    v[#v + 1] = id
end

function EventClass:RemoveEvent(tbl, id)
    if not tbl then
        FrostLogE(self.__classname, "EventClass.RemoveEventHandler : invalid parameter")
        return
    end

    if id then
        self:RemoveEventHandler(tbl, id)

        local v = self._eventTable[tbl]
        if v then
            for i = 1, #v do
                if v[i] == id then
                    table.remove(v, i)
                    break
                end
            end

            if #v == 0 then
                self._eventTable[tbl] = null
            end
        end

        return
    end

    local data = self._eventTable[tbl]
    if not data then
        return
    end

    for i = 1, #data do
        self:RemoveEventHandler(tbl, data[i])
    end
    self._eventTable[tbl] = nil
end

function EventClass:AddEventHook(id, tbl, func)
    if not id or not tbl or not func then
        FrostLogE(self.__classname, "EventClass.AddEventHook : invalid parameter", id, tbl, func)
        return
    end
    for i = 1, #self._eventHook, 3 do
        if self._eventHook[i] ~= nullTbl and self._eventHook[i] == id and self._eventHook[i + 1] == tbl then
            FrostLogE(self.__classname, "EventClass.AddEventHook : duplicate add event hook")
        end
    end

    self._eventHook[#self._eventHook + 1] = id
    self._eventHook[#self._eventHook + 1] = tbl
    self._eventHook[#self._eventHook + 1] = func
end

function EventClass:RemoveEventHook(tbl, id)
    if not tbl then
        FrostLogE(self.__classname, "EventClass.RemoveEventHook : invalid parameter")
        return
    end
    for i = 1, #self._eventHook, 3 do
        if self._eventHook[i] ~= nullTbl and (not id or self._eventHook[i] == id) and self._eventHook[i + 1] == tbl then
            self._eventHook[i] = nullTbl
            self._eventHook[i + 1] = nullTbl
            self._eventHook[i + 2] = nullTbl
            if id then
                return
            end
        end
    end
end

-- 获取事件参数
-- @param name 参数名（为nil标识lua专用事件的参数）
-- @return 参数表
function EventClass:GetEventArg()
    local arg = self._argument[self._useArgumentIndex]
    table.Clear(arg)

    self._useArgumentIndex = self._useArgumentIndex + 1
    if self._useArgumentIndex > #self._argument then
        self._useArgumentIndex = 1
    end

    return arg
end


-- 发送事件
-- @param id 事件ID
-- @param arg 事件参数
-- @param inOwner(number) 事件拥有者，如网络协议对应的Entity
-- @return 无
function EventClass:SendEvent(id, arg, inEventOwner)
    if not id then
        FrostLogE(self.__classname, "EventClass.SendEvent : invalid parameter")
        return
    end
    if id > self._csMaxEventId then
        self:OnEvent(id, arg, inEventOwner)
    else
        FrostLogE(self.__classname, "EventClass.SendEvent : invalid eventId")
    end
end

return EventClass