--[[******************************************************************
    author : zhangjingyi

    purpose ： 事件服务

*******************************************************************--]]

---@class EventService:ServiceBase
---@field __classname string
local EventService = Class("EventService", ClassLibraryMap.ServiceBase)

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

    local evenData = self._eventNameData[eventName]
    local data = false
    if evenData then
        data = evenData.data
        for index = 1, #data, 1 do
            local d = data[index]
            if d.obj == obj then
                FrostLogE(self.__classname, "EventServiceClass:ListenEvent : duplicate", eventName)
                return
            end
        end
    end
    if not evenData then
        evenData = {}
    end

    local handleData = {}
    if not data then
        data = {}
        local handle = event(eventName)
        handleData.obj = obj
        handleData.func = func
        handleData.handler = handle
        self:AddEventListener(handle, obj, func)
        data[#data + 1] = handleData
        evenData.event = handle
        self._eventNameData[eventName] = {event = handle, data = data}
    else
        handleData.obj = obj
        handleData.func = func
        handleData.handler = evenData.event
        self:AddEventListener(evenData.event, obj, func)

        data[#data + 1] = handleData
        self._eventNameData[eventName] = {event = evenData.event, data = data}
    end


   local objData = self._objData[obj]
    if not objData then
        objData = {}
        self._objData[obj] = objData
    else
        self._objData[obj] = {event = evenData.event, data = data}
    end

    return handle
end

function EventService:UnListenEvent(obj, eventName)
    FrostLogD(self.__classname, "UnListenEvent", eventName)
    if not obj then
        FrostLogE(self.__classname, "EventServiceClass:ListenEvent : invalid parameter", eventName)
        return
    end
    local handle = {}
    local evenData = false
    if eventName then
        evenData = self._eventNameData[eventName]
        local data = false
        if not evenData then
            evenData = {}
            data = false
            return
        end
        data = evenData.data
        for i = 1, #data, 1 do
            local tD = data[i]
            if tD.obj == obj then
                handle[#handle + 1] = tD.handle
            end
        end
    else
        evenData = self._objData[obj]
        local data = false
        if not evenData then
            evenData = {}
            data = false
            return
        end
        data = evenData.data
        for i = 1, #data, 1 do
            local tD = data[i]
            if tD.obj == obj then
                handle[#handle + 1] = tD.handle
            end
        end
    end
    for index = 1, #handle, 1 do
        evenData.event:RemoveListener(handle[index])
    end

    handle = {}
end

function EventService:SendEvent(id, sender, target, ...)
    FrostLogD(self.__classname, "SendEvent", id, ...)
    if not id then
        FrostLogE(self.__classname, "EventService:SendEvent : invalid parameter")
        return
    end
    local eventData = self._eventNameData[id]
    if not eventData  then
        FrostLogE(self.__classname, "EventService:SendEvent : not initialized", id)
        return
    end
    eventData.event(...)
end

function EventService:AddEventListener(event, inObj, func)
    if not inObj.evtListenerList then
        inObj.evtListenerList = {}
    end

    local handle = event:CreateListener(func, inObj)
    event:AddListener(handle)
    table.insert(inObj.evtListenerList, {evt = event, hanlde = handle})
end

---@type EventService
_G.EventService = EventService()

return EventService

