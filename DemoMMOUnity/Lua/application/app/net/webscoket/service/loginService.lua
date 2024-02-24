--- 登录 界面 loginService
---

---@class LoginService
local LoginService = class("LoginService")

---@param account string
---@param password string
function LoginService:LoginByAccount(account, password)
    LoginCacheData:SetAccount(account)
    LoginCacheData:SetPassword(password)
    LoginNetController:SendLoginResponse(account,password)
end

function LoginService:LoginTapToStart()
    local platform = "editor"
    if UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android then
        platform = "android"
    elseif UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer then
        platform = "ios"
    elseif UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsPlayer then
        platform = "pc"
    else
        printDebug("Current platform is unknown.")
    end
    local packetData = LoginTapToStartRequest
    if packetData == nil then
        printError("当前LoginTapToStartRequest lua 侧没有读取到 检查文件")
        return
    end
    local packet = LoginTapToStartRequest:new(platform)
    local buffer = ByteBuffer:new()
    packetData:write(buffer, packet)
    NetManager:SendMessageEvent(
        buffer:readString(),
        LoginTapToStartResponse:protocolId(),
        function(packetData)
            LoginNetController:AtLoginTapToStartResponse(packetData)
        end
    )
end

---@param account string
---@param password string
---@param affirmPassword string
function LoginService:RegisterAccount(account, password, affirmPassword)
    local packetData = RegisterRequest()
    if packetData == nil then
        printError("当前 RegisterRequest lua 侧没有读取到 检查文件")
        return
    end
    local packet = packetData:new(account, password, affirmPassword)
    NetManager:SendMessageEvent(
        packetData:write(packet),
        packetData:protocolId(),
        function(packetData)
            LoginNetController:AtRegisterResponse(packetData)
    
        end
    )
end

return LoginService
