--[[---------------------------------------------------------------------------------------
    Copyright 2023 - 2024 Tencent. All Rights Reserved
    Author : zhangjingyi
    brief : 管理游戏中的所有阶段
            1. 缓存所有已注册的阶段实例
            2. 处理阶段之间的切换
            3. 接收外部地图切换，角色状态等事件通知当前的Stage
--]]---------------------------------------------------------------------------------------

---@class GameStageService:ServiceBase
local GameStageService = Class("GameStageService", _G.ServiceBase)
local EStage = GlobalEnum.EState  -- 游戏阶段Enum
local LuaGameStageConfigList = require("frameWork.services.gameStage.gameStageConfig")
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
    -- EventService:ListenEvent()

end

--- 读取 lua 侧的游戏状态配置器
function GameStageService:_loadLuaGameStageConfig()
    for tIndex, tGameFilePath in ipairs(LuaGameStageConfigList) do
        FrostLogD(self.__classname, "Load game stage config class with", tGameFilePath)
        -- 读取对应的 LuaGameFilePath 文件
        local tGameClass = require(tGameFilePath)
        if not tGameClass and LuaHelp.IsChildOfClass(tGameClass, "GameBaseStage") then
            FrostLogE(self.__classname, "Get game base stage class failed with", tGameFilePath)
        else
            self:_refreshRegisterGameStageConfig(tGameClass)
        end
    end
end

---------------------------------------------------------------------------------------
--- 注册新的阶段，缓存
---@param inStageClass
---------------------------------------------------------------------------------------
function GameStageService:_refreshRegisterGameStageConfig(inStageClass)
    if not inStageClass then
        FrostLogE(self.__classname,"Refresh Register Game Stage failed with invalid class")
        return
    end
    local tGameStageClass = inStageClass()

end

function GameStageService:vDeinitialize()
end

return GameStageService