---@class NoAnswerAttachment
local NoAnswerAttachment = class("NoAnswerAttachment")
function NoAnswerAttachment:ctor()
    ---@type number
    self.taskExecutorHash = 0
end

---@param taskExecutorHash number 
---@return NoAnswerAttachment
function NoAnswerAttachment:new(taskExecutorHash)
    self.taskExecutorHash = taskExecutorHash --- int
    return self
end

---@return number
function NoAnswerAttachment:protocolId()
    return 5
end

---@return string
function NoAnswerAttachment:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            taskExecutorHash = self.taskExecutorHash
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return NoAnswerAttachment
function NoAnswerAttachment:read(data)

    local packet = self:new(
            data.taskExecutorHash)
    return packet
end

--- 
---@return number 
function NoAnswerAttachment:getTaskExecutorHash()
    return self.taskExecutorHash
end


return NoAnswerAttachment
