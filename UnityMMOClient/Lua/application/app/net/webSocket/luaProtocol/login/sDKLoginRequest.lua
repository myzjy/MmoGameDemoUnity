---@class SDKLoginRequest
local SDKLoginRequest = class("SDKLoginRequest")
function SDKLoginRequest:ctor()
    ---@type string
    self.account = string.empty
    ---@type string
    self.password = string.empty
end

---@param account string 
---@param password string 
---@return SDKLoginRequest
function SDKLoginRequest:new(account, password)
    self.account = account --- java.lang.String
    self.password = password --- java.lang.String
    return self
end

---@return number
function SDKLoginRequest:protocolId()
    return 5000
end

---@return string
function SDKLoginRequest:write()
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

---@return SDKLoginRequest
function SDKLoginRequest:read(data)

    local packet = self:new(
            data.account,
            data.password)
    return packet
end

--- 
---@type  string 
function SDKLoginRequest:getAccount()
    return self.account
end

--- 
---@type  string 
function SDKLoginRequest:getPassword()
    return self.password
end



return SDKLoginRequest
