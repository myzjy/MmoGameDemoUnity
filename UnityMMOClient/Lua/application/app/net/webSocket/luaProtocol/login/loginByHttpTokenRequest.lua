---@class LoginByHttpTokenRequest
local LoginByHttpTokenRequest = class("LoginByHttpTokenRequest")
function LoginByHttpTokenRequest:ctor()
    --- 客户端版本号
    ---@type string
    self.appVersion = string.empty
    --- 配置版本号
    ---@type string
    self.confVersion = string.empty
    --- ip
    ---@type string
    self.ip = string.empty
    --- 0是默认登录，1为断线重连
    ---@type number
    self.reason = 0
    --- 资源版本号
    ---@type string
    self.resourceVersion = string.empty
    ---@type string
    self.token = string.empty
end

---@param appVersion string 客户端版本号
---@param confVersion string 配置版本号
---@param ip string ip
---@param reason number 0是默认登录，1为断线重连
---@param resourceVersion string 资源版本号
---@param token string 
---@return LoginByHttpTokenRequest
function LoginByHttpTokenRequest:new(appVersion, confVersion, ip, reason, resourceVersion, token)
    self.appVersion = appVersion --- java.lang.String
    self.confVersion = confVersion --- java.lang.String
    self.ip = ip --- java.lang.String
    self.reason = reason --- int
    self.resourceVersion = resourceVersion --- java.lang.String
    self.token = token --- java.lang.String
    return self
end

---@return number
function LoginByHttpTokenRequest:protocolId()
    return 1015
end

---@return string
function LoginByHttpTokenRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            appVersion = self.appVersion,
            confVersion = self.confVersion,
            ip = self.ip,
            reason = self.reason,
            resourceVersion = self.resourceVersion,
            token = self.token
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return LoginByHttpTokenRequest
function LoginByHttpTokenRequest:read(data)

    local packet = self:new(
            data.appVersion,
            data.confVersion,
            data.ip,
            data.reason,
            data.resourceVersion,
            data.token)
    return packet
end

--- 客户端版本号
---@type  string 客户端版本号
function LoginByHttpTokenRequest:getAppVersion()
    return self.appVersion
end

--- 配置版本号
---@type  string 配置版本号
function LoginByHttpTokenRequest:getConfVersion()
    return self.confVersion
end

--- ip
---@type  string ip
function LoginByHttpTokenRequest:getIp()
    return self.ip
end

--- 0是默认登录，1为断线重连
---@return number 0是默认登录，1为断线重连
function LoginByHttpTokenRequest:getReason()
    return self.reason
end
--- 资源版本号
---@type  string 资源版本号
function LoginByHttpTokenRequest:getResourceVersion()
    return self.resourceVersion
end

--- 
---@type  string 
function LoginByHttpTokenRequest:getToken()
    return self.token
end



return LoginByHttpTokenRequest
