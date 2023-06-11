---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2023/5/16 0:11
---

--- UI Model View
local GameMainUIModelView = BaseClass(UIBaseModule).New()
--- UI 组件
local GameMainUIPanel = nil
--- 具体代码
local GameMainUIView = nil

--- 初始化 UI Model View
function GameMainUIModelView:InitUI()
    CS.Debug.Log("GameMainUIModelView Init")
    GameMainUIPanel = require("Game.GenerateScripts.UIModules.GameMainUIPanelView").New()
    GameMainUIView = require("Game.UI.GameMain.View.GameMainView").New()
    GameMainUIModelView.Init(---配置类
            GameMainConfig,
    --- UI 详细代码
            GameMainUIView,
    ---UI组件绑定信息
            GameMainUIPanel)
    GameMainUIModelView.InstanceOrReuse()

end
--- UI 通知事件
---@return table
function GameMainUIModelView:Notification()
    ---不能直接返回，直接返回在内部拿不到表
    local data = {
        [GameMainConfig.eventNotification.OPEN_GAMEMAIN_PANEL] = GameMainConfig.eventNotification.OPEN_GAMEMAIN_PANEL,
        [GameMainConfig.eventNotification.CLOSE_GAMEMAIN_PANEL] = GameMainConfig.eventNotification.CLOSE_GAMEMAIN_PANEL,
        [GameMainConfig.eventNotification.TIME_GAMEMAIN_PANEL] = GameMainConfig.eventNotification.TIME_GAMEMAIN_PANEL,
    }
    return data
end

function GameMainUIModelView:NotificationHandler(_eventNotification)
    local eventSwitch = {
        [GameMainConfig.eventNotification.OPEN_GAMEMAIN_PANEL] = function()
            if GameMainUIModelView.isReuse then
                GameMainUIModelView.InstanceOrReuse()
            else
                GameMainUIModelView:InitUI()
            end
        end,
        [GameMainConfig.eventNotification.CLOSE_GAMEMAIN_PANEL] = function(obj)
            GameMainUIView.OnHide()
        end,
        [GameMainConfig.eventNotification.TIME_GAMEMAIN_PANEL] = function(obj)
            if GameMainUIView == nil then
                return
            end
            local timeString = obj or string
            ---显示时间
            GameMainUIView:ShowNowTime(timeString)
        end
    }
    local switchAction = eventSwitch[_eventNotification.eventName]
    if eventSwitch then
        return switchAction(_eventNotification.eventBody)
    end

end
---返回 GameMainUIView UI具体脚本
---@return GameMainVIew
function GameMainUIModelView:GetGameMainUIView()
    return GameMainUIView
end
---返回 GameMainUIView UI 绑定相关组件
---@return GameMainUIPanelView
function GameMainUIModelView:GetGameMainUIPane()
    return GameMainUIPanel
end
function GameMainUIModelView:Build()
    GameMainViewController:GetInstance():OnInit(GameMainUIModelView)
end

return GameMainUIModelView