---
---datetime.datetime
---
---@class RegisterPartView:UIBaseView
local RegisterPartView = class("RegisterPartView", UIBaseView())

function RegisterPartView:ctor(view)
    self.rootCanvasGroup = view:GetObjTypeStr("root_CanvasGroup") or UnityEngine.CanvasGroup
    self.rootObj = view:GetObjTypeStr("root") or UnityEngine.GameObject
    self.root = self.rootObj.transform
    self.registerAccountInputField = view:GetObjTypeStr("registerAccountInputField") or UnityEngine.UI.InputField
    self.registerPasswordInputField = view:GetObjTypeStr("registerPasswordInputField") or UnityEngine.UI.InputField
    self.registerAffirmPasswordInputField = view:GetObjTypeStr("registerAffirmPasswordInputField") or
        UnityEngine.UI.InputField
    self.okButton = view:GetObjTypeStr("okButton") or UnityEngine.UI.Button
    self.cancelButton = view:GetObjTypeStr("cancelButton") or UnityEngine.UI.Button
    self:SetListener(self.okButton, function()
        local account = self.registerAccountInputField.text
        local password = self.registerPasswordInputField.text
        local affirmPassword = self.registerAffirmPasswordInputField.text
        GameEvent.RegisterAccount(account, password, affirmPassword)
    end)
    self:SetListener(self.cancelButton, function()
        LoginUIController:GetInstance():Open()
    end)
end

function RegisterPartView:OnShow()
    self.rootObj:SetActive(true)
    self.rootCanvasGroup.alpha = 1
    self.rootCanvasGroup.interactable = true
    self.rootCanvasGroup.blocksRaycasts = true
end

function RegisterPartView:OnHide()
    if self.rootObj.activeSelf ~= true then
        return
    end
    self.rootCanvasGroup.alpha = 0
    self.rootCanvasGroup.interactable = false
    self.rootCanvasGroup.blocksRaycasts = false
    self.rootObj:SetActive(false)
    LoginUIController:GetInstance():Open()
end

return RegisterPartView
