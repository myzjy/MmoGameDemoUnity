--- 基础
local UIBaseView = BaseClass()
--- 保存 C# 侧对应的预制体上面UIView
local uiPanelView={}

---复用 UI调度
function UIBaseView:ReUse()
    uiPanelView:OnShow()
    UIBaseView:OnShow()
end

---打开UI
function UIBaseView:OnShow()
    uiPanelView.OnShow()
end

---设置 UI基础面板
---@param view any 预制体
---@param ViewPanel any 预制体的面板数据转换 data
function UIBaseView:SetUIView(view,ViewPanel)
    self.SelfUIView = view
    --- 保存 预制体上面的 UIView
    uiPanelView = self.SelfUIView:GetComponent("UIView")
    self.viewPanel=ViewPanel
end
---返回 UIView
---@return table
function UIBaseView:GetUIView()
    return uiPanelView
end

--- 隐藏 UI
function UIBaseView:OnHide()
    -- uiPanelView = self.SelfUIView:GetComponent("UIView")
    --- 调用UIView 关闭
    uiPanelView.OnClose()
end

return UIBaseView
