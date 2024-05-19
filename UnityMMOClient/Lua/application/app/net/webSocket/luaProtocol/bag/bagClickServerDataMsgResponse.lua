---@class BagClickServerDataMsgResponse
local BagClickServerDataMsgResponse = class("BagClickServerDataMsgResponse")

function BagClickServerDataMsgResponse:ctor()
    PrintDebug("invoke lua function ctor")
    ---@type number 返回我调用到面板 类型
    self.msgPalType = 0
    ---@type number 消息所属者
    self.msgMasterId = 0
    ---@type number 当前道具 武器 等所属玩家 id
    self.suMasterId = 0
    self.data = nil
end

function BagClickServerDataMsgResponse:protocolId()
    return 1042
end

return BagClickServerDataMsgResponse
