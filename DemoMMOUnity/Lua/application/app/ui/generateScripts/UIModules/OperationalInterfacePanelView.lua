---@class OperationalInterfacePanelView
OperationalInterfacePanelView = class("OperationalInterfacePanelView")
function OperationalInterfacePanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.ReturnButton = self._UIView:GetObjType("ReturnButton") or UnityEngine.UI.Button
	self.MapGuideObj = self._UIView:GetObjType("MapGuideObj") or UnityEngine.GameObject
	self.Grid = self._UIView:GetObjType("Grid") or ZJYFrameWork.Module.UICommon.AdaptiveReuseGrid
end

