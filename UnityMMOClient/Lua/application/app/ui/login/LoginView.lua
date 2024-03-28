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
			PrintDebug("UILoginUIPanelView:Init()")
			LoginView:OnInit()
		end,
		showFunc = function()
			PrintDebug("UILoginUIPanelView:showUI()-->showFunc")
			LoginView:OnShow()
		end,
		hideFunc = function() end,
	}
	self:Load(self.UIConfig)
	self:LoadUI(self.UIConfig)
	self:InstanceOrReuse()
end

function LoginView:OnInit()
	PrintInfo("LoginView:OnInit line 10")

	---@type LoginPanelView
	local viewPanel = self.viewPanel
	---@type LoginPartView
	self.LoginPartView = loginPartView(viewPanel.LoginPart_UISerializableKeyObject)
	---@type RegisterPartView
	self.RegisterPartView = registerPartView(viewPanel.RegisterPart_UISerializableKeyObject)
	---@type LoginTapToStartView
	self.LoginTapToStartView = loginTapToStartView(viewPanel.LoginStart_UISerializableKeyObject)
	LoginUIController:GetInstance():Build(self, self.LoginPartView, self.RegisterPartView, self.LoginTapToStartView)
end

function LoginView:OnShow()
	PrintInfo("LoginView:OnShow line 21")
	LoginUIController:GetInstance():Open()
end

function LoginView:StartLoginTip()
	--coroutine.start()
end

return LoginView
