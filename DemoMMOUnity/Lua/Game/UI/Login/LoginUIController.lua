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
	if self.LoginView.reUse then
		--- 已经打开界面
		self.LoginView:InstanceOrReuse()
	else
		---界面没打开 没生成过
		self.LoginView:OnLoad()
	end
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
end

function LoginUIController:Build(LoginView, LoginPartView, RegisterPartView)
	---@type LoginView|nil
	self.LoginView = LoginView
	---@type LoginPartView|nil
	self.LoginPartView = LoginPartView
	---@type RegisterPartView|nil
	self.RegisterPartView = RegisterPartView
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
		LoginService:LoginByAccount(account, password)
	end)
	self:SetListener(self.LoginPartView.RegisterBtn, function()
		self.LoginPartView:OnHide()
		self.RegisterPartView:OnShow()
	end)
end
