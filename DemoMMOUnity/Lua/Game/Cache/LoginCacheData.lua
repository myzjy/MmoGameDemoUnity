---@class LoginCacheData
LoginCacheData = class("LoginCacheData")

---@param account string
function LoginCacheData:SetAccount(account)
	---@type string
	self.account = account
end

---@param password string
function LoginCacheData:SetPassword(password)
	---@type string
	self.password = password
end
