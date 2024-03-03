local BagUIClickMsgVP = class("BagUIClickMsgVP")
function BagUIClickMsgVP:ctor()
    printDebug("")
    self:init()
end

--- bag msg click refush init
--- 背包界面初始化
function BagUIClickMsgVP:init()
    --显示 名字消息
    self.titleName = string.empty
    ---@type UnityEngine.UI.Text
    self.titleText = nil
end

return BagUIClickMsgVP
