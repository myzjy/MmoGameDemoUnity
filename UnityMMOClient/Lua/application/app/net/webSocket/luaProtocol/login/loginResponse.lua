---@class LoginResponse
local LoginResponse = class("LoginResponse")
function LoginResponse:ctor()
    --- 玩家一些数据
    ---@type LoginUserServerInfoData
    self.loginUserServerInfoData = {}
    --- 玩家数据库加密token
    ---@type string
    self.token = string.empty
    --- 玩家id
    ---@type number
    self.uid = 0
end

---@param loginUserServerInfoData LoginUserServerInfoData 玩家一些数据
---@param token string 玩家数据库加密token
---@param uid number 玩家id
---@return LoginResponse
function LoginResponse:new(loginUserServerInfoData, token, uid)
    self.loginUserServerInfoData = loginUserServerInfoData --- com.gameServer.common.protocol.login.LoginUserServerInfoData
    self.token = token --- java.lang.String
    self.uid = uid --- long
    return self
end

---@return number
function LoginResponse:protocolId()
    return 1001
end

---@return string
function LoginResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            loginUserServerInfoData = self.loginUserServerInfoData,
            token = self.token,
            uid = self.uid
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return LoginResponse
function LoginResponse:read(data)
    local loginUserServerInfoDataPacket = LoginUserServerInfoData()
    local loginUserServerInfoData = loginUserServerInfoDataPacket:read(data.loginUserServerInfoData)

    local packet = self:new(
            loginUserServerInfoData,
            data.token,
            data.uid)
    return packet
end

--- 玩家一些数据
---@return LoginUserServerInfoData 玩家一些数据
function LoginResponse:getLoginUserServerInfoData()
    return self.loginUserServerInfoData
end
--- 玩家数据库加密token
---@type  string 玩家数据库加密token
function LoginResponse:getToken()
    return self.token
end

--- 玩家id
---@return number 玩家id
function LoginResponse:getUid()
    return self.uid
end


return LoginResponse
