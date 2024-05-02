---@class GameMainUIViewController:LuaUIObject
local GameMainUIViewController = class("GameMainUIViewController", LuaUIObject)
local gameMainView = require("application.app.ui.gameMain.gameMainView")
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
    PrintDebug("call function open ")
    if self.GameMainView == nil then
        if Debug > 0 then
            PrintError("GameMainView 并未打开界面 Open 生成")
        end
        local viewPanel = gameMainView()
        -- 去生成界面
        viewPanel:OnLoad()
        return
    end
    self:OnClose()
    self.GameMainView:OnShow()
end

function GameMainUIViewController:OnClose()
    if self.GameMainView == nil then
        if Debug > 0 then
            PrintError("GameMainView 并未打开界面 生成 OnClose")
        end
        return
    end
    if self.GameMainView.reUse then
        self.GameMainView:OnHide()
    else
        if Debug > 0 then
            PrintError("GameMainView  OnClose 并未打开界面 生成")
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
    self:OpenServiceSendEvent()
end

--- 初始化打开界面得时候，发送
function GameMainUIViewController:InitSendEvent()

end

--- 打开界面得时候，发送 那些协议过去
function GameMainUIViewController:OpenServiceSendEvent()
    -- 更新 显示 金币 砖石 付费砖石 协议
    GameMainNetController:SendGameMainUIPanelRequest()
    -- 更新角色状态
end

function GameMainUIViewController:SendPhysicalPowerDown()
    local physicalPowerInfo = ServerConfigNetController:GetPhysicalPowerInfoData()
    --- 体力 对比
    if physicalPowerInfo:getMaximumStrength() > physicalPowerInfo:getNowPhysicalPower() then
        self.physicalCoro = StartCoroutine(function()
            while true do
                coroutine.yield(UnityEngine.WaitForSeconds(1))

                -- 发送协议
                local userInfo = ServerConfigNetController:GetPlayerInfo()
                GameMainNetController:SendPhysicalPowerService(userInfo.uid)
            end
        end)
    end
end

return GameMainUIViewController
