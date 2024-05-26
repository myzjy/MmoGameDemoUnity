---@class AuthUidToGatewayConfirm
local AuthUidToGatewayConfirm = class("AuthUidToGatewayConfirm")
function AuthUidToGatewayConfirm:ctor()
    ---@type number
    self.uid = 0
end

---@param uid number 
---@return AuthUidToGatewayConfirm
function AuthUidToGatewayConfirm:new(uid)
    self.uid = uid --- long
    return self
end

---@return number
function AuthUidToGatewayConfirm:protocolId()
    return 21
end

---@return string
function AuthUidToGatewayConfirm:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            uid = self.uid
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return AuthUidToGatewayConfirm
function AuthUidToGatewayConfirm:read(data)

    local packet = self:new(
            data.uid)
    return packet
end

--- 
---@return number 
function AuthUidToGatewayConfirm:getUid()
    return self.uid
end


return AuthUidToGatewayConfirm
