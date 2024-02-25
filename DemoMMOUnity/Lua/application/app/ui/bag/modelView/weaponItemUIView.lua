local WeaponItemUIView = class("WeaponItemUIView", LuaUIObject())
function WeaponItemUIView:ctor()
    ---@type UnityEngine.GameObject
    self.gameObject = nil
    ---@type ZJYFrameWork.UISerializable.UISerializableKeyObject
    self.uSKeyObject = nil
    ---@type UnityEngine.CanvasGroup
    self.itemCanvasGroup = nil
    ---@type UnityEngine.UI.Image
    self.itemBgImage = nil
    ---@type UnityEngine.UI.Image
    self.itemIcon = nil
    ---@type UnityEngine.UI.Text
    self.itemNumText = nil
    ---@type UnityEngine.UI.Button
    self.clicBtn = nil
end

function WeaponItemUIView:Init(thisKeyObject)
    self.gameObject = thisKeyObject
    self.uSKeyObject = self.gameObject:GetComponent("UISerializableKeyObject")
    self.itemCanvasGroup=self.uSKeyObject:GetObjTypeStr("itemCanvasGroup") or UnityEngine.CanvasGroup
    self.itemBgImage=self.uSKeyObject:GetObjTypeStr("itemBgImage") or UnityEngine.UI.Image
    self.itemIcon=self.uSKeyObject:GetObjTypeStr("itemIcon") or UnityEngine.UI.Image
    self.itemNumText=self.uSKeyObject:GetObjTypeStr("itemNumText") or UnityEngine.UI.Text
    self.clicBtn=self.uSKeyObject:GetObjTypeStr("clicBtn") or UnityEngine.UI.Button
end

return WeaponItemUIView
