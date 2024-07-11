--[[******************************************************************
    author : zhangjingyi

    purpose ： 事件服务

*******************************************************************--]]

---@class EventService:ServiceBase
local EventService = Class("EventService", ServiceBase)

function EventService:ctor()
    self._eventNameData = {}
    self._objData = {}

end

function EventService:OnEvent(eventName, handle, ...)
    FrostLogD(self.__classname,"OnEvent",eventName)
    if not eventName then
		FrostLogE(self.__classname, "EventServiceClass.OnEvent : invalid parameter")
        return
    end

    local self = EventService

    local data = self._eventNameData[eventName]
    if not data then
        return
    end

    for index = 1, #data, 1 do
        local d = data[index]
        if d.handle == handle then
            d.func(d.obj, ...)
        end
    end
end
function EventService:OnRemoveEvent(eventName, handle)
    FrostLogD(self.__classname, "OnRemoveEvent", eventName, handle)
    if not eventName or not handle then
		FrostLogE(self.__classname, "EventServiceClass.OnRemoveEvent : invalid parameter", eventName)
        return
    end

    local self = EventService

    local data = self._eventNameData[eventName]
    if not data then
        return
    end

    local handleData
    for index = 1, #data, 1 do
        if data[index].handle == handle then
            handleData = table.remove(data, index)
            break
        end
    end

    if not handleData then
        return
    end

    if #data == 0 then
        self._eventNameData[eventName] = nil
        data = {}
    end

    data = self._objData[handleData.obj]
    if data then
        for index = 1, #data, 1 do
            if data[index].handle == handle then
                table.remove(data, index)
                break
            end
        end

        if #data == 0 then
            self._objData[handleData.obj] = nil
        end
    end

   handleData = {}
end

--- 侦听事件
--- @param eventName string 事件ID
--- @param obj table 回调对象
--- @param func function 回调函数
--- @param time number 回调次数 -1：一直回调（默认值）
--- @param target any 只会收到target对象发送的event。默认nil：会收到任意对象发送的event。
--- @param source any 会收到指定发送给source对象或者未指定发送对象的event。默认nil：只会收到未指定发送对象的event。
function EventService:ListenEvent(eventName, obj, func, time, target, source)
    FrostLogD(self.__classname, "ListenEvent", eventName)
    if not eventName or not obj or not func then
        FrostLogE(self.__classname, "EventServiceClass:ListenEvent : invalid parameter", eventName)
        return
    end

    local data = self._eventNameData[eventName]
    if data then
        for index = 1, #data, 1 do
            local d = data[index]
            if d.obj == obj then
                FrostLogE(self.__classname, "EventServiceClass:ListenEvent : duplicate", eventName)
                return
            end
        end
    end

    local handle = event(eventName)

    local handleData = {}
    handle.obj = obj
    handle.func = func
    
end

function EventService:AddEventListener(event, func, inObj)
    if not inObj.evtListenerList then
        inObj.evtListenerList = {}
    end

    local handle = event:CreateListener(func, inObj)
    event:AddListener(handle)
    table.insert(inObj.evtListenerList, {evt = event, hanlde = handle})
end


return EventService