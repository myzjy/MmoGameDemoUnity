---@class LoginPanelView
LoginPanelView = class("LoginPanelView")
function LoginPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.bg_Image = self._UIView:GetObjTypeStr("bg_Image") or UnityEngine.UI.Image
	self.Gonggao_Button = self._UIView:GetObjTypeStr("Gonggao_Button") or UnityEngine.UI.Button
	self.tips = self._UIView:GetObjTypeStr("tips") or UnityEngine.GameObject
	self.UserNameText = self._UIView:GetObjTypeStr("UserNameText") or UnityEngine.UI.Text
	self.LoginPart = self._UIView:GetObjTypeStr("LoginPart") or UnityEngine.GameObject
	self.LoginPart_UISerializableKeyObject = self._UIView:GetObjTypeStr("LoginPart_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.RegisterPart_UISerializableKeyObject = self._UIView:GetObjTypeStr("RegisterPart_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.LoginStart_CanvasGroup = self._UIView:GetObjTypeStr("LoginStart_CanvasGroup") or UnityEngine.CanvasGroup
	self.LoginStart_UISerializableKeyObject = self._UIView:GetObjTypeStr("LoginStart_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.AudioSource = self._UIView:GetObjTypeStr("AudioSource") or UnityEngine.AudioSource
end

