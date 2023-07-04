---@class PhysicalPowerUserPropsResponse
local PhysicalPowerUserPropsResponse = {}
local json = require("Common.json")
---comment
---@param nowPhysicalPower number
---@param residueTime number
---@param maximumStrength  number
---@param maximusResidueEndTime number
---@param residueNowTime number
function PhysicalPowerUserPropsResponse:new(nowPhysicalPower, residueTime, maximumStrength, maximusResidueEndTime,
                                            residueNowTime)
    local obj = {
        nowPhysicalPower = nowPhysicalPower,
        residueTime = residueTime,
        maximumStrength = maximumStrength,
        maximusResidueEndTime = maximusResidueEndTime,
        residueNowTime = residueNowTime
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end

function PhysicalPowerUserPropsResponse:protocolId()
    return 1026
end

function PhysicalPowerUserPropsResponse:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or PhysicalPowerUserPropsResponse

    local message = {
        protocolId = data:protocolId(),
        packet = data
    }
    local jsonStr = json.encode(message)
    buffer:writeString(jsonStr)
end

---  读取器 从字节流管理器中读取到对应字符
---@param buffer any
---@return table
function PhysicalPowerUserPropsResponse:read(buffer)
    local jsonString = buffer:readString()

    local data = json.decode(jsonString)
    local jsonData = PhysicalPowerUserPropsResponse:new(data.packet.nowPhysicalPower,
        data.packet.residueTime,
        data.packet.maximumStrength,
        data.packet.maximusResidueEndTime,
        data.packet.residueNowTime)
    return jsonData
end

return PhysicalPowerUserPropsResponse
