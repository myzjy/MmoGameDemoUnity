---@class LoginUIController:LuaUIObject
LoginUIController = class("LoginUIController", LuaUIObject)

function LoginUIController:GetInstance()
	return LoginUIController
end

---打开界面
function LoginUIController:Open()
	if self.LoginView == nil then
		if Debug > 0 then
			printError("LoginView 并未打开界面 生成")
		end
		-- 去生成界面
		LoginView:OnLoad()
		return
	end
	self:OnClose()
	self.LoginView.UIView:OnShow()
	self.RegisterPartView:OnHide()
	self.LoginTapToStartView:OnHide()
	self.LoginPartView:OnShow()
end

function LoginUIController:OnHide()
	if self.LoginView == nil then
		if Debug > 0 then
			printError("LoginView 并未打开界面 生成")
		end
		return
	end

	self.LoginPartView:OnHide()
	self.LoginTapToStartView:OnHide()
	self.RegisterPartView:OnHide()
end

function LoginUIController:OnClose()
	if self.LoginView == nil then
		if Debug > 0 then
			printError("LoginView 并未打开界面 生成")
		end
		return
	end
	if self.LoginView.reUse then
		self.LoginView:OnHide()
	else
		if Debug > 0 then
			printError("LoginView 并未打开界面 生成")
		end
	end
	self.LoginPartView:OnHide()
	self.LoginTapToStartView:OnHide()
	self.RegisterPartView:OnHide()
end

function LoginUIController:OpenLoginTapToStartUI()
	if Debug > 0 then
		printDebug("点击开始游戏之后，服务器在开启时间，可以正常进入")
	end
	--- 开始登录
	self.LoginTapToStartView:OnShow()

end

---comment
---@param LoginView LoginView|nil
---@param LoginPartView LoginPartView|nil
---@param RegisterPartView RegisterPartView|nil
---@param LoginTapToStartView LoginTapToStartView|nil
function LoginUIController:Build(LoginView, LoginPartView, RegisterPartView, LoginTapToStartView)
	---@type LoginView|nil
	self.LoginView = LoginView
	---@type LoginPartView|nil
	self.LoginPartView = LoginPartView
	---@type RegisterPartView|nil
	self.RegisterPartView = RegisterPartView
	---@type LoginTapToStartView|nil
	self.LoginTapToStartView = LoginTapToStartView

	self:SetListener(self.LoginPartView.LoginBtn, function()
		printDebug(
			"账号密码登录[account:"
			.. self.LoginPartView.account.text
			.. "][password:"
			.. self.LoginPartView.password.text
			.. "]"
		)
		local account = self.LoginPartView.account.text
		local password = self.LoginPartView.password.text
		GameEvent.LoginSuccess(account, password)

		--LoginService:LoginByAccount(account, password)
	end)
	self:SetListener(self.LoginPartView.RegisterBtn, function()
		self.LoginPartView:OnHide()
		self.RegisterPartView:OnShow()
	end)
end
