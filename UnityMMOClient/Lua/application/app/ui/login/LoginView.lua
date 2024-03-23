---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2023/5/13 16:17
---

---@class LoginView:UIView
local LoginView = class("LoginView", UIView)
local loginPartView = require("application.app.ui.login.LoginPartView")
local registerPartView = require("application.app.ui.login.RegisterPartView")
local loginTapToStartView = require("application.app.ui.login.LoginTapToStartView")
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
	local viewPanel = self.viewPanel
	self.LoginPartView = loginPartView(viewPanel.LoginPart_UISerializableKeyObject)
	self.RegisterPartView = registerPartView(viewPanel.RegisterPart_UISerializableKeyObject)
	self.LoginTapToStartView = loginTapToStartView(viewPanel.LoginStart_UISerializableKeyObject)
	LoginUIController:GetInstance():Build(self, self.LoginPartView, self.RegisterPartView, self.LoginTapToStartView)
end

function LoginView:OnShow()
	printInfo("LoginView:OnShow line 21")
	LoginUIController:GetInstance():Open()
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

return LoginView
