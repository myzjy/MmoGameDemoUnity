---@class RegisterRequest
local RegisterRequest = class("RegisterRequest")
function RegisterRequest:ctor()
    --- 账号
    ---@type string
    self.account = string.empty
    --- 确认密码
    ---@type string
    self.affirmPassword = string.empty
    --- 密码
    ---@type string
    self.password = string.empty
end

---@param account string 账号
---@param affirmPassword string 确认密码
---@param password string 密码
---@return RegisterRequest
function RegisterRequest:new(account, affirmPassword, password)
    self.account = account --- java.lang.String
    self.affirmPassword = affirmPassword --- java.lang.String
    self.password = password --- java.lang.String
    return self
end

---@return number
function RegisterRequest:protocolId()
    return 1005
end

---@return string
function RegisterRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            account = self.account,
            affirmPassword = self.affirmPassword,
            password = self.password
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return RegisterRequest
function RegisterRequest:read(data)

    local packet = self:new(
            data.account,
            data.affirmPassword,
            data.password)
    return packet
end

--- 账号
---@type  string 账号
function RegisterRequest:getAccount()
    return self.account
end

--- 确认密码
---@type  string 确认密码
function RegisterRequest:getAffirmPassword()
    return self.affirmPassword
end

--- 密码
---@type  string 密码
function RegisterRequest:getPassword()
    return self.password
end



return RegisterRequest
