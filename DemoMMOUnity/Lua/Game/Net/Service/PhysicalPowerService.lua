---@class PhysicalPowerService
PhysicalPowerService = class("PhysicalPowerService")

function PhysicalPowerService:SendPhysicalPower()
    local protocol = ProtocolManager.getProtocol(1023)
    if protocol == nil then
        printError("协议1023不存在,lua侧没有当前协议结构体,请检查c#侧")
        return
    end
    local protocolData = protocol:new(PlayerUserCaCheData:GetUID())
    local buffer = ByteBuffer:new()
    protocol:write(buffer, protocolData)
    local str = buffer:readString()
    PacketDispatcher:SendMessage(str)
end

---扣除体力
---@param num number 使用体力
function PhysicalPowerService:SubPhysicalPowerService(num)
    local ProtocolData = ProtocolConfig.PhysicalPowerUserPropsRequest.protocolData
    if ProtocolData == nil then
        printError("协议" .. ProtocolConfig.PhysicalPowerUserPropsRequest.id .. "不存在,lua侧没有当前协议结构体,请检查c#侧")
        return
    end

    local protocolData = ProtocolData:new(num)

    local buffer = ByteBuffer:new()

    ProtocolData:write(buffer, protocolData)
    local str = buffer:readString()
    PacketDispatcher:SendMessage(str)
end
--- 恢复体力
function PhysicalPowerService:SendPhysicalPowerSecondsRequest()
    local protocolId = ProtocolManager.getProtocol(ProtocolConfig.PhysicalPowerSecondsRequest.id)
    local protocolData=protocolId:new(timestamp)
    printDebug("json: data.nowTime ：" .. protocolData.nowTime)
    local buffer = ByteBuffer:new()
    ProtocolManager.write(buffer, protocolData)
    local str = buffer:readString()
    PacketDispatcher:SendMessage(str)
end
