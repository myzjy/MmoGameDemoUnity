---@class BagUIPanelView
BagUIPanelView = class("BagUIPanelView")
function BagUIPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.equipmentItem_UISerializableKeyObject = self._UIView:GetObjType("equipmentItem_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.equipmentItem = self._UIView:GetObjType("equipmentItem") or CS.UnityEngine.GameObject
end

