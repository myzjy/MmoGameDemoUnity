---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/19 18:44
---
print("加载 ProtocolManager.lua 文件")

print("require service files lua script start")

LoginService = require("application.app.net.webscoket.service.loginService")
---------------------------- error 101     ----------------------------
Error = require("application.app.net.webscoket.luaProtocol.common.error")
-----------------------------------------------------------------------

----------------------------  pong 104    ----------------------------
Pong = require("application.app.net.webscoket.luaProtocol.common.pong")
-----------------------------------------------------------------------
----------------------------  login   ----------------------------
LoginRequest = require("application.app.net.webscoket.luaProtocol.login.loginRequest")
LoginResponse = require("application.app.net.webscoket.luaProtocol.login.loginResponse")
-----------------------------------------------------------------------------------

----------------------------------reigister-------------------------------------------------
RegisterRequest = require("application.app.net.webscoket.luaProtocol.register.registerRequest")
RegisterResponse = require("application.app.net.webscoket.luaProtocol.register.registerResponse")
-----------------------------------------------------------------------------------------------------------

----------------------------------loginTapToStart-------------------------------------------------
LoginTapToStartRequest = require("application.app.net.webscoket.luaProtocol.loginTapToStart.loginTapToStartRequest")
LoginTapToStartResponse = require("application.app.net.webscoket.luaProtocol.loginTapToStart.loginTapToStartResponse")
-----------------------------------------------------------------------------------------------------------


local protocols = {}

local ProtocolManager = {}


-- table扩展方法，map的大小
function table.mapSize(map)
    local size = 0
    for _, _ in pairs(map) do
        size = size + 1
    end
    return size
end

function ProtocolManager.getProtocol(protocolId)
    local protocol = protocols[protocolId]
    if protocol == nil then
        printError("[protocolId:" .. protocolId .. "]协议不存在")
        return nil
    end
    return protocol
end

function ProtocolManager.write(buffer, packet)
    local protocolId = packet:protocolId()
    -- 写入包体
    ProtocolManager.getProtocol(protocolId):write(buffer, packet)
end

---读取
---@param buffer ByteBuffer 字节读取器
function ProtocolManager.read(buffer)
    local jsonString = buffer:readString()
    local jsonData = JSON.decode(jsonString);
    ---获取对应id
    local protocolId = jsonData.protocolId
    local byteBuffer = require("application.app.net.webscoket.luaProtocol.buffer.byteBuffer"):new()
    --- 把json字符串放入字节器中
    byteBuffer:writeString(jsonString)
    local protocol = ProtocolManager.getProtocol(protocolId)
    --- 返回对应的结构
    local protocolData = protocol:read(byteBuffer)
    return protocolData
end

function ProtocolManager.initProtocolManager()
    protocols[101] = Error
    protocols[104] = Pong
    protocols[1000] = LoginRequest
    protocols[1001] = LoginResponse

    protocols[LoginTapToStartRequest:protocolId()] = LoginTapToStartRequest
    protocols[LoginTapToStartResponse:protocolId()] = LoginTapToStartResponse

    protocols[RegisterRequest:protocolId()] = RegisterRequest
    protocols[RegisterResponse:protocolId()] = RegisterResponse
end

ProtocolManager.ProtocolConfigEvent = {}

function ProtocolManager:AddProtocolConfigEvent(id, func)
    self.ProtocolConfigEvent[id] = func
end

function ProtocolManager:FireProtocolConfigEvent(id, ...)
    local eventList = ...
    self.ProtocolConfigEvent[id](eventList)
end

return ProtocolManager
