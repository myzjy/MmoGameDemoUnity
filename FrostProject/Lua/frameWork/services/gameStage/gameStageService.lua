--[[---------------------------------------------------------------------------------------
    Copyright 2023 - 2024 Tencent. All Rights Reserved
    Author : zhangjingyi
    brief : 管理游戏中的所有阶段
            1. 缓存所有已注册的阶段实例
            2. 处理阶段之间的切换
            3. 接收外部地图切换，角色状态等事件通知当前的Stage
--]]---------------------------------------------------------------------------------------

---@class GameStageService:ServiceBase
local GameStageService = Class("GameStageService", ClassLibraryMap.ServiceBase)
local EStage = GlobalEnum.EState  -- 游戏阶段Enum
function GameStageService:ctor()
    -- 当前的阶段实例
    self._curStageInstance = false
    -- 缓存即将切换的新阶段类型
    self._pendingStageType = EStage.None
    self._mapStage = {}
end

-------------------------------------------------------------------------------------------
-- 子类覆盖，进行返回类的静态配置数据
-------------------------------------------------------------------------------------------
function GameStageService:vGetConfig()
    return
    {
        name = "GameStageService",
    }
end
------------------------------------------------------------------------------
--- 初始化相关 监听事件、初始化Config
------------------------------------------------------------------------------
function GameStageService:vInitialize()
    --EventService:ListenEvent("LuaEventID.OnSatgePreLoadMap", self, self)
    self:_loadLuaGameStageConfig()
end

--- 读取 lua 侧的游戏状态配置器
function GameStageService:_loadLuaGameStageConfig()
    local tLuaGameStageConfigList = require("frameWork.services.gameStage.gameStageConfig")
    for tIndex, tGameFilePath in ipairs(tLuaGameStageConfigList) do
        FrostLogD(self.__classname, "加载游戏阶段配置类", tGameFilePath)
        -- 读取对应的 LuaGameFilePath 文件
        local tGameClass = require(tGameFilePath)
        if not tGameClass and LuaHelp.IsChildOfClass(tGameClass, "GameBaseStage") then
            FrostLogE(self.__classname, "获取游戏基阶段类失败", tGameFilePath)
        else
            self:_refreshRegisterGameStageConfig(tGameClass)
        end
    end
end

---------------------------------------------------------------------------------------
--- 注册新的阶段，缓存
---@param inStageClass GameBaseStage
---------------------------------------------------------------------------------------
function GameStageService:_refreshRegisterGameStageConfig(inStageClass)
    if not inStageClass then
        FrostLogE(self.__classname, "刷新注册游戏阶段失败，类无效")
        return
    end
    ---@type GameBaseStage
    local tGameStageClass = inStageClass()
    local tGameStageType = tGameStageClass.NowGameStageType
    if tGameStageType == GlobalEnum.EStage.None then
        FrostLogE(self.__classname, "跳过注册阶段", tStageType, inStageClass.__classname)
        return
    end
    FrostLogD(self.__classname, "Register stage with", tStageType, inStageClass.__classname)
    if self._mapStage[tStageType] then
        FrostLogE(self.__classname, "Already register stage with", tStageType, inStageClass.__classname)
        return
    end
    self._mapStage[tStageType] = tStageInstance
end

function GameStageService:vDeinitialize()
    EventService:UnListenEvent(self)
end

-------------------------------------------------------------------------------------------
-- 获得当前的阶段类型
-- @return(EStage) 阶段类型
-------------------------------------------------------------------------------------------
function GameStageService:GetCurStage()
    if self._curStageInstance then
        return self._curStageInstance.StageType
    end
    return EStage.None
end

-------------------------------------------------------------------------------------------
-- 变更当前的阶段
-- @param inChangeContext(*) 切换的上下文，会传递给新的阶段实例
-------------------------------------------------------------------------------------------
function GameStageService:_changeStage(inChangeContext)
    local tNextStageInstance = self._mapStage[self._pendingStageType]
    if not tNextStageInstance then
        FrostLogE(self.__classname, "Can't get stage instance by", self._pendingStageType)
        return
    end
    local tPreStageInstance = self._curStageInstance
    FrostLogD(self.__classname, "change stage from", tPreStageInstance and tPreStageInstance.__classname or "NoPreStage", "to", tNextStageInstance.__classname)
    local tPreStageType = EStage.None
    if tPreStageInstance then
        tPreStageType = tPreStageInstance.StageType
        tPreStageInstance:Leave()
    end
    -- 1. 先更新缓存，兼容新阶段的进入处理立即切换下一个阶段的情况
    self._curStageInstance = tNextStageInstance
    self._pendingStageType = EStage.None
    -- 2. 触发新阶段的回调，在回调中可能立即切换新的阶段
    tNextStageInstance:OnEnter(tPreStageType, inChangeContext)
end

-------------------------------------------------------------------------------------------
-- 地图加载前的回调
-- @param inMapPath(string) 地图的路径
-------------------------------------------------------------------------------------------
function GameStageService:OnStagePreLoadMap(inMapPath)
    local tCurStageInstance = self._curStageInstance
    if not tCurStageInstance then return end
    FrostLogD(self.__classname, "OnStagePreLoadMap", inMapPath, "in", tCurStageInstance.__classname, tCurStageInstance.StageType)
    tCurStageInstance:PreMapLoad(inMapPath)
    if self._pendingStageType ~= EStage.None then
        tCurStageInstance:Leave()
    end
end

-------------------------------------------------------------------------------------------
-- 地图加载后的回调
-- @param inWorld(UWorld) 加载后的地图实例
-------------------------------------------------------------------------------------------
function GameStageService:OnStagePostLoadMap(inWorld)
    local tCurStageInstance = self._curStageInstance
    if not tCurStageInstance then return end
    FrostLogD(self.__classname, "OnStagePostLoadMap", inWorld:GetName(), "in", tCurStageInstance.__classname, tCurStageInstance.StageType)
    tCurStageInstance:PostLoadMap(inWorld)
end

return GameStageService
