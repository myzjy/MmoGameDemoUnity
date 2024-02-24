---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/22 14:17
---
---@class LoginNetController
local LoginNetController = class("LoginNetController")
LoginConst = {
    Status = {},
    Event = {
        Login = 1001,
        Register = 1005,
        Pong = 104,
        ---登录拦截
        LoginTapTOStart = 1014,
        NetOpen = "LoginConst.Event.NetOpenEvent",
        StartLogin = "LoginConst.Event.StartLogin"
    }
}
---@type LoginNetController
local instance = nil

function LoginNetController.GetInstance()
    if not instance then
        instance = LoginNetController()
    end
    return instance
end

function LoginNetController:Init()
    LoginNetController:InitEvents()
end

function LoginNetController:InitEvents()
    printDebug("LoginNetController:InitEvents() line 24")
    GlobalEventSystem:Bind(
        PacketDispatcher.Event.OnConnect,
        function(url)
            LoginNetController:Connect(url)
        end
    )
    GlobalEventSystem:Bind(
        PacketDispatcher.Event.OnOpen,
        function()
            LoginNetController:OnNetOpenEvent()
        end
    )
    -- ProtocolManager:AddProtocolConfigEvent(LoginConst.Event.Login, function(data)
    -- 	LoginNetController:AtLoginResponse(data)
    -- end)

    UIUtils.AddEventListener(GameEvent.LoginResonse, self.AtLoginResponse, self)
    UIUtils.AddEventListener(GameEvent.RegisterResonse, self.AtRegisterResponse, self)
    -- UIUtils.AddEventListener(GameEvent.LoginSuccess, self.SendLoginResponse, self)
end

--- 网络打开
function LoginNetController:OnNetOpenEvent()
    UICommonViewController:GetInstance():OpenUIDataScenePanel(1, 1)
    UICommonViewController.LoadingRotate:OnClose()
    if Debug > 0 then
        printDebug("连接成功事件，登录服务器 登录过服务器 打开UI")
    end
    LoginUIController:GetInstance():Open()
end

---@param data LoginResponse
function LoginNetController:AtLoginResponse(data)
    local response = data
    local token = response.token
    local uid = response.uid
    local userName = response.userName
    if Debug > 0 then
        printDebug(
            "[user:" ..
            userName ..
            "][token:" ..
            token ..
            "]" ..
            "[uid:" ..
            uid ..
            "]" ..
            "[goldNum:" ..
            response.goldNum ..
            "],[premiumDiamondNum:" .. response.premiumDiamondNum
        )
    end
    PlayerUserCaCheData:SetUIDValue(uid)
    PlayerUserCaCheData:SetUseName(userName)
    PlayerUserCaCheData:SetDiamondNumValue(response.diamondNum)
    PlayerUserCaCheData:SetGoldNum(response.goldNum)
    PlayerUserCaCheData:SetPremiumDiamondNumValue(response.premiumDiamondNum)
    LoginUIController:GetInstance():OnHide()
    LoginUIController:GetInstance():OpenLoginTapToStartUI()
end

function LoginNetController:Connect(url)
    printDebug("LoginNetController:Connect(url)line 46" .. url)
end

function LoginNetController:AtPong(data)
    -- local timeNum = string.format("%.0f", (data.time / 1000))
    -- local time = os.date("%Y年%m月%d日 %H时%M分%S秒", tonumber(timeNum))
    --  printDebug("当前时间" .. time)
    -- GameMainViewController:GetInstance():ShowTime(time)
    GetSchedulerManager().serverTime = data.time
    ZJYFrameWork.UISerializable.Manager.DateTimeUtil.SetNow(data.time)
end

--- 是否可以登录
---@param data LoginTapToStartResponse
function LoginNetController:AtLoginTapToStartResponse(data)
    local response = data
    if response.message ~= nil then
        printDebug("可以登陆" .. JSON.encode(data.accessGame) .. "," .. response.message)
    end
    if response.accessGame then
    else
        --- 不能登錄
        UICommonViewController:GetInstance():OnOpenDialog(
            DialogConfig.ButtonType.YesNO,
            "",
            "当前不在登录时间..MESSAGE:" .. response.message,
            "确定",
            "取消",
            function(res)
            end
        )
        return
    end
    LoginUIController:GetInstance().LoginTapToStartView:LoginSatrtGame()
end

function LoginNetController:AtRegisterResponse(data)
    local response = data
    LoginUIController:GetInstance():Open()
end

---@param account string
---@param password string
function LoginNetController:SendLoginResponse(account, password)
    LoginCacheData:SetAccount(account)
    LoginCacheData:SetPassword(password)
    if LoginRequest == nil then
        printError("当前 LoginRequest 脚本 没有读取到 请检查")
        return
    end
    ---@type LoginRequest|nil
    local packetData = LoginRequest()
    if packetData == nil then
        printError("当前 LoginRequest 脚本 没有读取到 请检查")
        return
    end
    local packet = packetData:new(account, password)

    PacketDispatcher:AddProtocolConfigEvent(
        Error:protocolId(),
        ---@param data{ errorCode:number, errorMessage:string ,module:number}
        function(data)
            UIUtils.OnOpenDialog(
                I18nManager:GetString(data.errorMessage),
                function(res)
                end
            )
            -- 错误机制
        end
    )
    local jsonStr = packet:write()
    NetManager:SendMessageEvent(
        jsonStr,
        LoginResponse:protocolId(),
        function(data)
            printDebug("click start invke atLoginResponse")
            GameEvent.LoginResonse(data)
        end
    )
end

return LoginNetController
