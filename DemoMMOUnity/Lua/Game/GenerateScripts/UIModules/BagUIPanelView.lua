---@class BagUIPanelView
BagUIPanelView = class("BagUIPanelView")
function BagUIPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.EquipAndItemPanel = self._UIView:GetObjType("EquipAndItemPanel") or CS.UnityEngine.GameObject
end

