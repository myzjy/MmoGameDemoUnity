---@class BagClickServerDataMsgRequet
local BagClickServerDataMsgRequet = class("BagClickServerDataMsgRequet")

function BagClickServerDataMsgRequet:ctor()
    ---@type string 点击的 名字
    self.clickNameHanler = string.empty
    ---@type number 根据 所选 type 返回协议不一样
    self.clickType = 0
end

return BagClickServerDataMsgRequet
