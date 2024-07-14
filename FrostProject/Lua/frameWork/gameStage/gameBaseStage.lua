--[[--------------------------------------------------------------------------------
    author : zhangjingyi
    brief : 游戏阶段基类

--]]--------------------------------------------------------------------------------

---@class GameBaseStage
local GameBaseStage = Class("GameBaseStage")

function GameBaseStage:ctor()
    FrostLogD(self.__classname,"in GameBaseStage:ctor")

    -- 阶段类型 上一个
    self.PreGameStageType = GlobalEnum.EState.None
    -- 当前游戏阶段类型
    self.NowGameStageType = GlobalEnum.EState.None
end

function GameBaseStage:OnEnter(inPreGameStageType)
    
end

return GameBaseStage