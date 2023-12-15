---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2023/5/13 16:17
---

---@class LoginView:UIView
LoginView = class("LoginView", UIView)
require("Game.GenerateScripts.UIModules.LoginPanelView")
LoginView.LoginPartView = require("Game.UI.Login.LoginPartView")
LoginView.RegisterPartView = require("Game.UI.Login.RegisterPartView")
LoginView.LoginTapToStartView = require("Game.UI.Login.LoginTapToStartView")
function LoginView:OnLoad()
	self.UIConfig = {
		Config = LoginConfig,
		viewPanel = LoginPanelView,
		initFunc = function()
			printDebug("UILoginUIPanelView:Init()")
			LoginView:OnInit()
		end,
		showFunc = function()
			printDebug("UILoginUIPanelView:showUI()-->showFunc")
			LoginView:OnShow()
		end,
		hideFunc = function() end,
	}
	self:Load(self.UIConfig)
	self:LoadUI(self.UIConfig)
	self:InstanceOrReuse()
end

function LoginView:OnInit()
	printInfo("LoginView:OnInit line 10")

	---@type LoginPanelView
	local viewPanel = LoginView.viewPanel
	self.LoginPartView:Build(viewPanel.LoginPart_UISerializableKeyObject)
	self.RegisterPartView:Build(viewPanel.RegisterPart_UISerializableKeyObject)
    self.LoginTapToStartView:Build(viewPanel.LoginStart_UISerializableKeyObject)
	LoginView.viewPanel.RegisterPartView:Build()
	LoginView.viewPanel.LoginTapToStartView:Build(nil)
	LoginUIController:GetInstance():Build(self, self.LoginPartView, self.RegisterPartView)
end

function LoginView:OnShow()
	printInfo("LoginView:OnShow line 21")
end


function LoginView:StartLoginTip()
	--coroutine.start()
end

--- UI 通知事件
function LoginView:Notification()
	---不能直接返回，直接返回在内部拿不到表
	local data = {
		[LoginConfig.eventNotification.OPEN_LOGIN_INIT_PANEL] = LoginConfig.eventNotification.OPEN_LOGIN_INIT_PANEL,
		[LoginConfig.eventNotification.CLOSE_LOGIN_INIT_PANEL] = LoginConfig.eventNotification.CLOSE_LOGIN_INIT_PANEL,
		[LoginConfig.eventNotification.OpenLoginTapToStartUI] = LoginConfig.eventNotification.OpenLoginTapToStartUI,
		[LoginConfig.eventNotification.ShowLoginAccountUI] = LoginConfig.eventNotification.ShowLoginAccountUI,
	}
	return data
end

function LoginView:NotificationHandler(_eventNotification)
	local eventSwitch = {
		[LoginConfig.eventNotification.OPEN_LOGIN_INIT_PANEL] = function()
			if self.reUse then
				self:InstanceOrReuse()
			else
				self:OnLoad()
			end
		end,
		[LoginConfig.eventNotification.CLOSE_LOGIN_INIT_PANEL] = function(obj)
			LoginView:OnHide()
		end,
		[LoginConfig.eventNotification.OpenLoginTapToStartUI] = function(obj)
			if Debug > 0 then
				printDebug("点击开始游戏之后，服务器在开启时间，可以正常进入")
			end
		end,
		[LoginConfig.eventNotification.ShowLoginAccountUI] = function(obj)
			LoginView:OnHide()
		end,
	}
	local switchAction = eventSwitch[_eventNotification.eventName]
	if eventSwitch then
		return switchAction(_eventNotification.eventBody)
	end
end
