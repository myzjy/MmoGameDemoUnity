---
---datetime.datetime
---
---@class RegisterPartView:UIBaseView
local RegisterPartView = class("RegisterPartView", UIBaseView)

function RegisterPartView:Build(view)
    self.rootCanvasGroup = view:GetObjType("root_CanvasGroup") or CS.UnityEngine.CanvasGroup
    self.rootObj = view:GetObjType("root") or CS.UnityEngine.GameObject
    self.root = self.rootObj.transform
    self.registerAccountInputField = view:GetObjType("registerAccountInputField") or CS.UnityEngine.UI.InputField
    self.registerPasswordInputField = view:GetObjType("registerPasswordInputField") or CS.UnityEngine.UI.InputField
    self.registerAffirmPasswordInputField = view:GetObjType("registerAffirmPasswordInputField") or
        CS.UnityEngine.UI.InputField
    self.okButton = view:GetObjType("okButton") or CS.UnityEngine.UI.Button
    self.cancelButton = view:GetObjType("cancelButton") or CS.UnityEngine.UI.Button
end

function RegisterPartView:OnShow()
    self.rootCanvasGroup:DOKill()
    self.rootCanvasGroup:DOFade(0, 0)
    self.rootCanvasGroup:DOFade(0, 0)
        :SetEase(CS.DG.Tweening.Ease.Linear)
        :SetLoops(CS.DG.Tweening.LoopType.Yoyo)
        :OnComplete(function()
            self.rootCanvasGroup.interactable = true
            self.rootCanvasGroup.blocksRaycasts = true
            self.rootObj:SetActive(true)
        end)
end

function RegisterPartView:OnHide()
    if self.rootObj.activeSelf ~= true then
        return
    end
    self.rootCanvasGroup.DOKill();
    self.rootCanvasGroup.DOFade(0, 0);
    self.rootCanvasGroup.DOFade(0, 0.2)
        :SetEase(CS.DG.Tweening.Ease.Linear)
        :SetLoops(3, CS.DG.Tweening.LoopType.Yoyo)
        :OnComplete(function()
            self.rootCanvasGroup.interactable = false
            self.rootCanvasGroup.blocksRaycasts = false
            -- PlayManager.Instance.LoadScene(Data.scene_home);
            self.rootObj:SetActive(false)
            LoginUIController:GetInstance():Open()
        end
        );
end

return RegisterPartView