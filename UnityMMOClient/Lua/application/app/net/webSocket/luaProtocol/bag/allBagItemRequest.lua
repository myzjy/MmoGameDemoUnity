---@class AllBagItemRequest
local AllBagItemRequest = class("AllBagItemRequest")
function AllBagItemRequest:ctor()
    --- 背包的协议号
    ---@type string
    self.msgProtocol = string.empty
    --- 调用协议 之后 在我点击
    ---@type number
    self.type = 0
end

---@param msgProtocol string 背包的协议号
---@param type number 调用协议 之后 在我点击
---@return AllBagItemRequest
function AllBagItemRequest:new(msgProtocol, type)
    self.msgProtocol = msgProtocol --- java.lang.String
    self.type = type --- int
    return self
end

---@return number
function AllBagItemRequest:protocolId()
    return 1007
end

---@return string
function AllBagItemRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            msgProtocol = self.msgProtocol,
            type = self.type
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return AllBagItemRequest
function AllBagItemRequest:read(data)

    local packet = self:new(
            data.msgProtocol,
            data.type)
    return packet
end

--- 背包的协议号
---@type  string 背包的协议号
function AllBagItemRequest:getMsgProtocol()
    return self.msgProtocol
end

--- 调用协议 之后 在我点击
---@return number 调用协议 之后 在我点击
function AllBagItemRequest:getType()
    return self.type
end


return AllBagItemRequest
