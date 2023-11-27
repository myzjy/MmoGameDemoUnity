--- 通知界面
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/11/27 14:54
---

---@class UICommonView:LuaUIObject
UICommonView = class("UICommonView", LuaUIObject);
function UICommonView:OnInit(gameObject)
    self.config = require("Game.UI.CommonUI.DialogConfig")
    local backgroundCloseTransform = gameObject.transform:Find(self.config.config.backgroundCloseButton)
    local titleTextTransform = gameObject.transform:Find(self.config.config.titleText)
    local BodyTextTransform = gameObject.transform:Find(self.config.config.BodyText)
    local NoButtonTransform = gameObject.transform:Find(self.config.config.NoButton)
    local NoButtonTextTransform = gameObject.transform:Find(self.config.config.NoButtonText)
    local YesButtonTransform = gameObject.transform:Find(self.config.config.YesButton)
    local YesButtonTextTransform = gameObject.transform:Find(self.config.config.YesButtonText)

    self.backgroundCloseButton = self:GetComponent(backgroundCloseTransform, "Button")
    self.titleText = self:GetComponent(titleTextTransform, "Text")
    self.BodyText = self:GetComponent(BodyTextTransform, "Text")
end
