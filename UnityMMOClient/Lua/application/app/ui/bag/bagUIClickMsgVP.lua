---@class BagUIClickMsgVP
local BagUIClickMsgVP = class("BagUIClickMsgVP")
local WeaponStartIconObjView = require("application.app.ui.bag.modelView.weaponStartIconObjView")


function BagUIClickMsgVP:ctor()
    PrintDebug("init commopent")
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
    self.mid_icon = nil;
    ---显示 星星 数量的父物体
    ---@type UnityEngine.GameObject
    self.mid_starList = nil;

    --- 显示 武器 等级 数据
    ---@type UnityEngine.UI.Text
    self.weaponLvShowText = nil
    ---显示 武器 等级 数据
    ---@type UnityEngine.GameObject
    self.weaponLvShow = nil
    ---显示 强化到几阶
    ---@type UnityEngine.GameObject
    self.endTopStarList = nil
    --- 标识 武器 强化到 几阶
    ---@type UnityEngine.GameObject
    self.startObj = nil;
    ---@type ZJYFrameWork.UISerializable.UISerializableKeyObject
    self.startObj_UISerializableKeyObject = nil
    ---存放 生成 强化 星阶
    ---@type table<number,WeaponStartIconObjView>
    ---``` lua
    --- local data =  WeaponStartIconObjView(gameObject)
    --- local data = {
    ---     gameObject=nil,
    ---     icons_CanvasGroup:UnityEngine.CanvasGroup
    --- }
    ---```
    self.startObjSKeyList = {}
    --- 表示 当前武器 精炼 等阶
    ---@type UnityEngine.GameObject
    self.end_refine = nil;
    --- 武器 精炼到 几阶的数字
    ---@type UnityEngine.UI.Text
    self.end_refine_WeaponRefineNum_Text = nil;
    --- 武器 精炼到 几阶的数字 text
    ---@type UnityEngine.UI.Text
    self.end_refine_WeaponRefineNumStringText = nil;
    --- 武器技能
    ---@type UnityEngine.GameObject
    self.end_WeaponSkillIntroduce = nil;
    --- 技能文本 text 组件
    ---@type UnityEngine.UI.Text
    self.end_WeaponSkillIntroduce_Text = nil;
    ---@type UnityEngine.UI.Text
    self.end_WeaponIntroduce_Text = nil;
end

---@param dataCofig WeaponsConfigData
---@param weaponMsgData WeaponPlayerUserDataStruct
---@param uid number
function BagUIClickMsgVP:openShowWeaponMsg(dataCofig, weaponMsgData, uid)
    self.startObjSKeyList = {}
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
    self.weaponLvShowText = self.sKeyObj:GetObjTypeStr("end_top_WeaponNumText") or UnityEngine.UI.Text;

    self.mid_starList = self.sKeyObj:GetObjTypeStr("mid_starList") or UnityEngine.GameObject;
    self.mid_icon = self.sKeyObj:GetObjTypeStr("mid_icon") or UnityEngine.GameObject;

    self.starObj = self.sKeyObj:GetObjTypeStr("startObj") or UnityEngine.GameObject;
    self.end_refine = self.sKeyObj:GetObjTypeStr("end_refine") or UnityEngine.GameObject;
    self.end_refine_WeaponRefineNum_Text = self.sKeyObj:GetObjTypeStr("end_refine_WeaponRefineNum_Text") or
        UnityEngine.UI.Text;
    self.end_refine_WeaponRefineNumStringText = self.sKeyObj:GetObjTypeStr("end_refine_WeaponRefineNumStringText") or
        UnityEngine.UI.Text;

    self.end_WeaponSkillIntroduce = self.sKeyObj:GetObjTypeStr("end_WeaponSkillIntroduce") or UnityEngine.GameObject;
    self.end_WeaponIntroduce_Text = self.sKeyObj:GetObjTypeStr("end_WeaponIntroduce_Text") or UnityEngine.UI.Text;

    self.titleText.text = dataCofig.weaponName
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
        --- self:openShowWeaponMsg()
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
