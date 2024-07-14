--[[--------------------------------------------
    Copyright 2024 - 2026 Tencent. All Rights Reserved
    Author : zhangjingyi
    brief :  网络收发协议
--]]
--------------------------------------------

---@class NetWorkConnectionService:ServiceBase
local NetWorkConnectionService = Class("NetWorkConnectionService", ServiceBase)

function NetWorkConnectionService:ctor()
    self._SendHeartBeatInterval = 2500   -- 单位：ms
    self._serverSettings = SettingNativeService.ServerSettings
    self._UpdateInterval = 0.5
end

-------------------------------------------------------------------------------------------
-- 子类覆盖，进行返回类的静态配置数据
-------------------------------------------------------------------------------------------
function NetWorkConnectionService:vGetConfig()
    return
    {
        name = "NetWorkConnectionService",
    }
end

function NetWorkConnectionService:Initialize()
end

function NetWorkConnectionService:TryToConnect()
    FrostLogD(self.__classname, "TryToConnect: ServerIP|Port=", self._serverSettings.ApiWebSocketUrl)
    local ret = NetMessageService:Connect(self._serverSettings.ApiWebSocketUrl)
    if not ret then
        FrostLogD(self.__classname, "TryToConnect failed, ret=", ret)
    end
end

return NetWorkConnectionService