---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/22 14:17
---
---@class LoginNetController
local LoginNetController = class("LoginNetController")
local json = require("Common.json")
LoginConst = {
    Status = {

    },
    Event = {
        Login = 1001,
        Pong = 104,
        ---登录拦截
        LoginTapTOStart = 1014,
        NetOpen = "LoginConst.Event.NetOpenEvent",
        StartLogin = "LoginConst.Event.StartLogin",
        LoginSucceed = "LoginConst.Event.LoginSucceed",
    }
}

function LoginNetController:Init()
    LoginNetController:InitEvents()
end

function LoginNetController:InitEvents()
    printDebug("LoginNetController:InitEvents() line 24")
    GlobalEventSystem:Bind(PacketDispatcher.Event.OnConnect, function(url)
        LoginNetController:Connect(url)
    end)
    GlobalEventSystem:Bind(LoginConst.Event.Login, function(data)
        LoginNetController:AtLoginResponse(data)
    end)
    GlobalEventSystem:Bind(LoginConst.Event.Pong, function(data)
        LoginNetController:AtPong(data)
    end)
    GlobalEventSystem:Bind(LoginConst.Event.LoginTapTOStart, function(data)
        LoginNetController:AtLoginTapToStartResponse(data)
    end)
end

--- 网络打开
function LoginNetController:OnNetOpenEvent()
    CommonController.Instance.snackbar.OpenUIDataScenePanel(1, 1);
    CommonController.Instance.loadingRotate.OnClose();
end

function LoginNetController:AtLoginResponse(data)
    local response = data
    local token = response.token
    local uid = response.uid;
    local userName = response.userName;
    if Debug > 0 then
        printDebug("[user:" ..
        userName ..
        "][token:" ..
        token ..
        "]" ..
        "[uid:" .. uid .. "]" .. "[goldNum:" .. response.goldNum .. "],[premiumDiamondNum:" .. response
        .premiumDiamondNum)
    end
    PlayerUserCaCheData:SetUIDValue(uid)
    PlayerUserCaCheData:SetUseName(userName)
    PlayerUserCaCheData:SetDiamondNumValue(response.diamondNum)
    PlayerUserCaCheData:SetGoldNum(response.goldNum)
    PlayerUserCaCheData:SetPremiumDiamondNumValue(response.premiumDiamondNum)
end

function LoginNetController:Connect(url)
    printDebug("LoginNetController:Connect(url)line 46" .. type(url))
end

function LoginNetController:AtPong(data)
    local timeNum = string.format("%.0f", (data.time / 1000));
    local time = os.date("%Y年%m月%d日 %H时%M分%S秒", tonumber(timeNum))
    printDebug("当前时间" .. time)
    GameMainViewController:GetInstance():ShowTime(time)
end

--- 是否可以登录
function LoginNetController:AtLoginTapToStartResponse(data)
    local response = data
    if response.message ~= nil then
        printDebug("可以登陆" .. json.encode(data.accessGame) .. "," .. response.message)
    end
    if response.accessGame then
    else
        --- 不能登錄
        CommonController.Instance.snackbar.OpenCommonUIPanel(DialogConfig.ButtonType.YesNo, "",
            "当前不在登录时间..MESSAGE:" .. response.message,
            function(res)

            end, "确定", "取消")
        return
    end
    DispatchEvent(LoginConfig.eventNotification.OpenLoginTapToStartUI)
end

return LoginNetController

