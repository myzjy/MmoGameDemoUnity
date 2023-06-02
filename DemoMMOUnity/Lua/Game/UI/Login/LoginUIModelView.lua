---@class LoginUIModelView
local LoginUIModelView = BaseClass(UIBaseModule)

local LoginView = nil
local LoginPanel = nil
local isReuse = false

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
    }
    return data
end

function LoginUIModelView:NotificationHandler(_eventNotification)
    local eventSwitch = {
        [LoginConfig.eventNotification.OPEN_LOGIN_INIT_PANEL] = function()
            if isReuse then
                LoginUIModelView.InstanceOrReuse()
            else
                isReuse = true
                LoginUIModelView:InitUI()
            end
        end,
        [LoginConfig.eventNotification.CLOSE_LOGIN_INIT_PANEL] = function(obj)
            LoginView.OnHide()
        end
    }
    local switchAction = eventSwitch[_eventNotification.eventName]
    if eventSwitch then
        return switchAction(_eventNotification.eventBody)
    end

end

return LoginUIModelView
