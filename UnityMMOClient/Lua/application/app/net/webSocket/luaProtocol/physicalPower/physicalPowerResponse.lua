---@class PhysicalPowerResponse
local PhysicalPowerResponse = class("PhysicalPowerResponse")
function PhysicalPowerResponse:ctor()
    --- 数据体力
    ---@type PhysicalPowerInfoData
    self.physicalPowerInfoData = {}
end

---@param physicalPowerInfoData PhysicalPowerInfoData 数据体力
---@return PhysicalPowerResponse
function PhysicalPowerResponse:new(physicalPowerInfoData)
    self.physicalPowerInfoData = physicalPowerInfoData --- com.gameServer.common.protocol.physicalPower.PhysicalPowerInfoData
    return self
end

---@return number
function PhysicalPowerResponse:protocolId()
    return 1024
end

---@return string
function PhysicalPowerResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            physicalPowerInfoData = self.physicalPowerInfoData
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return PhysicalPowerResponse
function PhysicalPowerResponse:read(data)
    local physicalPowerInfoDataPacket = PhysicalPowerInfoData()
    local physicalPowerInfoData = physicalPowerInfoDataPacket:read(data.physicalPowerInfoData)

    local packet = self:new(
            physicalPowerInfoData)
    return packet
end

--- 数据体力
---@return PhysicalPowerInfoData 数据体力
function PhysicalPowerResponse:getPhysicalPowerInfoData()
    return self.physicalPowerInfoData
end


return PhysicalPowerResponse
