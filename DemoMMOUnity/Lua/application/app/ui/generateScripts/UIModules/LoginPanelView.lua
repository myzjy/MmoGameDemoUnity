---@class LoginPanelView
LoginPanelView = class("LoginPanelView")
function LoginPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.bg_Image = self._UIView:GetObjType("bg_Image") or CS.UnityEngine.UI.Image
	self.Gonggao_Button = self._UIView:GetObjType("Gonggao_Button") or CS.UnityEngine.UI.Button
	self.tips = self._UIView:GetObjType("tips") or CS.UnityEngine.GameObject
	self.UserNameText = self._UIView:GetObjType("UserNameText") or CS.UnityEngine.UI.Text
	self.LoginPart = self._UIView:GetObjType("LoginPart") or CS.UnityEngine.GameObject
	self.LoginPart_UISerializableKeyObject = self._UIView:GetObjType("LoginPart_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.RegisterPart_UISerializableKeyObject = self._UIView:GetObjType("RegisterPart_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.LoginStart_CanvasGroup = self._UIView:GetObjType("LoginStart_CanvasGroup") or CS.UnityEngine.CanvasGroup
	self.LoginStart_UISerializableKeyObject = self._UIView:GetObjType("LoginStart_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.LoginController = self._UIView:GetObjType("LoginController") or CS.ZJYFrameWork.Hotfix.UISerializable.LoginUIController
	self.AudioSource = self._UIView:GetObjType("AudioSource") or CS.UnityEngine.AudioSource
end

