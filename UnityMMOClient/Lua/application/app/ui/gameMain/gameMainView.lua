---@class GameMainView:UIView
local GameMainView = class("GameMainView", UIView())
GameMainConfig = {
    prefabName = "GameMainUIPanel",
    --- 当前会生成在那一层
    canvasType = UIConfigEnum.UICanvasType.UI,
    sortType = UIConfigEnum.UISortType.First,
    --- 当前 UI 交互事件 消息
    eventNotification = {
        --- 打开游戏主界面
        OPEN_GAMEMAINPANEL_UI = "OPEN_GAMEMAINPANEL_UI",
        --- 关闭 游戏主界面
        CLOSE_GAMEMAINPANEL_UI = "CLOSE_GAMEMAINPANEL_UI",
    }
}
function GameMainView:OnLoad()
    self.UIConfig = {
        Config = GameMainConfig,
        viewPanel = GameMainUIPanelView,
        initFunc = function()
            PrintDebug("GameMainView:Init()")
            GameMainView:OnInit()
        end,
        showFunc = function()
            PrintDebug("GameMainView:showUI()-->showFunc")
            GameMainView:OnShow()
        end,
        hideFunc = function()
            -- UpdateBeat:RemoveListener(self:Update())
        end,
    }
    self:Load(self.UIConfig)
    self:LoadUI(self.UIConfig)
    self:InstanceOrReuse()
end

function GameMainView:OnInit()
    PrintDebug("call GameMainView Lua Script function to OnInit ....")
    ---@type GameMainUIPanelView
    local viewPanel = self.viewPanel

    self.GemsTimButton = viewPanel.GemsTim_UISerializableKeyObject:GetObjTypeStr("click") or CS.UnityEngine.UI.Button
    self.GemsText = viewPanel.GemsTim_UISerializableKeyObject:GetObjTypeStr("numText") or CS.UnityEngine.UI.Text
    --设置用户名
    self:SetTopHeadNameText(PlayerUserCaCheData:GetUseName())
    self.GemText = viewPanel.Gem_UISerializableKeyObject:GetObjTypeStr("numText") or CS.UnityEngine.UI.Text
    self.GoldCoinText = viewPanel.glod_UISerializableKeyObject:GetObjTypeStr("numText") or CS.UnityEngine.UI.Text
    self.GoldTimButton = viewPanel.glod_UISerializableKeyObject:GetObjTypeStr("click") or CS.UnityEngine.UI.Button
    GameMainUIViewController:GetInstance():Build(self)
    GameMainUIViewController:GetInstance():UIInitEvent()
end

---设置用户名
---@param userName string 用户名字
function GameMainView:SetTopHeadNameText(userName)
    PrintDebug("call GameMainView " ..
        " Lua Script function to SetTopHeadNameText" .. " set userName value: " .. userName)
    ---@type GameMainUIPanelView
    local viewPanel = self.viewPanel
    viewPanel.top_head_Name_Text.text = userName
end

-- 设置显示体力
---@param noPhysicalPower number 当前体力
---@param maxPhysicalPower number 最大体力
function GameMainView:SetPhysicalPowerText(noPhysicalPower, maxPhysicalPower)
    ---@type GameMainUIPanelView
    local viewPanel = self.viewPanel
    viewPanel.physicalPowerTip_Text.text = noPhysicalPower .. "/" .. maxPhysicalPower
    viewPanel.physicalPower_Text.text = noPhysicalPower .. ""
end

function GameMainView:OnShow()
    PrintInfo("GameMainView:OnShow line 74")
    self.UIView:OnShow()
    -- GameMainUIViewController:GetInstance():Open()
end

return GameMainView
