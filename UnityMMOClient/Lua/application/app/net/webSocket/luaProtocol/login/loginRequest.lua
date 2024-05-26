---@class LoginRequest
local LoginRequest = class("LoginRequest")
function LoginRequest:ctor()
    --- 账号
    ---@type string
    self.account = string.empty
    --- 密码
    ---@type string
    self.password = string.empty
end

---@param account string 账号
---@param password string 密码
---@return LoginRequest
function LoginRequest:new(account, password)
    self.account = account --- java.lang.String
    self.password = password --- java.lang.String
    return self
end

---@return number
function LoginRequest:protocolId()
    return 1000
end

---@return string
function LoginRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            account = self.account,
            password = self.password
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return LoginRequest
function LoginRequest:read(data)

    local packet = self:new(
            data.account,
            data.password)
    return packet
end

--- 账号
---@type  string 账号
function LoginRequest:getAccount()
    return self.account
end

--- 密码
---@type  string 密码
function LoginRequest:getPassword()
    return self.password
end



return LoginRequest
