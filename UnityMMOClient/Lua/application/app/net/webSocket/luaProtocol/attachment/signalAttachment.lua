---@class SignalAttachment
local SignalAttachment = class("SignalAttachment")
function SignalAttachment:ctor()
    --- 0 for the server, 1 or 2 for the sync or async native client, 12 for the outside client such as browser, mobile
    ---@type number
    self.client = 0
    ---@type number
    self.signalId = 0
    ---@type number
    self.taskExecutorHash = 0
    ---@type number
    self.timestamp = 0
end

---@param client number 0 for the server, 1 or 2 for the sync or async native client, 12 for the outside client such as browser, mobile
---@param signalId number 
---@param taskExecutorHash number 
---@param timestamp number 
---@return SignalAttachment
function SignalAttachment:new(client, signalId, taskExecutorHash, timestamp)
    self.client = client --- byte
    self.signalId = signalId --- int
    self.taskExecutorHash = taskExecutorHash --- int
    self.timestamp = timestamp --- long
    return self
end

---@return number
function SignalAttachment:protocolId()
    return 0
end

---@return string
function SignalAttachment:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            client = self.client,
            signalId = self.signalId,
            taskExecutorHash = self.taskExecutorHash,
            timestamp = self.timestamp
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return SignalAttachment
function SignalAttachment:read(data)

    local packet = self:new(
            data.client,
            data.signalId,
            data.taskExecutorHash,
            data.timestamp)
    return packet
end

--- 0 for the server, 1 or 2 for the sync or async native client, 12 for the outside client such as browser, mobile
---@return number 0 for the server, 1 or 2 for the sync or async native client, 12 for the outside client such as browser, mobile
function SignalAttachment:getClient()
    return self.client
end
--- 
---@return number 
function SignalAttachment:getSignalId()
    return self.signalId
end
--- 
---@return number 
function SignalAttachment:getTaskExecutorHash()
    return self.taskExecutorHash
end
--- 
---@return number 
function SignalAttachment:getTimestamp()
    return self.timestamp
end


return SignalAttachment
