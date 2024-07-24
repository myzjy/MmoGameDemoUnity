--[[---------------------------------------------------------------------------------------
    Author : zhangjingyi
    brief : 管理主关卡的地图加载和切换
            1. 管理地图相关的静态数据，如DT_Map内的信息
            2. 对外提供切换地图的接口，内部与CS对接切换地图的通用协议
            3. 响应引擎内的地图加载回调，通知外部当前地图加载状态
            4. 处理地图加载过程中的表现，如显示加载界面、等待角色拼装等
            5. 主要事件流程: 
                5.1 LuaEventID.OnStagePreLoadMap（地图加载完成）
                5.2 GMP.OnEnterWorld（玩家进入游戏）
                5.3 Map.PlayHasEntered（玩家组装完成，关闭加载界面）
--]]---------------------------------------------------------------------------------------

---@class SceneMapService:ServiceBase
local SceneMapService = Class("SceneMapService", ClassLibraryMap.ServiceBase)

--------------------------------------------------------------------------
--- 初始化 类
--------------------------------------------------------------------------
function SceneMapService:ctor()
    ---@type CS.FrostEngine.SceneModule
    self._sceneModule = GameModule.Scene
end

function SceneMapService:vGetConfig()
    return {
        name = "SceneMapService"
    }
end

function SceneMapService:vInitialize()
end

function SceneMapService:vDeinitialize()
    NetMessageService:RemoveListener(self)
    EventService:UnListenEvent(self)
end

---@param location string
---@param callBack function
---@param progressCallBack fun(progress:number)
function SceneMapService:LoadSubScene(location,callBack,progressCallBack)
    return  self._sceneModule:LoadSubScene(location,true,1, callBack,true,progressCallBack)
end

return SceneMapService
