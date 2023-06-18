LoginUIController = class("LoginUIController")

function LoginUIController:GetInstance()
    return LoginUIController
end

---打开界面
function LoginUIController:Open()
    if LoginView.reUse then
        --- 已经打开界面
        LoginView:InstanceOrReuse()
    else
        ---界面没打开 没生成过
        LoginView:OnLoad()
    end
end

function LoginUIController:OnClose()
    if LoginView.reUse then
        LoginView:OnHide()
    else
        if Debug > 0 then
            printError("LoginView 并未打开界面 生成")
        end
    end
end
