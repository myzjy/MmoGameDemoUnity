---@class LoginByHttpTokenResponse
local LoginByHttpTokenResponse = class("LoginByHttpTokenResponse")
function LoginByHttpTokenResponse:ctor()
    --- 信息
    ---@type string
    self.message = string.empty
end

---@param message string 信息
---@return LoginByHttpTokenResponse
function LoginByHttpTokenResponse:new(message)
    self.message = message --- java.lang.String
    return self
end

---@return number
function LoginByHttpTokenResponse:protocolId()
    return 1016
end

---@return string
function LoginByHttpTokenResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            message = self.message
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return LoginByHttpTokenResponse
function LoginByHttpTokenResponse:read(data)

    local packet = self:new(
            data.message)
    return packet
end

--- 信息
---@type  string 信息
function LoginByHttpTokenResponse:getMessage()
    return self.message
end



return LoginByHttpTokenResponse
