---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2024/7/24 下午9:52
---

---@class LoginStage:GameBaseStage
local LoginStage = Class("LoginStage", ClassLibraryMap.GameBaseStage)
function LoginStage:ctor()
    FrostLogD(self.__classname, "ctor")
    self.StageType = GlobalEnum.EStage.Login
    self._mapID = GConfig.MapID.Login
    self.bOfflineMode = false
end

-------------------------------------------------------------------------------------------
-- 检查地图是否匹配、监听与服务器的网络连接，连接成功后立即自动登陆
-------------------------------------------------------------------------------------------
function LoginStage:OnStage_Enter()
    FrostLogD(self.__classname, "OnStage_Enter")
    EventService:ListenEvent("LuaEventID.NetConnectDone", self, self.OnNetConnectDone)
    EventService:ListenEvent("Login.EnterPlayMode", self, self.OnEnterPlayMode)

end

-------------------------------------------------------------------------------------------
-- 连接成功后请求登陆，等待回执
-------------------------------------------------------------------------------------------
function LoginStage:OnNetConnectDone()
    FrostLogD(self.__classname, "OnNetConnectDone")
    --NetworkConnectService:SendCSLoginReq()
end

function LoginStage:OnStage_Leave()
    FrostLogD(self.__classname, "OnStage_Leave")
    EventService:UnListenEvent(self)
    NetMessageService:RemoveListener(self)
end

-------------------------------------------------------------------------------------------
-- 确认登陆地图打开后，显示登陆界面和鼠标
-------------------------------------------------------------------------------------------
function LoginStage:OnStage_PostLoadMap()
    FrostLogD(self.__classname, "OnStage_PostLoadMap")
    StateService:ChangeGameState(GameState.LoginStateID, nil, nil, "LoginProcedure")
end

return LoginStage