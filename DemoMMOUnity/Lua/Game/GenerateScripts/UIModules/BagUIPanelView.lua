local BagUIPanelView = BaseClass()
local _UIView = {}
function BagUIPanelView:Init(view)
	_UIView = view:GetComponent("UIView")
	self.EquipAndItemPanel = _UIView:GetObjType("EquipAndItemPanel") or CS.UnityEngine.GameObject
end

return BagUIPanelView