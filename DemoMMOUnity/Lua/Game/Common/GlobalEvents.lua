--- 全局事件
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/22 13:24

---@class GlobalEvent
GlobalEvent = {}
setmetatable(GlobalEvent, self)
--GlobalEventValues=BaseClass(GlobalEvent)
--function GlobalEvent:new()
--    local obj = {
--        GlobalEvent.all_event_dic,
--        GlobalEvent.calling_event_dic,
--        GlobalEvent.bind_id_to_event_id_dic
--    }
--    setmetatable(obj, self)
--    self.__index = self
--    return obj
--end

GlobalEvent.all_event_dic = {}
GlobalEvent.calling_event_dic = {}
GlobalEvent.bind_id_to_event_id_dic = {}
function GlobalEvent:Bind(event_id, event_func)
    if event_id == nil then
        print("Cat:GlobalEvent [Try to bind to a nil event_id] : ", debug.traceback())
        return
    end

    if event_func == nil then
        --故意报错输出调用堆栈
        print("Cat:GlobalEvent [Try to bind to a nil event_func] : ", debug.traceback())
        return
    end
    GlobalEvent.all_event_dic[event_id] = event_func
end
local getEvent = function(self, event_id)
    return GlobalEvent.all_event_dic[event_id]
end
function GlobalEvent:UnBindAll(is_delete)
    if is_delete then
        GlobalEvent.all_event_dic = nil
        GlobalEvent.bind_id_to_event_id_dic = nil
        GlobalEvent.calling_event_dic = nil
    else
        GlobalEvent.all_event_dic = {}
        GlobalEvent.bind_id_to_event_id_dic = {}
        GlobalEvent.calling_event_dic = {}
    end
end
GlobalEvent.value = {}
function GlobalEvent:Fire(event_id, table)
    if event_id == nil then
        print("Cat:EventSystem [Try to call GlobalEvent:Fire() with a nil event_id] : ", debug.traceback())
        return
    end
    --GlobalEvent.value = { table }
    --printDebug("GlobalEvent:Fire(event_id, table) line 58," .. type(table))

    GlobalEvent.all_event_dic[event_id](table)
    --local event_list = getEvent(self, event_id)
    --if event_list then
    --
    --end
end

return GlobalEvent