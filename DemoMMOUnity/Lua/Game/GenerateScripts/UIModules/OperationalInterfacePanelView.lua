---@class OperationalInterfacePanelView
OperationalInterfacePanelView = class("OperationalInterfacePanelView")
function OperationalInterfacePanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.ReturnButton = self._UIView:GetObjTypeStr("ReturnButton") or CS.UnityEngine.UI.Button
	self.MapGuideObj = self._UIView:GetObjTypeStr("MapGuideObj") or CS.UnityEngine.GameObject
	self.Grid = self._UIView:GetObjTypeStr("Grid") or CS.ZJYFrameWork.Module.UICommon.AdaptiveReuseGrid
end

