---@class BagUIPanelView
BagUIPanelView = class("BagUIPanelView")
function BagUIPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.equipmentOtem = self._UIView:GetObjTypeStr("equipmentOtem") or CS.UnityEngine.GameObject
end

