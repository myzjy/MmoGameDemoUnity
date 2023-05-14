local LoginUIModelView = BaseClass(UIBaseModule)
local LoginUIView = require("Game.Login.LoginView")
local UINotifEnum = LoginConfig
function LoginUIModelView:Init()
    CS.Debug.Log("LoginUIModelView Init")
    -- LoginUIModelView.base = UIBaseModule.New()
    --- Create the LoginUIModelView instance based on the
    -- LoginUIView
    LoginUIModelView.base = UIBaseModule.Create("LoginPanel", UIConfigEnum.UISortType.Last, UIConfigEnum.UICanvasType.UI,
        LoginUIView)
    LoginUIModelView.base.InstanceOrReuse()

end

--- UI 通知事件
---@return table
function LoginUIModelView:Notification()
    local Notification = {
        LoginConfig.OpenLoginUI
    }
    return Notification;
end

function LoginUIModelView:NotificationHandler(_eventNotification)
    local eventSwitch = {
        [LoginUIModelView:Notification()[0]] = function()
            LoginUIModelView:Create()
            LoginUIModelView.base.InstanceOrReuse()
        end
    }
end

return LoginUIModelView
