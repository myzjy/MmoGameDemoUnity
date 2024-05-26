---@class Error
local Error = class("Error")
function Error:ctor()
    ---@type number
    self.errorCode = 0
    ---@type string
    self.errorMessage = string.empty
    ---@type number
    self.module = 0
end

---@param errorCode number 
---@param errorMessage string 
---@param module number 
---@return Error
function Error:new(errorCode, errorMessage, module)
    self.errorCode = errorCode --- int
    self.errorMessage = errorMessage --- java.lang.String
    self.module = module --- int
    return self
end

---@return number
function Error:protocolId()
    return 101
end

---@return string
function Error:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            errorCode = self.errorCode,
            errorMessage = self.errorMessage,
            module = self.module
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return Error
function Error:read(data)

    local packet = self:new(
            data.errorCode,
            data.errorMessage,
            data.module)
    return packet
end

--- 
---@return number 
function Error:getErrorCode()
    return self.errorCode
end
--- 
---@type  string 
function Error:getErrorMessage()
    return self.errorMessage
end

--- 
---@return number 
function Error:getModule()
    return self.module
end


return Error
