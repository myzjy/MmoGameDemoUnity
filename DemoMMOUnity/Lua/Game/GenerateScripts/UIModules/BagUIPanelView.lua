---@class BagUIPanelView
BagUIPanelView = class("BagUIPanelView")
function BagUIPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.equipmentOtem = self._UIView:GetObjType("equipmentOtem") or CS.UnityEngine.GameObject
end

