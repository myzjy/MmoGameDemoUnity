---@class BagClickServerDataMsgResponse
local BagClickServerDataMsgResponse = class("BagClickServerDataMsgResponse")
function BagClickServerDataMsgResponse:ctor()
    ---@type BagClickMsgDataPanel
    self.data = {}
    --- id
    ---@type number
    self.id = 0
    --- 消息所属者
    ---@type number
    self.msgMasterId = 0
    --- 返回我调用到面板类型
    ---@type number
    self.msgPalType = 0
    --- 当前道具武器等所属玩家
    ---@type number
    self.subMasterId = 0
end

---@param data BagClickMsgDataPanel 
---@param id number id
---@param msgMasterId number 消息所属者
---@param msgPalType number 返回我调用到面板类型
---@param subMasterId number 当前道具武器等所属玩家
---@return BagClickServerDataMsgResponse
function BagClickServerDataMsgResponse:new(data, id, msgMasterId, msgPalType, subMasterId)
    self.data = data --- com.gameServer.common.protocol.bag.BagClickMsgDataPanel
    self.id = id --- int
    self.msgMasterId = msgMasterId --- long
    self.msgPalType = msgPalType --- int
    self.subMasterId = subMasterId --- long
    return self
end

---@return number
function BagClickServerDataMsgResponse:protocolId()
    return 1042
end

---@return string
function BagClickServerDataMsgResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            data = self.data,
            id = self.id,
            msgMasterId = self.msgMasterId,
            msgPalType = self.msgPalType,
            subMasterId = self.subMasterId
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return BagClickServerDataMsgResponse
function BagClickServerDataMsgResponse:read(data)
    local dataPacket = BagClickMsgDataPanel()
    local data = dataPacket:read(data.data)

    local packet = self:new(
            data,
            data.id,
            data.msgMasterId,
            data.msgPalType,
            data.subMasterId)
    return packet
end

--- 
---@return BagClickMsgDataPanel 
function BagClickServerDataMsgResponse:getBagClickMsgDataPanel()
    return self.data
end
--- id
---@return number id
function BagClickServerDataMsgResponse:getId()
    return self.id
end
--- 消息所属者
---@return number 消息所属者
function BagClickServerDataMsgResponse:getMsgMasterId()
    return self.msgMasterId
end
--- 返回我调用到面板类型
---@return number 返回我调用到面板类型
function BagClickServerDataMsgResponse:getMsgPalType()
    return self.msgPalType
end
--- 当前道具武器等所属玩家
---@return number 当前道具武器等所属玩家
function BagClickServerDataMsgResponse:getSubMasterId()
    return self.subMasterId
end


return BagClickServerDataMsgResponse
