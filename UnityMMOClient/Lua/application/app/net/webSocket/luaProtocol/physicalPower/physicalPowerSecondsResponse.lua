---@class PhysicalPowerSecondsResponse
local PhysicalPowerSecondsResponse = class("PhysicalPowerSecondsResponse")
function PhysicalPowerSecondsResponse:ctor()
    --- 数据体力
    ---@type PhysicalPowerInfoData
    self.physicalPowerInfoData = {}
end

---@param physicalPowerInfoData PhysicalPowerInfoData 数据体力
---@return PhysicalPowerSecondsResponse
function PhysicalPowerSecondsResponse:new(physicalPowerInfoData)
    self.physicalPowerInfoData = physicalPowerInfoData --- com.gameServer.common.protocol.physicalPower.PhysicalPowerInfoData
    return self
end

---@return number
function PhysicalPowerSecondsResponse:protocolId()
    return 1030
end

---@return string
function PhysicalPowerSecondsResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            physicalPowerInfoData = self.physicalPowerInfoData
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return PhysicalPowerSecondsResponse
function PhysicalPowerSecondsResponse:read(data)
    local physicalPowerInfoDataPacket = PhysicalPowerInfoData()
    local physicalPowerInfoData = physicalPowerInfoDataPacket:read(data.physicalPowerInfoData)

    local packet = self:new(
            physicalPowerInfoData)
    return packet
end

--- 数据体力
---@return PhysicalPowerInfoData 数据体力
function PhysicalPowerSecondsResponse:getPhysicalPowerInfoData()
    return self.physicalPowerInfoData
end


return PhysicalPowerSecondsResponse
