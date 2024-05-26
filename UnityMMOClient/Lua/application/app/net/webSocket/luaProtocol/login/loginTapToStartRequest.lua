---@class LoginTapToStartRequest
local LoginTapToStartRequest = class("LoginTapToStartRequest")
function LoginTapToStartRequest:ctor()
    --- 客户端平台名字
    ---@type string
    self.clientName = string.empty
end

---@param clientName string 客户端平台名字
---@return LoginTapToStartRequest
function LoginTapToStartRequest:new(clientName)
    self.clientName = clientName --- java.lang.String
    return self
end

---@return number
function LoginTapToStartRequest:protocolId()
    return 1013
end

---@return string
function LoginTapToStartRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            clientName = self.clientName
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return LoginTapToStartRequest
function LoginTapToStartRequest:read(data)

    local packet = self:new(
            data.clientName)
    return packet
end

--- 客户端平台名字
---@type  string 客户端平台名字
function LoginTapToStartRequest:getClientName()
    return self.clientName
end



return LoginTapToStartRequest
