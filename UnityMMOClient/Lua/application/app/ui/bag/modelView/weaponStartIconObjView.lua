---@class WeaponStartIconObjView
local WeaponStartIconObjView = class("WeaponStartIconObjView")

---@param gameObject UnityEngine.GameObject
function WeaponStartIconObjView:ctor(gameObject)
    self:OnDestroy()
    self.gameObject = gameObject
    ---@type ZJYFrameWork.UISerializable.UISerializableKeyObject
    self.sKeyObj = gameObject:GetComponent("UISerializableKeyObject");
    ---@type UnityEngine.CanvasGroup
    self.icons_CanvasGroup = LuaUtils.GetUIKeyCanvasGroup(self.sKeyObj, "icons_CanvasGroup");
end

--- 初始化
function WeaponStartIconObjView:init()
    self.gameObject:SetActive(true);
    self.icons_CanvasGroup.alpha = 0;
end

function WeaponStartIconObjView:Open()
    self.icons_CanvasGroup.alpha = 1;
end

function WeaponStartIconObjView:closeIcon()
    self.icons_CanvasGroup.alpha = 0;
end

function WeaponStartIconObjView:OnDestroy()
    self.gameObject = nil;
    self.sKeyObj = nil;
    self.icons_CanvasGroup = nil;
end

return WeaponStartIconObjView
