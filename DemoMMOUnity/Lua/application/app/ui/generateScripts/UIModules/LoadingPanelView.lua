---@class LoadingPanelView
LoadingPanelView = class("LoadingPanelView")
function LoadingPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.leftSlider_Slider = self._UIView:GetObjType("leftSlider_Slider") or UnityEngine.UI.Slider
	self.rightSlider_Slider = self._UIView:GetObjType("rightSlider_Slider") or UnityEngine.UI.Slider
	self.progressNum_Text = self._UIView:GetObjType("progressNum_Text") or UnityEngine.UI.Text
	self.LoadingController = self._UIView:GetObjType("LoadingController") or UnityEngine.Object
end

