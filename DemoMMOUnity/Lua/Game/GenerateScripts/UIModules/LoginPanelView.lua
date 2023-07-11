---@class LoginPanelView
LoginPanelView = class("LoginPanelView")
function LoginPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.bg_Image = self._UIView:GetObjType("bg_Image") or CS.UnityEngine.UI.Image
	self.Gonggao_Button = self._UIView:GetObjType("Gonggao_Button") or CS.UnityEngine.UI.Button
	self.tips = self._UIView:GetObjType("tips") or CS.UnityEngine.GameObject
	self.UserNameText = self._UIView:GetObjType("UserNameText") or CS.UnityEngine.UI.Text
	self.LoginPart = self._UIView:GetObjType("LoginPart") or CS.UnityEngine.GameObject
	self.LoginPartView = self._UIView:GetObjType("LoginPartView") or CS.ZJYFrameWork.UISerializable.LoginPartView
	self.RegisterPartView = self._UIView:GetObjType("RegisterPartView") or CS.ZJYFrameWork.UISerializable.RegisterPartView
	self.LoginTapToStartView = self._UIView:GetObjType("LoginTapToStartView") or CS.ZJYFrameWork.Hotfix.UISerializable.LoginTapToStartView
	self.LoginController = self._UIView:GetObjType("LoginController") or CS.ZJYFrameWork.Hotfix.UISerializable.LoginUIController
	self.AudioSource = self._UIView:GetObjType("AudioSource") or CS.UnityEngine.AudioSource
end

