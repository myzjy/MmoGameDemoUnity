---@class RegisterResponse
local RegisterResponse = class("RegisterResponse")
function RegisterResponse:ctor()
    --- 注册时，没有注册成功时，错误消息
    ---@type string
    self.error = string.empty
    --- 是否注册成功
    ---@type boolean
    self.mRegister = 0
end

---@param error string 注册时，没有注册成功时，错误消息
---@param boolean boolean 是否注册成功
---@return RegisterResponse
function RegisterResponse:new(error, mRegister)
    self.error = error --- java.lang.String
    self.mRegister = mRegister --- boolean
    return self
end

---@return number
function RegisterResponse:protocolId()
    return 1006
end

---@return string
function RegisterResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            error = self.error,
            mRegister = self.mRegister
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return RegisterResponse
function RegisterResponse:read(data)

    local packet = self:new(
            data.error,
            data.mRegister)
    return packet
end

--- 注册时，没有注册成功时，错误消息
---@type  string 注册时，没有注册成功时，错误消息
function RegisterResponse:getError()
    return self.error
end

--- 是否注册成功
---@return boolean 是否注册成功
function RegisterResponse:getMRegister()
    return self.mRegister
end


return RegisterResponse
