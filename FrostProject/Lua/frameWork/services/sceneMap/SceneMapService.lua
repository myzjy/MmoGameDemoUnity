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
    self.MapID = 0
    self.MapPath = string.empty
end

function SceneMapService:vGetConfig()
    return
    {
        name = "SceneMapService",
    }
end
function SceneMapService:vInitialize()
    
end

function SceneMapService:GetMapID()
    return self.MapID
end

--- 加载场景
---@param location string 场景的定位地址
---@param sceneMode CS.UnityEngine.SceneManagement.LoadSceneMode 场景加载模式
---@param suspendLoad boolean 加载完毕时是否主动挂起
---@param priority number 优先级
---@param callBack fun(inParam: CS.YooAsset.SceneHandle) 加载回调
---@param gcCollect boolean 加载主场景是否回收垃圾
---@param progressCallBack fun(inProgress: number) 加载进度回调
---@return CS.YooAsset.SceneHandle
function SceneMapService:LoadScene(location, sceneMode, suspendLoad, priority, callBack, gcCollect, progressCallBack)
    self:OnStagePreLoadMap(location)
    return self._sceneModule:LoadScene(location, sceneMode, suspendLoad, priority, callBack, gcCollect, progressCallBack)
end

---@param location string
---@param callBack fun(inParam: CS.YooAsset.SceneHandle) 加载回调
---@param progressCallBack fun(inProgress:number)
---@return CS.YooAsset.SceneHandle
function SceneMapService:LoadSubScene(location, callBack, progressCallBack)
    self:OnStagePreLoadMap(location)
    return self._sceneModule:LoadSubScene(location, false, 1, callBack, true, progressCallBack)
end
-------------------------------------------------------------------------------------------
-- 地图加载前的回调
--- @param inMapPath(string) 地图的路径
-------------------------------------------------------------------------------------------
function SceneMapService:OnStagePreLoadMap(inMapPath)
    self.MapPath = inMapPath
    FrostLogD(self.__classname, "OnStagePreLoadMap", JSON.encode(inMapPath), self:GetMapID())
    EventService:SendEvent("LuaEventID.OnStagePreLoadMap", nil, nil, inMapPath, self:GetMapID())
end

-------------------------------------------------------------------------------------------
-- 地图加载后的回调
--- @param inWorld(CS.YooAsset.SceneHandle) 加载后的地图实例
-------------------------------------------------------------------------------------------
function SceneMapService:OnStagePostLoadMap(inWorld)
    FrostLogD(self.__classname, "OnStagePostLoadMap", JSON.encode(inWorld))
    --self:OnPostLoadMap(inWorld)
    EventService:SendEvent("LuaEventID.OnStagePostLoadMap", nil, nil, inWorld)
end

return SceneMapService

