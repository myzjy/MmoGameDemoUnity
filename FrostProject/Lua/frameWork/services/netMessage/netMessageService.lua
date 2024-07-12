--[[--------------------------------------------
    Copyright 2024 - 2026 Tencent. All Rights Reserved
    Author : zhangjingyi
    brief :  网络收发协议
--]]--------------------------------------------

--- @class NetMessageService : ServiceBase
local NetMessageService = Class("NetMessageService", ServiceBase)

function NetMessageService:ctor()
    self._GlobalEvents = EventClass()
end

function NetMessageService:vGetConfig()
    return
    {
        name = "NetMessageService",
    }
end

function NetMessageService:vInitialize()
    
end
function NetMessageService:Connect(url)
    local function ConnectHandler(inIsConnected)
        
    end
    NetworkNativeService:Connect(url)
end

----------------------------------------------------------------------------------------------------------------
--- 添加对指定网络消息的监听
--- @param inNetMessageID number (MSG_NET_CODE) 网络消息id
--- @param inListener any (LuaTable) object 网络协议监听者，收到inNetMessageID 后 执行 inListener的 inListenFunction
--- @param inListenFunction function inListener 的函数， 收到inNetMessageID 后 执行 inListener的 inListenFunction
----------------------------------------------------------------------------------------------------------------
function NetMessageService:ListenMessage(inNetMessageID,inListener,inListenFunction)
    if not inListener or not inListenFunction then
        FrostLogE(self.__classname,"Invalid listener parameter", inNetMessageID)
        return   
    end
    local tEvent = self:_GetEventByObject()
    tEvent:AddEvent(inNetMessageID,inListener,inListenFunction)
end

----------------------------------------------------------------
--- 移除监听者对指定网络消息的监听
--- @param inListener any (LuaTabel) 网络协议监听者
--- @param inNetMessageID number 网络消息id
function NetMessageService:RemoveListener(inListener, inNetMessageID)
    if not inListener then
        FrostLogE(self.__classname,"not Invalid to remove", inNetMessageID)
        return   
    end
    local tEvent = self:_GetEventByObject()
    tEvent:RemoveEvent(inListener, inNetMessageID)
end

----------------------------------------------------------------
--- 获取全局的事件管理器
--- @return EventClass
----------------------------------------------------------------
function NetMessageService:_GetEventByObject()
    return self._GlobalEvents
end

return NetMessageService