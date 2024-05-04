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
    self.GemsText = LuaUtils.GetKeyUIText(viewPanel.GemsTim_UISerializableKeyObject.gameObject, "numText")
    self.GemText = LuaUtils.GetKeyUIText(viewPanel.Gem_UISerializableKeyObject.gameObject, "numText")
    self.GoldCoinText = LuaUtils.GetKeyUIText(viewPanel.glod_UISerializableKeyObject.gameObject, "numText")
    self.GoldTimButton = LuaUtils.GetKeyButtonGameObject(viewPanel.glod_UISerializableKeyObject.gameObject, "click")

    self:RegisterEvent()

    GameMainUIViewController:GetInstance():Build(self)
    GameMainUIViewController:GetInstance():UIInitEvent()
end

function GameMainView:RegisterEvent()
    UIUtils.AddEventListener(GameEvent.UpDateGemsAndGlodInfo, self.RefreshShowInfoData, self)
    UIUtils.AddEventListener(GameEvent.UPdateGameMainUserInfoMsg, self.RefreshShowUserInfoData, self)
    UIUtils.AddEventListener(GameEvent.UpdateGamePhysicalInfo, self.RefreshShowPhysicalPower, self)
    GameMainUIViewController:SendPhysicalPowerDown()

    -- UIUtils.AddEventListener(LateUpdateBeat, self.OnUpdate)
end

---设置用户名
---@param userName string 用户名字
function GameMainView:SetTopHeadNameText(userName)
    PrintDebug("call Lua Script function to SetTopHeadNameText set userName value: " .. userName)
    ---@type GameMainUIPanelView
    local viewPanel = self.viewPanel
    viewPanel.top_head_Name_Text.text = userName
end

--- comment
--- @param userInfo PhysicalPowerInfoData
function GameMainView:RefreshShowPhysicalPower(userInfo)
    self.userPhysicalInfo = userInfo
    self:SetPhysicalPowerText(self.userPhysicalInfo.nowPhysicalPower, self.userPhysicalInfo.maximumStrength)
end

-- 设置显示体力
---@param nowPhysicalPower number 当前体力
---@param maximumStrength number 最大体力
function GameMainView:SetPhysicalPowerText(nowPhysicalPower, maximumStrength)
    ---@type GameMainUIPanelView
    local viewPanel = self.viewPanel

    viewPanel.physicalPowerTip_Text.text = nowPhysicalPower .. "/" .. maximumStrength
    viewPanel.physicalPower_Text.text = nowPhysicalPower .. ""
end

function GameMainView:RefreshShowInfoData(userInfo)
    self.GemsText.text = userInfo.premiumDiamondNum .. ""
    self.GemText.text = userInfo.diamondNum .. ""
    self.GoldCoinText.text = userInfo.goldNum .. ""
end

function GameMainView:RefreshShowUserInfoData(userInfo)
    ---@type GameMainUIPanelView
    local viewPanel = self.viewPanel
    viewPanel.top_head_Lv_LvNum_Text.text = userInfo.lv .. ""
    viewPanel.top_head_Lv_Image.fillAmount = (userInfo.exp / userInfo.maxExp)
    self:SetTopHeadNameText(userInfo.userName)
    self:RefreshShowInfoData(userInfo)
end

function GameMainView:OnShow()
    PrintInfo("GameMainView:OnShow line 74")
    self.UIView:OnShow()
end

function GameMainView:OnOpen()

end

return GameMainView
