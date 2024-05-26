---@class GetPlayerInfoRequest
local GetPlayerInfoRequest = class("GetPlayerInfoRequest")
function GetPlayerInfoRequest:ctor()
    --- 返回加密的信息
    ---@type string
    self.token = string.empty
end

---@param token string 返回加密的信息
---@return GetPlayerInfoRequest
function GetPlayerInfoRequest:new(token)
    self.token = token --- java.lang.String
    return self
end

---@return number
function GetPlayerInfoRequest:protocolId()
    return 1004
end

---@return string
function GetPlayerInfoRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            token = self.token
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return GetPlayerInfoRequest
function GetPlayerInfoRequest:read(data)

    local packet = self:new(
            data.token)
    return packet
end

--- 返回加密的信息
---@type  string 返回加密的信息
function GetPlayerInfoRequest:getToken()
    return self.token
end



return GetPlayerInfoRequest
