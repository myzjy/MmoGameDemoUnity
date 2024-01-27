---@class BagUIPanelView
BagUIPanelView = class("BagUIPanelView")
function BagUIPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.equipmentItem_UISerializableKeyObject = self._UIView:GetObjType("equipmentItem_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.equipmentItem_CanvasGroup = self._UIView:GetObjType("equipmentItem_CanvasGroup") or CS.UnityEngine.CanvasGroup
	self.topSelectTipsShowText = self._UIView:GetObjType("topSelectTipsShowText") or CS.UnityEngine.UI.Text
	self.CloseBtn = self._UIView:GetObjType("CloseBtn") or CS.UnityEngine.UI.Button
	self.equipmentButton = self._UIView:GetObjType("equipmentButton") or CS.UnityEngine.UI.Button
	self.equipmentButtons_UISerializableKeyObject = self._UIView:GetObjType("equipmentButtons_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.WeaponInDetailItem_UISerializableKeyObject = self._UIView:GetObjType("WeaponInDetailItem_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.weaponGrid = self._UIView:GetObjType("weaponGrid") or CS.UnityEngine.GameObject
end

