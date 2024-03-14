---@class BagUIPanelView
BagUIPanelView = class("BagUIPanelView")
function BagUIPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.equipmentItem_UISerializableKeyObject = self._UIView:GetObjTypeStr("equipmentItem_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.topSelectTipsShowText = self._UIView:GetObjTypeStr("topSelectTipsShowText") or UnityEngine.UI.Text
	self.CloseBtn = self._UIView:GetObjTypeStr("CloseBtn") or UnityEngine.UI.Button
	self.equipmentButton = self._UIView:GetObjTypeStr("equipmentButton") or UnityEngine.UI.Button
	self.equipmentButtons_UISerializableKeyObject = self._UIView:GetObjTypeStr("equipmentButtons_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.WeaponInDetailItem_UISerializableKeyObject = self._UIView:GetObjTypeStr("WeaponInDetailItem_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.weaponGrid = self._UIView:GetObjTypeStr("weaponGrid") or UnityEngine.GameObject
end

