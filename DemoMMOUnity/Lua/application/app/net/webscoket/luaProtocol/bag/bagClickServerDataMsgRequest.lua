---@class BagClickServerDataMsgRequest
local BagClickServerDataMsgRequest = class("BagClickServerDataMsgRequet")

function BagClickServerDataMsgRequest:ctor()
    ---@type string 点击的 名字
    self.clickNameHandler = string.empty
    ---@type number 根据 所选 type 返回协议不一样
    self.clickType = 0
    ---@type number 根据不同的武器 道具id 服务器处理不一样
    self.id=0
    ---@type number 具体是那个玩家
    self.masterUserId=0;
end

return BagClickServerDataMsgRequest
