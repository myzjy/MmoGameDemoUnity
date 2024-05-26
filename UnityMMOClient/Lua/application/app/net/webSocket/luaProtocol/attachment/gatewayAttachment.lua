---@class GatewayAttachment
local GatewayAttachment = class("GatewayAttachment")
function GatewayAttachment:ctor()
    ---@type boolean
    self.client = 0
    ---@type number
    self.sid = 0
    ---@type SignalAttachment
    self.signalAttachment = {}
    ---@type number
    self.taskExecutorHash = 0
    ---@type number
    self.uid = 0
end

---@param boolean boolean 
---@param sid number 
---@param signalAttachment SignalAttachment 
---@param taskExecutorHash number 
---@param uid number 
---@return GatewayAttachment
function GatewayAttachment:new(client, sid, signalAttachment, taskExecutorHash, uid)
    self.client = client --- boolean
    self.sid = sid --- long
    self.signalAttachment = signalAttachment --- com.zfoo.net.router.attachment.SignalAttachment
    self.taskExecutorHash = taskExecutorHash --- int
    self.uid = uid --- long
    return self
end

---@return number
function GatewayAttachment:protocolId()
    return 2
end

---@return string
function GatewayAttachment:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            client = self.client,
            sid = self.sid,
            signalAttachment = self.signalAttachment,
            taskExecutorHash = self.taskExecutorHash,
            uid = self.uid
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return GatewayAttachment
function GatewayAttachment:read(data)
    local signalAttachmentPacket = SignalAttachment()
    local signalAttachment = signalAttachmentPacket:read(data.signalAttachment)

    local packet = self:new(
            data.client,
            data.sid,
            signalAttachment,
            data.taskExecutorHash,
            data.uid)
    return packet
end

--- 
---@return boolean 
function GatewayAttachment:getClient()
    return self.client
end
--- 
---@return number 
function GatewayAttachment:getSid()
    return self.sid
end
--- 
---@return SignalAttachment 
function GatewayAttachment:getSignalAttachment()
    return self.signalAttachment
end
--- 
---@return number 
function GatewayAttachment:getTaskExecutorHash()
    return self.taskExecutorHash
end
--- 
---@return number 
function GatewayAttachment:getUid()
    return self.uid
end


return GatewayAttachment
