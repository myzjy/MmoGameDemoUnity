---@class LoginUIModelView
local LoginUIModelView = BaseClass(UIBaseModule.New())

local LoginView = nil
local LoginPanel = nil

function LoginUIModelView:InitUI()
    printDebug("LoginUIModelView Init")
    -- LoginUIModelView.base = UIBaseModule.New()
    --- Create the LoginUIModelView instance based on the

    LoginView = require("Game.UI.Login.LoginView").New()
    LoginPanel = require("Game.GenerateScripts.UIModules.LoginPanelView").New()
    LoginUIModelView.Init(
    ---配置类
            LoginConfig,
    --- UI 详细代码
            LoginView,
    ---UI组件绑定信息
            LoginPanel)
    LoginUIModelView.InstanceOrReuse()
end

--- UI 通知事件
function LoginUIModelView:Notification()
    ---不能直接返回，直接返回在内部拿不到表
    local data = {
        [LoginConfig.eventNotification.OPEN_LOGIN_INIT_PANEL] = LoginConfig.eventNotification.OPEN_LOGIN_INIT_PANEL,
        [LoginConfig.eventNotification.CLOSE_LOGIN_INIT_PANEL] = LoginConfig.eventNotification.CLOSE_LOGIN_INIT_PANEL,
        [LoginConfig.eventNotification.OpenLoginTapToStartUI] = LoginConfig.eventNotification.OpenLoginTapToStartUI,
        [LoginConfig.eventNotification.ShowLoginAccountUI] = LoginConfig.eventNotification.ShowLoginAccountUI,
    }
    return data
end

function LoginUIModelView:NotificationHandler(_eventNotification)
    local eventSwitch = {
        [LoginConfig.eventNotification.OPEN_LOGIN_INIT_PANEL] = function()
            if LoginUIModelView.isReuse then
                LoginUIModelView.InstanceOrReuse()
            else
                LoginUIModelView:InitUI()
            end
        end,
        [LoginConfig.eventNotification.CLOSE_LOGIN_INIT_PANEL] = function(obj)
            LoginView.OnHide()
        end,
        [LoginConfig.eventNotification.OpenLoginTapToStartUI] = function(obj)
            if Debug > 0 then
                printDebug("点击开始游戏之后，服务器在开启时间，可以正常进入")
            end
            LoginView:LoginStartGame()
        end,
        [LoginConfig.eventNotification.ShowLoginAccountUI] = function(obj)
            LoginView.OnHide()
        end
    }
    local switchAction = eventSwitch[_eventNotification.eventName]
    if eventSwitch then
        return switchAction(_eventNotification.eventBody)
    end
end
function LoginUIModelView:Build()
    
end
return LoginUIModelView
