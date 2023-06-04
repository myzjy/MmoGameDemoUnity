--- 网络包注册
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/22 13:10

printDebug("加载 PacketDispatcher.lua 文件")
local PacketDispatcher = {}
--_receiversMap = {}
PacketDispatcher.Event = {
    OnConnect = "PacketDispatcher.Event.OnConnect",
    OnDisConnect = "PacketDispatcher.Event.OnDisConnect",
}
packetValue = nil
PacketDispatcher.urlString = nil
--- 链接建立通知lua
function OnConnectServer(url)
    PacketDispatcher.urlString = url
    ---链接成功
    GlobalEventSystem:Fire(PacketDispatcher.Event.OnConnect, PacketDispatcher.urlString)
end

function OnDisConnectFromServer()
    printDebug("Game server disconnected!!")
    GlobalEventSystem:Fire(PacketDispatcher.Event.OnDisConnect)
end
function OnReceiveLineFromServer(bytes)
    printDebug(type(bytes))
    str = bytes
    local packet = readBytes(str)
    --packetValue = packet
    PacketDispatcher:Receive(packet)
end

function PacketDispatcher:SendMessage(bytes)
    global.netManager:SendMessage(bytes)
end
function PacketDispatcher:Init()
    local loginNetController = require("Game.UI.Login.LoginNetController"):New()
    loginNetController.Init()

end

--function PacketDispatcher:Receive(str)
--    printDebug("PacketDispatcher:Receive(packet) line 50," .. type(packet))
--    PacketDispatcher.packetValue = packet
--    GlobalEventValue:Fire(PacketDispatcher.packetValue:protocolId(), PacketDispatcher.packetValue)
--end

function PacketDispatcher:Receive(packet)
    --packetValue = packet
    GlobalEventSystem:Fire(packet:protocolId(), packet)
end

return PacketDispatcher