--- 登录 界面 loginService
---

---@class LoginService
local LoginService = class("LoginService")
local function LoginServiceConfig()
    return {
        LoginRequest = {
            protocolId = 1000,
            ---@type LoginRequest|nil
            packet = ProtocolManager.getProtocol(1000)
        }
    }
end
---@param account string
---@param password string
function LoginService:LoginByAccount(account, password)
    LoginChcheData:SetAccount(account)
    LoginChcheData:SetPassword(password)
    local packetData = LoginServiceConfig().LoginRequest.packet
    if packetData == nil then
        printError("当前 LoginReuqest 脚本 没有读取到 请检查")
        return
    end
    local pakcet = packetData:new(account, password)
    local buffer = ByteBuffer:new()
    ProtocolManager.write(buffer, pakcet)
    global.netManager:SendMessage(buffer.readByte())
end

function LoginService:LoginTapToStart()
    
end

return LoginService
