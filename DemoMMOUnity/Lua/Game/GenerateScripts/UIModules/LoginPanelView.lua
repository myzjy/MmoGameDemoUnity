---@class LoginPanelView
local LoginPanelView = BaseClass()
local _UIView = {}
function LoginPanelView:Init(view)
	_UIView = view:GetComponent("UIView")
	self.LoginPart = _UIView:GetObjType("LoginPart") or CS.UnityEngine.GameObject
	self.LoginPartView = _UIView:GetObjType("LoginPartView") or CS.ZJYFrameWork.UISerializable.LoginPartView
	self.RegisterPartView = _UIView:GetObjType("RegisterPartView") or CS.ZJYFrameWork.UISerializable.RegisterPartView
	self.LoginTapToStartView = _UIView:GetObjType("LoginTapToStartView") or CS.ZJYFrameWork.Hotfix.UISerializable.LoginTapToStartView
	self.LoginController = _UIView:GetObjType("LoginController") or CS.ZJYFrameWork.Hotfix.UISerializable.LoginController
	self.tips = _UIView:GetObjType("tips") or CS.UnityEngine.GameObject
	self.UserNameText = _UIView:GetObjType("UserNameText") or CS.UnityEngine.UI.Text
	self.Gonggao_Button = _UIView:GetObjType("Gonggao_Button") or CS.UnityEngine.UI.Button
	self.bg_Image = _UIView:GetObjType("bg_Image") or CS.UnityEngine.UI.Image
end

return LoginPanelView