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

return NetMessageService