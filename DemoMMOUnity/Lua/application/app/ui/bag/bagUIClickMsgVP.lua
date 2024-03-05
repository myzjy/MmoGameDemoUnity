local BagUIClickMsgVP = class("BagUIClickMsgVP")
function BagUIClickMsgVP:ctor()
    printDebug("")
    self:init()
end

--- bag msg click refush init
--- 背包界面初始化
function BagUIClickMsgVP:init()
    ---@type UnityEngine.UI.Image
    self.weaponIconImg = nil
    --显示 名字消息
    ---@type string
    self.titleName = string.empty
    --- 标题 Text组件
    ---@type UnityEngine.UI.Text
    self.titleText = nil
    --- 武器 type
    ---@type string
    self.weaponTypeName = string.empty
    --- 武器 type Text组件
    ---@type UnityEngine.UI.Text
    self.weaponTypeNameText = nil
    --- 表示 武器 副属性 总父物体
    ---@type UnityEngine.GameObject
    self.midWeaponViceObj = nil

    ---武器副属性 具体名字
    ---@type string
    self.midWeaponViceNameStr = string.empty
    --- 武器 副属性 名字 Text组件
    ---@type UnityEngine.UI.Text
    self.midWeaponViceNameText = nil
    --- 武器 副属性 具体数值
    ---@type string
    self.midWeaponViceNumStr = string.empty
    --- 武器 副属性 具体数值  Text组件
    ---@type UnityEngine.UI.Text
    self.midWeaponViceNumText = nil

    ---武器副属性 具体名字
    ---@type string
    self.midWeaponMainNameStr = string.empty
    --- 武器 副属性 名字 Text组件
    ---@type UnityEngine.UI.Text
    self.midWeaponMainNameText = nil
    --- 武器 副属性 具体数值
    ---@type string
    self.midWeaponMainNumStr = string.empty
    --- 武器 副属性 具体数值  Text组件
    ---@type UnityEngine.UI.Text
    self.midWeaponMainNumText = nil
end

return BagUIClickMsgVP
