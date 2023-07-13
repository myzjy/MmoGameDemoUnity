---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/19 18:44
---
printDebug("加载 ProtocolManager.lua 文件")

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
    local byteBuffer = require("Game.Net.LuaProtocol.Buffer.ByteBuffer"):new()
    --- 把json字符串放入字节器中
    byteBuffer:writeString(jsonString)
    local protocol = ProtocolManager.getProtocol(protocolId)
    --- 返回对应的结构
    local protocolData = protocol:read(byteBuffer)
    return protocolData
end

function ProtocolManager.initProtocolManager()
    ---------------------------------Start of Service-------------------------------------

    require("Game.Net.Service.PhysicalPowerService")

    ---------------------------------End of Service-------------------------------------

    ---------------------------- error 101     ----------------------------
    local error = require("Game.Net.LuaProtocol.Common.Error")
    -----------------------------------------------------------------------

    ----------------------------  pong 104    ----------------------------
    local pong = require("Game.Net.LuaProtocol.Common.Pong")
    -----------------------------------------------------------------------
    ----------------------------  login   ----------------------------
    local loginRequest = require("Game.Net.LuaProtocol.Login.LoginRequest")
    local loginResponse = require("Game.Net.LuaProtocol.Login.LoginResponse")
    -----------------------------register------------------------------------------
    local registerRequest = require("Game.Net.LuaProtocol.Login.RegisterRequest")
    local registerResponse = require("Game.Net.LuaProtocol.Login.RegisterResponse")
    -----------------------------loginTapToStart------------------------------------------------------------
    local loginTapToStartRequest = require("Game.Net.LuaProtocol.Login.LoginTapToStartRequest")
    local loginTapToStartResponse = require("Game.Net.LuaProtocol.Login.LoginTapToStartResponse")
    ----------------------------------------------------------------------- --------------------------------
    -----------------------------serverConfigStart------------------------------------------------------------
    local serverConfigResponse = require("Game.Net.LuaProtocol.ServerConfig.ServerConfigResponse")
    --------------------------------------------------------------------------------------------------------

    -----------------------------physicalPowerUser------------------------------------------------------------
    local physicalPowerUserRequest = require("Game.Net.LuaProtocol.PhysicalPower.PhysicalPowerUsePropsRequest")
    local physicalPowerUserResponse = require("Game.Net.LuaProtocol.PhysicalPower.PhysicalPowerUserPropsResponse")
    --------------------------------------------------------------------------------------------------------

    -----------------------------physicalPowerSeconds------------------------------------------------------------
    local physicalPowerSecondsRequest = require("Game.Net.LuaProtocol.PhysicalPower.PhysicalPowerSecondsRequest")
    local PhysicalPowerSecondsResponse = require("Game.Net.LuaProtocol.PhysicalPower.PhysicalPowerSecondsResponse")
    --------------------------------------------------------------------------------------------------------

    -----------------------------PhysicalPower------------------------------------------------------------
    local PhysicalPowerRequest = require("Game.Net.LuaProtocol.PhysicalPower.PhysicalPowerRequest")
    local PhysicalPowerResponse = require("Game.Net.LuaProtocol.PhysicalPower.PhysicalPowerResponse")
    --------------------------------------------------------------------------------------------------------

    protocols[101] = error
    protocols[104] = pong
    protocols[1000] = loginRequest
    protocols[1001] = loginResponse
    protocols[1005] = registerRequest
    protocols[1006] = registerResponse
    protocols[1010] = serverConfigResponse
    protocols[1013] = loginTapToStartRequest
    protocols[loginTapToStartResponse:protocolId()] = loginTapToStartResponse

    protocols[1023] = PhysicalPowerRequest
    protocols[1024] = PhysicalPowerResponse

    protocols[1025] = physicalPowerUserRequest
    protocols[1026] = physicalPowerUserResponse

    protocols[1029] = physicalPowerSecondsRequest
    protocols[1030] = PhysicalPowerSecondsResponse
end

ProtocolConfig = {
    ---@type {id:number,protocolData:fun(id:number):PhysicalPowerUsePropsRequest}
    PhysicalPowerUserPropsRequest = { id = 1025, protocolData = function(id)
        return ProtocolManager.getProtocol(id)
    end },
    ---@type {id:number,protocolData:fun(id:number):PhysicalPowerSecondsRequest}
    PhysicalPowerSecondsRequest = { id = 1029, protocolData = function(id)
        return ProtocolManager.getProtocol(id)
    end }
}

return ProtocolManager
