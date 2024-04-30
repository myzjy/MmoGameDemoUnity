---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/19 18:44
---
print("加载 ProtocolManager.lua 文件")

print("require service files lua script start")

---------------------------- error 101     ----------------------------
Error = require("application.app.net.webscoket.luaProtocol.common.error")
-----------------------------------------------------------------------

----------------------------  pong 104    ----------------------------
Pong = require("application.app.net.webscoket.luaProtocol.common.pong")
-----------------------------------------------------------------------
----------------------------  login   ----------------------------
LoginRequest = require("application.app.net.webscoket.luaProtocol.login.loginRequest")
LoginResponse = require("application.app.net.webscoket.luaProtocol.login.loginResponse")
LoginUserServerInfoData = require("application.app.net.webscoket.luaProtocol.login.loginUserServerInfoData")

-----------------------------------------------------------------------------------

----------------------------------reigister-------------------------------------------------
RegisterRequest = require("application.app.net.webscoket.luaProtocol.register.registerRequest")
RegisterResponse = require("application.app.net.webscoket.luaProtocol.register.registerResponse")
-----------------------------------------------------------------------------------------------------------

----------------------------------loginTapToStart-------------------------------------------------
LoginTapToStartRequest = require("application.app.net.webscoket.luaProtocol.loginTapToStart.loginTapToStartRequest")
LoginTapToStartResponse = require("application.app.net.webscoket.luaProtocol.loginTapToStart.loginTapToStartResponse")
-----------------------------------------------------------------------------------------------------------

----------------------------------serverConfig-------------------------------------------------
ServerConfigRequest = require("application.app.net.webscoket.luaProtocol.serverConfig.serverConfigRequest")
ServerConfigResponse = require("application.app.net.webscoket.luaProtocol.serverConfig.serverConfigResponse")
-----------------------------------------------------------------------------------------------------------

----------------------------------bag-------------------------------------------------
ItemBaseData = require("application.app.net.webscoket.luaProtocol.bag.itemBaseData")
AllBagItemRequest = require("application.app.net.webscoket.luaProtocol.bag.allBagItemRequest")
AllBagItemResponse = require("application.app.net.webscoket.luaProtocol.bag.allBagItemResponse")
BagUserItemMsgData = require("application.app.net.webscoket.luaProtocol.bag.bagUserItemMsgData")
-----------------------------------------------------------------------------------------------------------

----------------------------------equipment-------------------------------------------------
EquipmentBaseData = require("application.app.net.webscoket.luaProtocol.euipment.equipmentBaseData")
EquipmentConfigBaseData = require("application.app.net.webscoket.luaProtocol.euipment.equipmentConfigBaseData")
EquipmentDesBaseData = require("application.app.net.webscoket.luaProtocol.euipment.equipmentDesBaseData")
EquipmentGrowthConfigBaseData = require(
    "application.app.net.webscoket.luaProtocol.euipment.equipmentGrowthConfigBaseData")
EquipmentGrowthViceConfigBaseData = require(
    "application.app.net.webscoket.luaProtocol.euipment.equipmentGrowthViceConfigBaseData")
EquipmentPrimaryConfigBaseData = require(
    "application.app.net.webscoket.luaProtocol.euipment.equipmentPrimaryConfigBaseData")
-----------------------------------------------------------------------------------------------------------

--------------------------------------------Weapon------------------------------------------------------------
WeaponPlayerUserDataRequest = require("application.app.net.webscoket.luaProtocol.weapon.weaponPlayerUserDataRequest")
WeaponPlayerUserDataResponse = require("application.app.net.webscoket.luaProtocol.weapon.weaponPlayerUserDataResponse")
WeaponPlayerUserDataStruct = require("application.app.net.webscoket.luaProtocol.weapon.weaponPlayerUserDataStruct")
WeaponsConfigData = require("application.app.net.webscoket.luaProtocol.weapon.weaponsConfigData")
--------------------------------------------end Weapon------------------------------------------------------------

--------------------------------------------playerUser-----------------------------------------------------------

PlayerSceneInfoData = require("application.app.net.webscoket.luaProtocol.playerUser.playerSceneInfoData")
UserMsgInfoData = require("application.app.net.webscoket.luaProtocol.playerUser.userMsgInfoData")

------------------------------------------end playerUser----------------------------------------------------------

--------------------------------------------PhysicalPower-----------------------------------------------------------

PhysicalPowerRequest = require("application.app.net.webscoket.luaProtocol.physicalPower.physicalPowerRequest")
PhysicalPowerResponse = require("application.app.net.webscoket.luaProtocol.physicalPower.physicalPowerResponse")
PhysicalPowerInfoData = require("application.app.net.webscoket.luaProtocol.physicalPower.physicalPowerInfoData")

--------------------------------------------end PhysicalPower-------------------------------------------------------

------------------------------------------gameMain--------------------------------------------------------------------

GameMainUIPanelRequest = require("application.app.net.webscoket.luaProtocol.gameMain.gameMainUIPanelRequest")
GameMainUserResourcesResponse = require "application.app.net.webscoket.luaProtocol.gameMain.gameMainUserResourcesResponse"

------------------------------------------end gameMain--------------------------------------------------------------------


-- local protocols = {}

local ProtocolManager = {}


-- table扩展方法，map的大小
function table.mapSize(map)
    local size = 0
    for _, _ in pairs(map) do
        size = size + 1
    end
    return size
end

-- ---@param protocolId any
-- ---@return any
-- function ProtocolManager.getProtocol(protocolId)
--     local protocol = protocols[protocolId]
--     if protocol == nil then
--         PrintError("[protocolId:" .. protocolId .. "]协议不存在")
--     end
--     return protocol
-- end

-- function ProtocolManager.write(buffer, packet)
--     local protocolId = packet:protocolId()
--     -- 写入包体
--     ProtocolManager.getProtocol(protocolId):write(buffer, packet)
-- end

-- ---读取
-- ---@param buffer ByteBuffer 字节读取器
-- function ProtocolManager:read(buffer)
--     local jsonString = buffer:readString()
--     local jsonData = JSON.decode(jsonString);
--     ---获取对应id
--     local protocolId = jsonData.protocolId
--     local protocol = protocols[protocolId]
--     if protocol == NULL then
--         return nil
--     end
--     local packetData = protocol();
--     --- 返回对应的结构
--     packetData = packetData:read(jsonData)
--     return packetData
-- end

function ProtocolManager.initProtocolManager()
    -- protocols[101] = Error
    -- protocols[104] = Pong

    -- protocols[1000] = LoginRequest
    -- protocols[1001] = LoginResponse

    -- protocols[RegisterRequest:protocolId()] = RegisterRequest
    -- protocols[RegisterResponse:protocolId()] = RegisterResponse

    -- protocols[LoginTapToStartRequest:protocolId()] = LoginTapToStartRequest
    -- protocols[LoginTapToStartResponse:protocolId()] = LoginTapToStartResponse

    -- protocols[ServerConfigRequest:protocolId()] = ServerConfigRequest
    -- protocols[ServerConfigResponse:protocolId()] = ServerConfigResponse

    -- protocols[ItemBaseData:protocolId()] = ItemBaseData

    -- protocols[EquipmentBaseData:protocolId()] = EquipmentBaseData
    -- protocols[EquipmentConfigBaseData:protocolId()] = EquipmentConfigBaseData
    -- protocols[EquipmentDesBaseData:protocolId()] = EquipmentDesBaseData
    -- protocols[EquipmentGrowthConfigBaseData:protocolId()] = EquipmentGrowthConfigBaseData
    -- protocols[EquipmentGrowthViceConfigBaseData:protocolId()] = EquipmentGrowthViceConfigBaseData
    -- protocols[EquipmentPrimaryConfigBaseData:protocolId()] = EquipmentPrimaryConfigBaseData

    -- protocols[WeaponsConfigData:protocolId()] = WeaponsConfigData
end

return ProtocolManager
