--[[--------------------------------------------
    Copyright 2024 - 2026 Tencent. All Rights Reserved
    Author : zhangjingyi
    brief :  网络收发协议
--]]
--------------------------------------------

--- @class NetMessageService : ServiceBase
local NetMessageService = Class("NetMessageService", ClassLibraryMap.ServiceBase)

function NetMessageService:ctor()
    ---@type EventClass
    self._GlobalEvents = ClassLibraryMap.EventClass()
    -- 是否 回调成功
    self._IsConnected = false
    self._LastConnectedUrl = ""
    self._NetMsgDataMap = {}
end

function NetMessageService:vGetConfig()
    return
    {
        name = "NetMessageService",
    }
end

function NetMessageService:vInitialize()
    -- EventService:ListenEvent("")
    -- open
    FrostLogD(self.__classname, "NetMessageService:vInitialize")

    CS.FrostEngine.GameEvent.AddEventListener(101, function()
        self._IsConnected = NetworkNativeService:IsConnect()
        if self._IsConnected then
            FrostLogD(self.__classname, "NetMessageService.Connect url = ", self._LastConnectedUrl)
            GameTimeService:OnConnected()
        else
            FrostLogE(self.__classname, "NetMessageService.Connect Failed!  url = ", self._LastConnectedUrl)
        end
    end)
end

local function OnNetDataRecieved(inMsgData)
    _G.NetMessageService:OnReceiveMessage(inMsgData)
end

function NetMessageService:Connect(url)
    self._LastConnectedUrl = url
    self._IsConnected = NetworkNativeService:Connect(url)
end

------------------------------------------------------------------
--- 向cs发送协议
---@param inMsgId number 协议id
---@param inMessage any
------------------------------------------------------------------
function NetMessageService:Send(inMsgId, inMessage)
    if inMsgId == 0 or inMsgId == nil then
        FrostLogE(self.__classname, "NetMessageService.Send , message is none")
        return false
    end

    local strMsg = JSON.encode({
        protocolId = inMsgId,
        packet = inMessage
    })

    if string.IsNullOrEmpty(strMsg) then
        FrostLogE(self.__classname, "NetMessageService.Send Fail MsgId = ", inMsgId, ", Message", inMessage)
        return false
    end

    self:SendData(inMsgId, strMsg, true)
    return true
end

function NetMessageService:SendData(inMsgId, inStrMsg, inMsgSize)
    local tLogTag = self.__classname
    FrostLogD(tLogTag, "With SendRawData", inMsgId)
    NetworkNativeService:SendMessage(inStrMsg)
    return true
end

----------------------------------------------------------------------------------------------------------------
--- 添加对指定网络消息的监听
--- @param inNetMessageID number MSG_NET_CODE) 网络消息id
--- @param inListener any LuaTable) object 网络协议监听者，收到inNetMessageID 后 执行 inListener的 inListenFunction
--- @param inListenFunction function inListener 的函数， 收到inNetMessageID 后 执行 inListener的 inListenFunction
----------------------------------------------------------------------------------------------------------------
function NetMessageService:ListenMessage(inNetMessageID, inListener, inListenFunction)
    if not inListener or not inListenFunction then
        FrostLogE(self.__classname, "Invalid listener parameter", inNetMessageID)
        return
    end
    local tEvent = self:_GetEventByObject()
    tEvent:AddEvent(inNetMessageID, inListener, inListenFunction)
end

----------------------------------------------------------------
--- 移除监听者对指定网络消息的监听
--- @param inListener any LuaTabel) 网络协议监听者
--- @param inNetMessageID number 网络消息id
function NetMessageService:RemoveListener(inListener, inNetMessageID)
    if not inListener then
        FrostLogE(self.__classname, "not Invalid to remove", inNetMessageID)
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

function NetMessageService:OnReceiveMessage(inMessage)
    self:ReceiveData(inMessage)
end

function NetMessageService:ReceiveData(inMessage)
    local tMsgInfo = JSON.decode(inMessage)
    if not tMsgInfo then
        FrostLogE(self.__classname, "NetMessageService.ReceiveData Decode Fail, MsgName = ", inMessage)
        return
    end
    local msgId = tMsgInfo.protocolId
    local tEvent = self:_GetEventByObject()
    tEvent:SendEvent(msgId, tMsgInfo.packet, nil)
end

_G.OnNetDataRecieved = OnNetDataRecieved

return NetMessageService