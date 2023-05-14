local LoginUIModelView = BaseClass(UIBaseModule)
local UINotifEnum = LoginConfig
local LoginView = require("Game.Login.LoginView")
function LoginUIModelView:Init()
    CS.Debug.Log("LoginUIModelView Init")
    -- LoginUIModelView.base = UIBaseModule.New()
    --- Create the LoginUIModelView instance based on the
    self.LoginUIView = LoginView
    self.LoginPanel = require("Game.Login.LoginPanelView").New()
    LoginUIModelView.base =
        UIBaseModule.Create(
        "LoginPanel",
        UIConfigEnum.UISortType.Last,
        UIConfigEnum.UICanvasType.UI,
        self.LoginUIView,
        self.LoginPanel
    )
    LoginUIModelView.base.InstanceOrReuse()
end

--- UI 通知事件
---@return table
function LoginUIModelView:Notification()
    local Notification = {
        UINotifEnum.OpenLoginUI
    }
    return Notification
end

function LoginUIModelView:NotificationHandler(_eventNotification)
    local eventSwitch = {
        [LoginUIModelView:Notification()[0]] = function()
            -- LoginUIModelView:Create()
            LoginUIModelView.base.InstanceOrReuse()
        end
    }
end

return LoginUIModelView
