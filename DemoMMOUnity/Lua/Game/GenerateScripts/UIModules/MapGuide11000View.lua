---@class MapGuide11000View
MapGuide11000View = class("MapGuide11000View")
function MapGuide11000View:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.Grid01_Button = self._UIView:GetObjTypeStr("Grid01_Button") or CS.UnityEngine.UI.Button
end

