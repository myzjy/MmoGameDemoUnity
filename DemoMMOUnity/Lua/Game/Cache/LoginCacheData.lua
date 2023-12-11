LoginChcheData = class("LoginChcheData")

---@param account string
function LoginChcheData:SetAccount(account)
	---@type string
	self.account = account
end

---@param password string
function LoginChcheData:SetPassword(password)
	---@type string
	self.password = password
end
