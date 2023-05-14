local UIBaseView = BaseClass()
local uiPanelView={}

function UIBaseView:ReUse()
    self.SelfUIView:OnShow()
    UIBaseView:OnShow()
end

function UIBaseView:OnShow()
    uiPanelView.OnShow()
end

function UIBaseView:SetUIView(view,ViewPanel)
    self.SelfUIView = view
    uiPanelView = self.SelfUIView:GetComponent("UIView")
    self.viewPanel=ViewPanel
end

--- 隐藏 UI
function UIBaseView:OnHide()
    -- uiPanelView = self.SelfUIView:GetComponent("UIView")
    uiPanelView.OnClose()
end

return UIBaseView
