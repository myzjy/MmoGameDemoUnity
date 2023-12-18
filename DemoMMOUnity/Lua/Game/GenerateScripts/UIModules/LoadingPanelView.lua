---@class LoadingPanelView
LoadingPanelView = class("LoadingPanelView")
function LoadingPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.leftSlider_Slider = self._UIView:GetObjTypeStr("leftSlider_Slider") or CS.UnityEngine.UI.Slider
	self.rightSlider_Slider = self._UIView:GetObjTypeStr("rightSlider_Slider") or CS.UnityEngine.UI.Slider
	self.progressNum_Text = self._UIView:GetObjTypeStr("progressNum_Text") or CS.UnityEngine.UI.Text
	self.LoadingController = self._UIView:GetObjTypeStr("LoadingController") or CS.UnityEngine.Object
end

