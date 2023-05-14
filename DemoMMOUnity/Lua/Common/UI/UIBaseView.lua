local UIBaseView = BaseClass()
local viewPanel={}
function UIBaseView:Create(uiPanelView)
    local uiView = UIBaseView:New();

    -- uiView.SetUIView(View,uiPanelView)
    self.viewPanel=uiPanelView;
    return uiView

end
function UIBaseView:ReUse()
    self.SelfUIView:OnShow()
    UIBaseView:OnShow()
end

function UIBaseView:OnShow()
end
-- function UIBaseView:SetUIView(view)
--     print("初始化 set"..view)
--     local uiView = view:GetComponent("UIView")
--     self.SelfUIView = uiView
--     print("UIBaseView:SetUIView Init"..self.SelfUIView)
--     self.viewPanel.Init(view)
-- end
--- 隐藏 UI
function UIBaseView:OnHide()
    self.SelfUIView.OnClose()
end
UIBaseView.viewPanel=viewPanel
return UIBaseView