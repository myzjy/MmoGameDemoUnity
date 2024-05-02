--- 网络包注册
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/22 13:10

PrintDebug("加载 PacketDispatcher.lua 文件")
local PacketDispatcher = {}
--_receiversMap = {}
PacketDispatcher.Event = {
    OnConnect = "PacketDispatcher.Event.OnConnect",
    OnOpen = "PacketDispatcher.Event.OnOpen",
    OnDisConnect = "PacketDispatcher.Event.OnDisConnect"
}




-------------------------------- start Login   pack 包 --------------------------------------
LoginNetController = require("application.app.net.webscoket.controller.loginNetController").GetInstance()
ServerConfigNetController = require("application.app.net.webscoket.controller.serverConfigNetController").GetInstance()
WeaponNetController = require("application.app.net.webscoket.controller.weaponNetController"):GetInstance()
BagNetController = require("application.app.net.webscoket.controller.bagNetController"):GetInstance()
GameMainNetController = require("application.app.net.webscoket.controller.gameMainNetController"):GetInstance()
-------------------------------- end   Login    pack 包 --------------------------------------

PacketDispatcher.urlString = nil
--- 链接建立通知lua
function OnConnectServer(url)
    PacketDispatcher.urlString = url
    ---链接成功
    GlobalEventSystem:Fire(PacketDispatcher.Event.OnConnect, PacketDispatcher.urlString)
end

function OnDisConnectFromServer()
    PrintDebug("Game server disconnected!!")
    GlobalEventSystem:Fire(PacketDispatcher.Event.OnDisConnect)
end

function OnReceiveLineFromServer(bytes)
    local jsonData = JSON.decode(bytes);
    ---获取对应id
    local protocolId = jsonData.protocolId
    PacketDispatcher:ReceiveMsg(protocolId, jsonData.packet)
end

function PacketDispatcher:SendMessage(bytes)
    -- NetManager:SendMessageEvent(bytes)
end

function PacketDispatcher:Init()
    ------------------------------------Inti event list-----------------------------------------

    self.msgMap = {}
    self.msgMap[RegisterResponse:protocolId()] = handle(self.OnRegisterResponse, self)
    self.msgMap[LoginResponse:protocolId()] = handle(self.OnLoginResponse, self)
    self.msgMap[LoginTapToStartResponse:protocolId()] = handle(self.OnLoginTapToStartResponse, self)
    self.msgMap[LoginConst.Event.Pong] = function(data)
        LoginNetController:AtPong(data)
    end
    self.msgMap[WeaponPlayerUserDataResponse:protocolId()] = handle(self.OnWeaponPlayerUserDataResponse, self)
    self.msgMap[AllBagItemResponse:protocolId()] = handle(self.OnAllBagItemResponse, self)
    self.msgMap[PhysicalPowerResponse:protocolId()] = handle(self.OnPhysicalPowerResponse, self)
    self.msgMap[GameMainUserResourcesResponse:protocolId()] = handle(self.OnGameMainUserResourcesResponse, self)

    -------------------------------- start Login   pack 包 --------------------------------------

    LoginNetController:Init()
    -------------------------------- end   Login    pack 包 --------------------------------------

    -------------------------------- start ServerConfig   pack 包 --------------------------------------
    ServerConfigNetController:RegisterEvent()
    -------------------------------- end   ServerConfig    pack 包 --------------------------------------

    -------------------------------- start weapon   pack 包 --------------------------------------
    WeaponNetController:RegisterEvent()
    -------------------------------- end   weapon    pack 包 --------------------------------------

    -------------------------------- start bag   pack 包 --------------------------------------
    BagNetController:RegisterEvent()
    -------------------------------- end   bag    pack 包 --------------------------------------
end

function PacketDispatcher:OnLoginResponse(data)
    local response = LoginResponse();
    local pakcet = response:read(data)
    GameEvent.LoginResonse(pakcet)
end

function PacketDispatcher:OnRegisterResponse(data)
    local response = RegisterResponse()
    local pakcet = response:read(data)
    GameEvent.RegisterResonse(pakcet)
end

function PacketDispatcher:OnLoginTapToStartResponse(data)
    local response = LoginTapToStartResponse()
    local pakcet = response:read(data)
    GameEvent.LoginTapToStartResponse(pakcet)
end

function PacketDispatcher:OnWeaponPlayerUserDataResponse(data)
    local response = WeaponPlayerUserDataResponse()
    local pakcet = response:read(data)
    GameEvent.AcquireUserIdWeaponService(pakcet)
end

function PacketDispatcher:OnAllBagItemResponse(data)
    local response = AllBagItemResponse()
    local pakcet = response:read(data)
    GameEvent.AtBagHeaderBtnService(pakcet)
end

function PacketDispatcher:OnPhysicalPowerResponse(data)
    ---@type PhysicalPowerResponse
    local response = PhysicalPowerResponse()
    local packet = response:read(data)
    ServerConfigNetController:SetPhysicalPowerInfoData(packet:getPhysicalPowerInfoData())
    GameEvent.UpdateGamePhysicalInfo(packet)
end

function PacketDispatcher:AddProtocolConfigEvent(id, method)
    self.msgMap[id] = method
end

function PacketDispatcher:Receive(packet)
    self.msgMap[packet:protocolId()](packet)
    --packetValue = packet
    -- ProtocolManager:FireProtocolConfigEvent(packet:protocolId(), packet)
end

function PacketDispatcher:ReceiveMsg(key, packet)
    self.msgMap[key](packet)
end

function PacketDispatcher:OnGameMainUserResourcesResponse(packetData)
    local data = GameMainUserResourcesResponse()
    local packet = data:read(packetData)
    ServerConfigNetController:SetUserMsgInfoData(packet.userMsgInfoData)
    GameEvent.UPdateGameMainUserInfoMsg(packet.userMsgInfoData)
end

return PacketDispatcher
