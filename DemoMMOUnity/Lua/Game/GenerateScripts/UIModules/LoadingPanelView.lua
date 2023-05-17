local LoadingPanelView = BaseClass()
local _UIView = {}
function LoadingPanelView:Init(view)
	_UIView = view:GetComponent("UIView")
	self.leftSlider_Slider = _UIView:GetObjType("leftSlider_Slider") or CS.UnityEngine.UI.Slider
	self.rightSlider_Slider = _UIView:GetObjType("rightSlider_Slider") or CS.UnityEngine.UI.Slider
	self.progressNum_Text = _UIView:GetObjType("progressNum_Text") or CS.UnityEngine.UI.Text
	self.LoadingController = _UIView:GetObjType("LoadingController") or CS.ZJYFrameWork.Hotfix.UISerializable.LoadUIController
end

return LoadingPanelView