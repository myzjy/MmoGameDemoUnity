---@class GameMainView:UIView
GameMainView = class("GameMainView", UIView)
GameMainConfig = {
    prefabName = "GameMainUIPanel",
    --- 当前会生成在那一层
    canvasType = UIConfigEnum.UICanvasType.UI,
    sortType = UIConfigEnum.UISortType.First,
    --- 当前 UI 交互事件 消息
    eventNotification = {
        --- 打开游戏主界面
        OPEN_GAMEMAINPANEL_UI = "OPEN_GAMEMAINPANEL_UI",
        --- 关闭 游戏主界面
        CLOSE_GAMEMAINPANEL_UI = "CLOSE_GAMEMAINPANEL_UI",
    }
}
require("application.app.ui.generateScripts.UIModules.GameMainUIPanelView")
function GameMainView:OnLoad()
    self.UIConfig = {
        Config = GameMainConfig,
        viewPanel = GameMainUIPanelView,
        initFunc = function()
            printDebug("GameMainView:Init()")
            GameMainView:OnInit()
        end,
        showFunc = function()
            printDebug("GameMainView:showUI()-->showFunc")
            GameMainView:OnShow()
        end,
        hideFunc = function() end,
    }
    self:Load(self.UIConfig)
    self:LoadUI(self.UIConfig)
    self:InstanceOrReuse()
end

function GameMainView:OnInit()
    printDebug("call GameMainView Lua Script function to OnInit ....")
end
