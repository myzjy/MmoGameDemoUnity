---@class OperationalInterfacePanelView
OperationalInterfacePanelView = class("OperationalInterfacePanelView")
function OperationalInterfacePanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.ReturnButton = self._UIView:GetObjType("ReturnButton") or CS.UnityEngine.UI.Button
	self.MapGuideObj = self._UIView:GetObjType("MapGuideObj") or CS.UnityEngine.GameObject
	self.Grid = self._UIView:GetObjType("Grid") or CS.ZJYFrameWork.Module.UICommon.AdaptiveReuseGrid
end

