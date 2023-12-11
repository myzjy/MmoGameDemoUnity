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
	LoginChcheData:SetAccount(account)
	LoginChcheData:SetPassword(password)
	local id = LoginServiceConfig().LoginRequest.id
	local packetData = LoginServiceConfig().LoginRequest.protocolData(id)
	if packetData == nil then
		printError("当前 LoginReuqest 脚本 没有读取到 请检查")
		return
	end
	local pakcet = packetData:new(account, password)
	local buffer = ByteBuffer:new()
	ProtocolManager.write(buffer, pakcet)
	global.netManager:SendMessage(buffer:readByte())
end

function LoginService:LoginTapToStart()
	local platform = "ediotr"
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
	global.netManager:SendMessage(buffer:readByte())
end

return LoginService
