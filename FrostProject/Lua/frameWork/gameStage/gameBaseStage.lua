--[[--------------------------------------------------------------------------------
    author : zhangjingyi
    brief : 游戏阶段基类

--]]--------------------------------------------------------------------------------

---@class GameBaseStage
local GameBaseStage = Class("GameBaseStage")

function GameBaseStage:ctor()
    FrostLogD(self.__classname,"in GameBaseStage:ctor")

    -- 阶段类型 上一个
    self.PreGameStageType = GlobalEnum.EStage.None
    -- 当前游戏阶段类型
    self.NowGameStageType = GlobalEnum.EStage.None
    -- 当前阶段绑定的地图ID，进入到当前阶段后自动加载此地图
    self._mapID = GlobalEnum.EInvalidDefine.StrEmpty
    -- 切换至当前状态的上下文
    -- {
    --     preLoadInfo(LuaTable) 可选，需要执行的异步操作
    --     {
    --         loadingSettings(LuaTable) 可选，加载界面的设置，为nil时不打开加载界面
    --         {
    --             loadingType(ELoadingType) 加载界面类型
    --             closeMode(EStagePreLoadCloseLoadingMode) 关闭加载界面的时机
    --         }
    --         listNecessaryHandlerGroups(LuaArray) 必要的异步处理器的组，直至加载完成才会真正的进入阶段逻辑，参数说明参考AsynchronousService:StartMultiGroups
    --         listHandlerGroups(LuaArray) 非必要的异步处理器的组，即时未完成也不阻碍进入阶段，参数说明参考AsynchronousService:StartMultiGroups
    --     }
    --     mapID(number) 切换地图的目标地图ID（DT_Map)
    --     playerPos(UVector) 可选，切换到地图之后的角色位置
    --     playerRotation(URotator) 可选，切换到地图之后的角色旋转
    -- }
    self._changeContext = {}
    -- 当前是否打开预载阶段的加载界面
    self._isShowPreLoadUI = false
end

--------------------------------------------------------------------------------------------------
-- 处理新阶段的进入，先执行阶段接入的回调，然后尝试该阶段可能存在地图
-- @param inPreGameStageType(EStage) 目标阶段类型
-- @param inChangeContext(*) 切换的上下文，会传递给新的阶段实例，字段信息参考 self._changeContext
--------------------------------------------------------------------------------------------------
function GameBaseStage:Enter(inPreGameStageType, inChangeContext)
    FrostLogD(self.__classname, "Enter " .. self.__classname, "PreStage is", inPreGameStageType, "with", JSON.encode(inChangeContext))
    -- 1. 由于Stage的实例是复用的，所以此处需要清理缓存
    table.Clear(self._changeContext)
    self.PreGameStageType = inPreGameStageType
    self._changeContext = inChangeContext or self._changeContext
    -- 2. 先进入阶段, 收集阶段实例要执行的预载处理和目标地图
    self:OnStage_Enter(self.PreGameStageType, self._changeContext)
    -- 3. 先预载，预载完成后再打开地图
    self:_startPreLoad()
end

-------------------------------------------------------------------------------------------
-- 地图开始加载前的回调
-- @param inMapPath(string) 地图路径
-------------------------------------------------------------------------------------------
function GameBaseStage:PreMapLoad(inMapPath)
    FrostLogD(self.__classname, "PreMapLoad", inMapPath)
    self:OnStage_PreMapLoad(inMapPath)
end

-------------------------------------------------------------------------------------------
-- 角色生成后，通常在spawn了APlayerController之后触发
-- @param inPlayerController(APlayerController) 角色控制器
-------------------------------------------------------------------------------------------
function GameBaseStage:PlayerPostLogin(inPlayerController, StartSpot)
    FrostLogD(self.__classname, "PlayerPostLogin", inPlayerController, StartSpot)
    self:OnStage_PlayerPostLogin(inPlayerController, StartSpot)
end

-------------------------------------------------------------------------------------------
-- 角色准备好后，通常依赖UWorld的BeginPlay，但是会在PostLoadMap之前
-- @param inPlayerController(APlayerController) 角色控制器
-------------------------------------------------------------------------------------------
function GameBaseStage:PlayerBeginPlay(inPlayerController)
    FrostLogD(self.__classname, "PlayerBeginPlay", inPlayerController)
    self:OnStage_PlayerBeginPlay(inPlayerController)
end

-------------------------------------------------------------------------------------------
-- 地图加载完成后的回调
-- @param inWorld(UWorld) 加载完的主关卡
-------------------------------------------------------------------------------------------
function GameBaseStage:PostLoadMap(inWorld)
    FrostLogD(self.__classname, "PostLoadMap " .. self.__classname, inWorld)
    -- 尝试关闭与加载阶段打开的加载界面
    self:_closePreLoadUI(GlobalEnum.EStagePreLoadCloseLoadingMode.PostLoadMap)
    self:OnStage_PostLoadMap(inWorld)
end

-------------------------------------------------------------------------------------------
-- 响应已就绪的角色进入地图
-- @param inPlayerCharacter(ANGRCCharacter) 进入地图的角色
-- @param inMapID(number) 地图ID
-- @param inMapType(EMapType) 地图类型
-------------------------------------------------------------------------------------------
function GameBaseStage:PlayerEnterWorld(inPlayerCharacter, inMapID, inMapType)
    FrostLogD(self.__classname, "PlayerEnterWorld " .. self.__classname, inMapID, inMapType)
    -- 尝试关闭与加载阶段打开的加载界面
    self:_closePreLoadUI(GlobalEnum.EStagePreLoadCloseLoadingMode.PlayerEnterWorld)
    self:OnStage_PlayerEnterWorld(inPlayerCharacter, inMapID, inMapType)
end

-------------------------------------------------------------------------------------------
-- 阶段退出的时机
-------------------------------------------------------------------------------------------
function GameBaseStage:Leave()
    FrostLogD(self.__classname, "Leave " .. self.__classname)
    self:OnStage_Leave()
end

function GameBaseStage:PlayerLogout(inPlayerController)
    FrostLogD(self.__classname, "PlayerLogout", inPlayerController)
    self:OnStage_PlayerLogout(inPlayerController)
end

function GameBaseStage:GetMapID()
    return self._changeContext.mapID or self._mapID
end

-----------------------------Only For Implement

-------------------------------------------------------------------------------------------
-- 子类覆盖，实现进入阶段后的自定义处理
-- @param inPreStage(EStage) 前一个阶段的类型
-- @param outStageContext(*) 切换的上下文，可以填充额外的数据，字段信息参考 self._changeContext
-------------------------------------------------------------------------------------------
function GameBaseStage:OnStage_Enter(inPreStageType, outStageContext)
end

-------------------------------------------------------------------------------------------
-- 子类覆盖，实现退出阶段后的自定义处理
-------------------------------------------------------------------------------------------
function GameBaseStage:OnStage_Leave()
end

-------------------------------------------------------------------------------------------
-- 启动预载，如果不存在阻塞的预载处理，则立即结束预载阶段
-------------------------------------------------------------------------------------------
function GameBaseStage:_startPreLoad()
    local tPreloadInfo = self._changeContext.preLoadInfo
    if not tPreloadInfo then
        self:_endPreLoad(true)
        return
    end
    -- 启动非阻塞的异步任务
    if tPreloadInfo.listHandlerGroups then
        AsynchronousService:StartMultiGroups(tPreloadInfo.listHandlerGroups)
    end
    -- 启动阻塞的异步任务
    if tPreloadInfo.listNecessaryHandlerGroups then
        -- 打开加载界面，直至预载阶段和地图加载完成后关闭
        local tLoadingSettings = tPreloadInfo.loadingSettings
        if tLoadingSettings then
            self:_showPreLoadUI(tLoadingSettings.loadingType)
        end
        local tAsyncSettigns =
        {
            isAutoStop = false,
            endCallback = function(inTaskGUID, inIsSuccessed)
                self:_endPreLoad(inIsSuccessed, inTaskGUID)
            end,
        }
        AsynchronousService:StartMultiGroups(tPreloadInfo.listNecessaryHandlerGroups, tAsyncSettigns)
        return
    end
    -- 无阻塞的异步任务，则立即结束预载阶段
    self:_endPreLoad(true)
end

-------------------------------------------------------------------------------------------
-- 预载阶段结束后，如果存在目标地图，则先加载地图，再关闭加载界面，否则立即关闭加载界面
-- @param inIsSuccessed(boolean) 预载是否成功
-- @param inTaskGUID(number) 预载的异步任务ID
-------------------------------------------------------------------------------------------
function GameBaseStage:_endPreLoad(inIsSuccessed, inTaskGUID)
    if not inIsSuccessed then
        FrostLogE(self.__classname, "Asynchronous task failed", inTaskGUID)
        return false
    end
    -- 关闭加载界面
    -- self:_closePreLoadUI(GlobalEnum.EStagePreLoadCloseLoadingMode.EndPreLoad)
    -- 如果存在需要加载的地图，则继续加载地图
    local tMapID = self:GetMapID()
    if UnityHelper.IsValidID(tMapID) then
        local function callBack(inParam)
            SceneMapService:OnStagePostLoadMap()
            -- self._changeContext.playerPos, self._changeContext.playerRotation
        end
        local function progressCallBack(inProgress)
            FrostLogD(self.__classname, "当前地图加载进度", inProgress)
        end
        SceneMapService:LoadSubScene(GConfig.MapNameIDList[tMapID], callBack, progressCallBack)
        return
    end
end

-------------------------------------------------------------------------------------------
-- 尝试关闭预加载的界面
-- @param inMode(EStagePreLoadCloseLoadingMode) 当前关闭的时机
-------------------------------------------------------------------------------------------
function GameBaseStage:_closePreLoadUI(inMode)
    if not self._isShowPreLoadUI then return end
    local tPreloadInfo = self._changeContext.preLoadInfo
    if not tPreloadInfo or not tPreloadInfo.loadingSettings then return end
    -- 关闭时机匹配时，执行关闭
    if tPreloadInfo.loadingSettings.closeMode == inMode then
        LoadingService:HideAll()
        self._isShowPreLoadUI = false
    end
end

return GameBaseStage
