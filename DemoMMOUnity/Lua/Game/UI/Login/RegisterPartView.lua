---
---datetime.datetime
---
---@class RegisterPartView:UIView
RegisterPartView = class("RegisterPartView", UIView)

function RegisterPartView:Build(view)
    self.rootCanvasGroup = view:GetObjType("root_CanvasGroup") or CS.UnityEngine.CanvasGroup
end
