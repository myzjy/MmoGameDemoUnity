---@class BagClickServerDataMsgRequest
local BagClickServerDataMsgRequest = class("BagClickServerDataMsgRequest")
function BagClickServerDataMsgRequest:ctor()
    --- 点击的名字
    ---@type string
    self.clickNameHandler = string.empty
    --- 根据所选type返回协议不一样
    ---@type number
    self.clickType = 0
    --- 根据不同的武器道具id服务器处理不一样
    ---@type number
    self.id = 0
    --- 具体是那个玩家发送
    ---@type number
    self.masterUserId = 0
end

---@param clickNameHandler string 点击的名字
---@param clickType number 根据所选type返回协议不一样
---@param id number 根据不同的武器道具id服务器处理不一样
---@param masterUserId number 具体是那个玩家发送
---@return BagClickServerDataMsgRequest
function BagClickServerDataMsgRequest:new(clickNameHandler, clickType, id, masterUserId)
    self.clickNameHandler = clickNameHandler --- java.lang.String
    self.clickType = clickType --- int
    self.id = id --- int
    self.masterUserId = masterUserId --- long
    return self
end

---@return number
function BagClickServerDataMsgRequest:protocolId()
    return 1041
end

---@return string
function BagClickServerDataMsgRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            clickNameHandler = self.clickNameHandler,
            clickType = self.clickType,
            id = self.id,
            masterUserId = self.masterUserId
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return BagClickServerDataMsgRequest
function BagClickServerDataMsgRequest:read(data)

    local packet = self:new(
            data.clickNameHandler,
            data.clickType,
            data.id,
            data.masterUserId)
    return packet
end

--- 点击的名字
---@type  string 点击的名字
function BagClickServerDataMsgRequest:getClickNameHandler()
    return self.clickNameHandler
end

--- 根据所选type返回协议不一样
---@return number 根据所选type返回协议不一样
function BagClickServerDataMsgRequest:getClickType()
    return self.clickType
end
--- 根据不同的武器道具id服务器处理不一样
---@return number 根据不同的武器道具id服务器处理不一样
function BagClickServerDataMsgRequest:getId()
    return self.id
end
--- 具体是那个玩家发送
---@return number 具体是那个玩家发送
function BagClickServerDataMsgRequest:getMasterUserId()
    return self.masterUserId
end


return BagClickServerDataMsgRequest
