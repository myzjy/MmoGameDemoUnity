--- 登录 界面 loginService
---

---@class LoginService
local LoginService = class("LoginService")
local function LoginServiceConfig()
	return {
		---@type {id:number,protocolData:fun(id:number):LoginRequest|nil}
		LoginRequest = {
			id = 1000,
			protocolData = function(id)
				return ProtocolManager.getProtocol(id)
			end,
		},
		---@type{id:number,protocolData:fun(id:number):LoginTapToStartRequest|nil}
		LoginTapToStartRequest = {
			id = 1013,
			protocolData = function(id)
				return ProtocolManager.getProtocol(id)
			end,
		},
	}
end
---@param account string
---@param password string
function LoginService:LoginByAccount(account, password)
	LoginCacheData:SetAccount(account)
	LoginCacheData:SetPassword(password)
	local id = LoginServiceConfig().LoginRequest.id
	local packetData = LoginServiceConfig().LoginRequest.protocolData(id)
	if packetData == nil then
		printError("当前 LoginRequest 脚本 没有读取到 请检查")
		return
	end
	local packet = packetData:new(account, password)
	local buffer = ByteBuffer:new()
	ProtocolManager.write(buffer, packet)
	global.netManager:SendMessage(buffer:readString())
end

function LoginService:LoginTapToStart()
	local platform = "editor"
	if CS.UnityEngine.Application.platform == CS.UnityEngine.RuntimePlatform.Android then
		platform = "android"
	elseif CS.UnityEngine.Application.platform == CS.UnityEngine.RuntimePlatform.IPhonePlayer then
		platform = "ios"
	elseif CS.UnityEngine.Application.platform == CS.UnityEngine.RuntimePlatform.WindowsPlayer then
		platform = "pc"
	else
		printDebug("Current platform is unknown.")
	end
	local id = LoginServiceConfig().LoginTapToStartRequest.id
	local packetData = LoginServiceConfig().LoginTapToStartRequest.protocolData(id)
	if packetData == nil then
		printError("当前LoginTapToStartRequest lua 侧没有读取到 检查文件")
		return
	end
	local packet = packetData:new(platform)
	local buffer = ByteBuffer:new()
	packetData:write(buffer, packet)
	global.netManager:SendMessage(buffer:readString())
end

---@param account string
---@param password string
---@param affirmPassword string
function LoginService:RegisterAccount(account, password, affirmPassword)
	local id = ProtocolConfig.RegisterRequest.id
	local packetData = ProtocolConfig.RegisterRequest.protocolData(id)
	if packetData == nil then
		printError("当前 RegisterRequest lua 侧没有读取到 检查文件")
		return
	end
	local packet = packetData:new(account, password, affirmPassword)
	local buffer = ByteBuffer:new()
	packetData:write(buffer, packet)
	global.netManager:SendMessage(buffer:readString())
end

return LoginService
