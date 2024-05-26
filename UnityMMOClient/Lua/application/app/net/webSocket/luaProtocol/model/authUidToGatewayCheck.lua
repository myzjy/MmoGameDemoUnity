---@class AuthUidToGatewayCheck
local AuthUidToGatewayCheck = class("AuthUidToGatewayCheck")
function AuthUidToGatewayCheck:ctor()
    ---@type number
    self.uid = 0
end

---@param uid number 
---@return AuthUidToGatewayCheck
function AuthUidToGatewayCheck:new(uid)
    self.uid = uid --- long
    return self
end

---@return number
function AuthUidToGatewayCheck:protocolId()
    return 20
end

---@return string
function AuthUidToGatewayCheck:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            uid = self.uid
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return AuthUidToGatewayCheck
function AuthUidToGatewayCheck:read(data)

    local packet = self:new(
            data.uid)
    return packet
end

--- 
---@return number 
function AuthUidToGatewayCheck:getUid()
    return self.uid
end


return AuthUidToGatewayCheck
