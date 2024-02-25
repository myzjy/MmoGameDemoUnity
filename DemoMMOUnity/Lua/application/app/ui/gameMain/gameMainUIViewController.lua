---@class GameMainUIViewController:LuaUIObject
local GameMainUIViewController = class("GameMainUIViewController", LuaUIObject)
---@type GameMainUIViewController
local instance = nil
function GameMainUIViewController:ctor()
    self.GameMainView = nil
end

function GameMainUIViewController:GetInstance()
    if not instance then
        instance = GameMainUIViewController()
    end
    return instance
end

function GameMainUIViewController:Open()
    if self.GameMainView == nil then
        if Debug > 0 then
            printError("GameMainView 并未打开界面 Open 生成")
        end
        -- 去生成界面
        GameMainView:OnLoad()
        return
    end
    self:OnClose()
    self.GameMainView.UIView:OnShow()
end

function GameMainUIViewController:OnClose()
    if self.GameMainView == nil then
        if Debug > 0 then
            printError("GameMainView 并未打开界面 生成 OnClose")
        end
        return
    end
    if self.GameMainView.reUse then
        self.GameMainView:OnHide()
    else
        if Debug > 0 then
            printError("GameMainView  OnClose 并未打开界面 生成")
        end
    end
    self.GameMainView:OnHide()
end

function GameMainUIViewController:Build(GameMainView)
    ---@type GameMainView|nil
    self.GameMainView = GameMainView
end

function GameMainUIViewController:UIInitEvent()
    ---@type GameMainUIPanelView
    local viewPanel = self.GameMainView.viewPanel
    self:SetListener(viewPanel.BagButton, function()
        -- 点击背包
        BagUIController:Open()
    end)
end
return GameMainUIViewController