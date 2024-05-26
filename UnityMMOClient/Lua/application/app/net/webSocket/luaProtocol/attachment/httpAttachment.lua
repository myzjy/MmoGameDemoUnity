---@class HttpAttachment
local HttpAttachment = class("HttpAttachment")
function HttpAttachment:ctor()
    ---@type number
    self.taskExecutorHash = 0
    ---@type number
    self.uid = 0
end

---@param taskExecutorHash number 
---@param uid number 
---@return HttpAttachment
function HttpAttachment:new(taskExecutorHash, uid)
    self.taskExecutorHash = taskExecutorHash --- int
    self.uid = uid --- long
    return self
end

---@return number
function HttpAttachment:protocolId()
    return 4
end

---@return string
function HttpAttachment:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            taskExecutorHash = self.taskExecutorHash,
            uid = self.uid
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return HttpAttachment
function HttpAttachment:read(data)

    local packet = self:new(
            data.taskExecutorHash,
            data.uid)
    return packet
end

--- 
---@return number 
function HttpAttachment:getTaskExecutorHash()
    return self.taskExecutorHash
end
--- 
---@return number 
function HttpAttachment:getUid()
    return self.uid
end


return HttpAttachment
