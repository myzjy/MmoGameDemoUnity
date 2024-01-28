---@class BagUIPanelView
BagUIPanelView = class("BagUIPanelView")
function BagUIPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.equipmentItem_UISerializableKeyObject = self._UIView:GetObjType("equipmentItem_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.topSelectTipsShowText = self._UIView:GetObjType("topSelectTipsShowText") or UnityEngine.UI.Text
	self.CloseBtn = self._UIView:GetObjType("CloseBtn") or UnityEngine.UI.Button
	self.equipmentButton = self._UIView:GetObjType("equipmentButton") or UnityEngine.UI.Button
	self.equipmentButtons_UISerializableKeyObject = self._UIView:GetObjType("equipmentButtons_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.WeaponInDetailItem_UISerializableKeyObject = self._UIView:GetObjType("WeaponInDetailItem_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.weaponGrid = self._UIView:GetObjType("weaponGrid") or UnityEngine.GameObject
end

