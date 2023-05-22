---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/22 14:17
---

LoginNetController = {}
LoginConst = {
    Status = {

    },
    Event = {
        Login = 1001,
        NetOpen = "LoginConst.Event.NetOpenEvent",
        StartLogin = "LoginConst.Event.StartLogin",
        LoginSucceed = "LoginConst.Event.LoginSucceed",
    }
}

function LoginNetController:Init()
    LoginController.InitEvents()
end
function LoginController:InitEvents()
    GlobalEventSystem:Bind(LoginConst.Event.Login, LoginController.AtLoginResponse, self)
end
--- 网络打开
function LoginController:OnNetOpenEvent()
    CommonController.Instance.snackbar.OpenUIDataScenePanel(1, 1);
    CommonController.Instance.loadingRotate.OnClose();

end

---@param response LoginResponse
function LoginNetController:AtLoginResponse(response)
    local token = response.token
    local uid = response.uid;
    local userName = response.userName;
    if Debug > 1 then
        printDebug("[user:" .. userName "][token:" .. token .. "]" .. "[uid:" .. uid .. "]")
    end

end

return LoginNetController