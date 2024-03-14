---@class BagClickServerDataMsgRequest
local BagClickServerDataMsgRequest = class("BagClickServerDataMsgRequet")

function BagClickServerDataMsgRequest:ctor()
    ---@type string 点击的 名字
    self.clickNameHandler = string.empty
    ---@type number 根据 所选 type 返回协议不一样
    self.clickType = 0
    ---@type number 根据不同的武器 道具id 服务器处理不一样
    self.id = 0
    ---@type number 具体是那个玩家
    self.masterUserId = 0;
end

--- new 赋值
---@param clickNameHandler string
---@param clickType number
---@param id number
---@param masterUserId number
---@return BagClickServerDataMsgRequest
function BagClickServerDataMsgRequest:new(clickNameHandler, clickType, id, masterUserId)
    self.clickNameHandler = clickNameHandler
    self.clickType = clickType
    self.id = id
    self.masterUserId = masterUserId
    return self
end

function BagClickServerDataMsgRequest:protocolId()
    return 1041
end

function BagClickServerDataMsgRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = { clickNameHandler = self.clickNameHandler, clickType = self.clickType, id = self.id, masterUserId = self.masterUserId }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end
---后续不需要 解析  Request Data 只有Reponse 会有

return BagClickServerDataMsgRequest
