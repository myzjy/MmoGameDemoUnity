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
        NGRLogE(self.__classname, "EventClass.IsEventHandle : invalid parameter")
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
        NGRLogE(self.__classname, "EventClass.OnEvent : invalid parameter")
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

return EventClass