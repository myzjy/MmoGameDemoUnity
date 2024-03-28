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
    local str = bytes
    -- local buffer = ByteBuffer:new()
    -- buffer:writeString(bytes)
    local jsonData = JSON.decode(bytes);
    ---获取对应id
    local protocolId = jsonData.protocolId

    -- local packet = readBytes(str)
    PacketDispatcher:ReceiveMsg(protocolId, jsonData.packet)
end

function PacketDispatcher:SendMessage(bytes)
    -- NetManager:SendMessageEvent(bytes)
end

function PacketDispatcher:Init()
    ------------------------------------Inti event list-----------------------------------------

    self.msgMap = {}
    self.msgMap[RegisterResponse:protocolId()] = handle(GameEvent.RegisterResonse, self)
    self.msgMap[LoginResponse:protocolId()] = function(data)
        GameEvent.LoginResonse(data)
    end
    self.msgMap[LoginTapToStartResponse:protocolId()] = handle(GameEvent.LoginTapToStartResponse, self)
    self.msgMap[LoginConst.Event.Pong] = function(data)
        LoginNetController:AtPong(data)
    end
    self.msgMap[WeaponPlayerUserDataRequest:protocolId()] = handle(GameEvent.AcquireUserIdWeaponService, self)
    self.msgMap[AllBagItemResponse:protocolId()] = handle(GameEvent.AtBagHeaderWeaponBtnService, self)

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

return PacketDispatcher
