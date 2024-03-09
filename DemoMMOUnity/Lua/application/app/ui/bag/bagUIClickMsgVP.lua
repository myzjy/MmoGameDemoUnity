---@class BagUIClickMsgVP
local BagUIClickMsgVP = class("BagUIClickMsgVP")
---@type BagUIClickMsgVP
local this = nil
function BagUIClickMsgVP:ctor()
    printDebug("init commopent")
    this = self
    self:init()
end

--- bag msg click refush init
--- 背包界面初始化
function BagUIClickMsgVP:init()
    ---@type ZJYFrameWork.UISerializable.UISerializableKeyObject
    self.sKeyObj = nil
    ---@type UnityEngine.UI.Image|UnityEngine.Object
    self.weaponIconImg = nil
    --显示 名字消息
    ---@type string
    self.titleName = string.empty
    --- 标题 Text组件
    ---@type UnityEngine.UI.Text|UnityEngine.Object
    self.titleText = nil
    --- 武器 type
    ---@type string
    self.weaponTypeName = string.empty
    --- 武器 type Text组件
    ---@type UnityEngine.UI.Text|UnityEngine.Object
    self.weaponTypeNameText = nil
    --- 表示 武器 副属性 总父物体
    ---@type UnityEngine.GameObject
    self.midWeaponViceObj = nil

    --- 武器副属性 集合物体
    ---@type UnityEngine.GameObject |UnityEngine.Object
    self.midWeaponViceRoot = nil;
    ---武器副属性 具体名字
    ---@type string
    self.midWeaponViceNameStr = string.empty
    --- 武器 副属性 名字 Text组件
    ---@type UnityEngine.UI.Text|UnityEngine.Object
    self.midWeaponViceNameText = nil
    --- 武器 副属性 具体数值
    ---@type string
    self.midWeaponViceNumStr = string.empty
    --- 武器 副属性 具体数值  Text组件
    ---@type UnityEngine.UI.Text|UnityEngine.Object
    self.midWeaponViceNumText = nil


    ---@type UnityEngine.GameObject
    self.midWeaponMainRoot = nil;
    ---武器主属性 具体名字
    ---@type string
    self.midWeaponMainNameStr = string.empty
    --- 武器 主属性 名字 Text组件
    ---@type UnityEngine.UI.Text|UnityEngine.Object
    self.midWeaponMainNameText = nil
    --- 武器 主属性 具体数值
    ---@type string
    self.midWeaponMainNumStr = string.empty
    --- 武器 主属性 具体数值  Text组件
    ---@type UnityEngine.UI.Text|UnityEngine.Object
    self.midWeaponMainNumText = nil

    --- 星星 object
    ---@type UnityEngine.GameObject
    self.starObj = nil;
    ---显示 星星 数量的父物体
    ---@type UnityEngine.GameObject
    self.mid_starList = nil;

    --- 显示 武器 等级 数据
    ---@type UnityEngine.UI.Text
    self.weaponLvShow = nil
end

function BagUIClickMsgVP:openShowWeaponMsg(dataCofig, weaponMsgData, uid)
    self.titleText = self.sKeyObj:GetObjTypeStr("top_topWeaponName_Text") or UnityEngine.UI.Text;
    self.weaponTypeNameText = self.sKeyObj:GetObjTypeStr("mid_WeaponTypeName_Text") or UnityEngine.UI.Text;

    self.weaponIconImg = self.sKeyObj:GetObjTypeStr("mid_weaponIcon_Image") or UnityEngine.UI.Image;
    self.midWeaponViceRoot = self.sKeyObj:GetObjTypeStr("mid_midWeaponViceRoot") or UnityEngine.GameObject;
    self.midWeaponViceNameText = self.sKeyObj:GetObjTypeStr("midWeaponViceRoot_WeaponViceNameText") or
        UnityEngine.UI.Text;
    self.midWeaponViceNumText = self.sKeyObj:GetObjTypeStr("midWeaponViceRoot_WeaponViceNumText") or UnityEngine.UI
        .Text;

    self.midWeaponMainRoot = self.sKeyObj:GetObjTypeStr("mid_midWeaponMainRoot") or UnityEngine.GameObject;
    self.midWeaponMainNumText = self.sKeyObj:GetObjTypeStr("mid_WeaponMainNumText_Text") or UnityEngine.UI.Text;
    self.midWeaponMainNameText = self.sKeyObj:GetObjTypeStr("mid_WeaponMainName_Text") or UnityEngine.UI.Text;
end

--- 启动面板
---@param panel UnityEngine.GameObject
---@param type number
function BagUIClickMsgVP:Start(panel, type)
    panel:SetActive(true)
    self.gameObject = panel
    self.sKeyObj = self.gameObject:GetComponent("UISerializableKeyObject")


    --根据 type 打开面板信息不一样
    if type == 1 then
        ---武器面板
        self:openShowWeaponMsg()
    end
end

---清空面板相关信息
function BagUIClickMsgVP:ClosePanelMsg()
    self.midWeaponMainNumText.text = string.empty
    self.midWeaponMainNumText.text = string.empty
    self.midWeaponViceNameText.text = string.empty
    self.midWeaponViceNumText.text = string.empty
end

return BagUIClickMsgVP
