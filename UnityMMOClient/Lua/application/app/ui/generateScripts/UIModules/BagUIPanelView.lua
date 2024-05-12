---@class BagUIPanelView
BagUIPanelView = class("BagUIPanelView")
function BagUIPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.weaponItem_UISerializableKeyObject = self._UIView:GetObjTypeStr("weaponItem_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.weaponItem = self._UIView:GetObjTypeStr("weaponItem") or UnityEngine.GameObject
	self.topSelectTipsShowText = self._UIView:GetObjTypeStr("topSelectTipsShowText") or UnityEngine.UI.Text
	self.CloseBtn = self._UIView:GetObjTypeStr("CloseBtn") or UnityEngine.UI.Button
	self.headerBtnButtons = self._UIView:GetObjTypeStr("headerBtnButtons") or UnityEngine.GameObject
	self.WeaponInDetailItem_UISerializableKeyObject = self._UIView:GetObjTypeStr("WeaponInDetailItem_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.weaponGrid = self._UIView:GetObjTypeStr("weaponGrid") or UnityEngine.GameObject
	self.TabHeadListGrid = self._UIView:GetObjTypeStr("TabHeadListGrid") or UnityEngine.GameObject
	self.WeaponListGroup = self._UIView:GetObjTypeStr("WeaponListGroup") or UnityEngine.CanvasGroup
end

