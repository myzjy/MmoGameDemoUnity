---@class LoginUIModelView
local LoginUIModelView = BaseClass(UIBaseModule)
---@class LoginConfig
local UINotifEnum = LoginConfig
---@class LoginView
local LoginView = require("Game.Login.LoginView")
LoginUIModelView.isResuse = false

---@class UIBaseModule
LoginUIModelView.base = nil
function LoginUIModelView:Init()
    printDebug("LoginUIModelView Init")
    -- LoginUIModelView.base = UIBaseModule.New()
    --- Create the LoginUIModelView instance based on the

    self.LoginUIView = LoginView
    self.LoginPanel = require("Game.GenerateScripts.UIModules.LoginPanelView").New()
    LoginUIModelView.base = UIBaseModule.Create(
            "LoginPanel",
            UIConfigEnum.UISortType.Last,
            UIConfigEnum.UICanvasType.UI,
            self.LoginUIView,
            self.LoginPanel
    )
    LoginUIModelView.base.InstanceOrReuse()
end

--- UI 通知事件
function LoginUIModelView:Notification()
    local UINotifEnum = {
        OpenLoginUI = "OPEN_LOGIN_UI"
    }

    return UINotifEnum
end

function LoginUIModelView:NotificationHandler(_eventNotification)
    local eventSwitch = {
        OpenLoginUI = function()
            -- LoginUIModelView:Create()
            if LoginUIModelView.isResuse then
                LoginUIModelView.base:InstisResuseanceOrReuse()
            else
                LoginUIModelView.isResuse = true
                LoginUIModelView:Init()
            end
        end
    }
    if _eventNotification.eventName == LoginUIModelView:Notification().OpenLoginUI then
        eventSwitch.OpenLoginUI()
    end
end

return LoginUIModelView
