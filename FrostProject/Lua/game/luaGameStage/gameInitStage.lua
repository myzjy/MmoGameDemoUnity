---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2024/7/23 下午10:57
---

---@class GameInitStage:GameBaseStage
local GameInitStage = Class("GameInitStage", ClassLibraryMap.GameBaseStage)

function GameInitStage:ctor()
    FrostLogD(self.__classname, ">>>> ctor")
    self.NowGameStageType = GlobalEnum.EStage.Init
end

-------------------------------------------------------------------------------------------
-- 进行必要的初始化
-------------------------------------------------------------------------------------------
function GameInitStage:OnStage_Enter()
    FrostLogD(self.__classname, ">>>> OnStage_Enter")
    ScheduleService:AddTimerOnNextFrame(self, self.GotoNextStage)
end

-------------------------------------------------------------------------------------------
-- 主要用识别网络、关卡、游戏模式等配置，切换到实际处理游戏的起始阶段
-------------------------------------------------------------------------------------------
function GameInitStage:GotoNextStage()
    local tNextStage = GlobalEnum.EStage.Login
    local tChangeContext = {}
    -- 正常登陆模式
    tNextStage = GlobalEnum.EStage.Login
    GameStageService:SetPendingStage(tNextStage, tChangeContext)
end

return GameInitStage