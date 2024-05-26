---@class Message
local Message = class("Message")
function Message:ctor()
    ---@type number
    self.code = 0
    ---@type string
    self.message = string.empty
    ---@type number
    self.module = 0
end

---@param code number 
---@param message string 
---@param module number 
---@return Message
function Message:new(code, message, module)
    self.code = code --- int
    self.message = message --- java.lang.String
    self.module = module --- byte
    return self
end

---@return number
function Message:protocolId()
    return 100
end

---@return string
function Message:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            code = self.code,
            message = self.message,
            module = self.module
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return Message
function Message:read(data)

    local packet = self:new(
            data.code,
            data.message,
            data.module)
    return packet
end

--- 
---@return number 
function Message:getCode()
    return self.code
end
--- 
---@type  string 
function Message:getMessage()
    return self.message
end

--- 
---@return number 
function Message:getModule()
    return self.module
end


return Message
