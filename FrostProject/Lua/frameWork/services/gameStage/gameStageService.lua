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
local EStage = GlobalEnum.EStage  -- 游戏阶段Enum
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
    EventService:ListenEvent("LuaEventID.OnSatgePreLoadMap", self, self.OnStagePostLoadMap)
    EventService:ListenEvent("LuaEventID.OnStagePostLoadMap", self, self.OnStagePostLoadMap)
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
        FrostLogE(self.__classname, "跳过注册阶段", tGameStageType, inStageClass.__classname)
        return
    end
    FrostLogD(self.__classname, "Register stage with", tGameStageType, inStageClass.__classname)
    if self._mapStage[tGameStageType] then
        FrostLogE(self.__classname, "Already register stage with", tGameStageType, inStageClass.__classname)
        return
    end
    self._mapStage[tGameStageType] = tGameStageClass
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
        tPreStageType = tPreStageInstance.NowGameStageType
        tPreStageInstance:Leave()
    end
    -- 1. 先更新缓存，兼容新阶段的进入处理立即切换下一个阶段的情况
    self._curStageInstance = tNextStageInstance
    self._pendingStageType = EStage.None
    -- 2. 触发新阶段的回调，在回调中可能立即切换新的阶段
    tNextStageInstance:Enter(tPreStageType, inChangeContext)
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
    FrostLogD(self.__classname, "OnStagePostLoadMap",inWorld.SceneName,"in", tCurStageInstance.__classname, tCurStageInstance.StageType)
    tCurStageInstance:PostLoadMap(inWorld)
end

-------------------------------------------------------------------------------------------
-- 设置即将变更的阶段，下一帧进行实际切换
-- @param inStageType(EStage) 目标阶段类型
-- @param inChangeContext(*) 切换的上下文，会传递给新的阶段实例
-- {
--     mapID(number) 切换地图的目标地图ID（DT_Map)
--     playerPos(FVector) 可选，切换到地图之后的角色位置
--     playerRotation(FRotator) 可选，切换到地图之后的角色旋转
-- }
-- @param inIsImmediately(boolean) 是否立即切换
-------------------------------------------------------------------------------------------
function GameStageService:SetPendingStage(inStageType, inChangeContext, inIsImmediately)
    if inChangeContext and inIsImmediately ~= nil then
        FrostLogD(self.__classname, "SetPendingStage", inStageType, JSON.encode(inChangeContext), inIsImmediately)
    else
        FrostLogD(self.__classname, "SetPendingStage", inStageType, "nil", "nil")
    end
    if self._pendingStageType ~= EStage.None then
        FrostLogE(self.__classname, "Already has pending stage", self._pendingStageType)
        return
    end
    if self:GetCurStage() == inStageType then
        FrostLogW(self.__classname, "Already in stage", inStageType)
        return
    end
    if not self._mapStage[inStageType] then
        FrostLogE(self.__classname, "Stage has not yet registered", inStageType)
        return
    end
    self._pendingStageType = inStageType
    if inIsImmediately then
        self:_changeStage(inChangeContext)
    else
        ScheduleService:AddTimerOnNextFrame(self, self._changeStage, inChangeContext)
    end
end

return GameStageService
