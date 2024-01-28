---@class GameMainView:UIView
GameMainView = class("GameMainView", UIView)
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
require("application.app.ui.generateScripts.UIModules.GameMainUIPanelView")
function GameMainView:OnLoad()
    self.UIConfig = {
        Config = GameMainConfig,
        viewPanel = GameMainUIPanelView,
        initFunc = function()
            printDebug("GameMainView:Init()")
            GameMainView:OnInit()
        end,
        showFunc = function()
            printDebug("GameMainView:showUI()-->showFunc")
            GameMainView:OnShow()
        end,
        hideFunc = function() end,
    }
    self:Load(self.UIConfig)
    self:LoadUI(self.UIConfig)
    self:InstanceOrReuse()
end

function GameMainView:OnInit()
    printDebug("call GameMainView Lua Script function to OnInit ....")
    ---@type GameMainUIPanelView
    local viewPanel = self.viewPanel

    self.GemsTimButton = viewPanel.GemsTim_UISerializableKeyObject:GetObjTypeStr("click") or CS.UnityEngine.UI.Button
    self.GemsText = viewPanel.GemsTim_UISerializableKeyObject:GetObjTypeStr("numText") or CS.UnityEngine.UI.Text
    --设置用户名
    self:SetTopHeadNameText(PlayerUserCaCheData:GetUseName())
    self.GemText = viewPanel.Gem_UISerializableKeyObject.GetObjTypeStr("numText") or CS.UnityEngine.UI.Text
    self.GoldCoinText = viewPanel.glod_UISerializableKeyObject.GetObjTypeStr("numText") or CS.UnityEngine.UI.Text
    self.GoldTimButton = viewPanel.glod_UISerializableKeyObject.GetObjTypeStr("click") or CS.UnityEngine.UI.Button
    self:OnShow()
end

---设置用户名
---@param userName string 用户名字
function GameMainView:SetTopHeadNameText(userName)
    printDebug("call GameMainView " ..
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
	printInfo("LoginView:OnShow line 21")
	LoginUIController:GetInstance():Open()
end

--- UI 通知事件
function GameMainView:Notification()
	---不能直接返回，直接返回在内部拿不到表
	local data = {
		[GameMainConfig.eventNotification.OPEN_GAMEMAINPANEL_UI] = GameMainConfig.eventNotification.OPEN_GAMEMAINPANEL_UI,
		[GameMainConfig.eventNotification.CLOSE_GAMEMAINPANEL_UI] = GameMainConfig.eventNotification.CLOSE_GAMEMAINPANEL_UI
	}
	return data
end

function GameMainView:NotificationHandler(_eventNotification)
	local eventSwitch = {
		[GameMainConfig.eventNotification.OPEN_GAMEMAINPANEL_UI] = function()
			if self.reUse then
				self:InstanceOrReuse()
			else
				self:OnLoad()
			end
		end,
		[GameMainConfig.eventNotification.CLOSE_GAMEMAINPANEL_UI] = function(obj)
			LoginView:OnHide()
		end
	}
	local switchAction = eventSwitch[_eventNotification.eventName]
	if eventSwitch then
		return switchAction(_eventNotification.eventBody)
	end
end
