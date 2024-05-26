---@class PhysicalPowerInfoData
local PhysicalPowerInfoData = class("PhysicalPowerInfoData")
function PhysicalPowerInfoData:ctor()
    --- 最大体力用于限制这个值会随着等级增长
    ---@type number
    self.maximumStrength = 0
    --- 我恢复到最大体力的结束时间
    ---@type number
    self.maximusResidueEndTime = 0
    --- 当前体力
    ---@type number
    self.nowPhysicalPower = 0
    --- 当前体力实时时间会跟着剩余时间一起变化
    ---@type number
    self.residueNowTime = 0
    --- 一点体力增长剩余时间
    ---@type number
    self.residueTime = 0
end

---@param maximumStrength number 最大体力用于限制这个值会随着等级增长
---@param maximusResidueEndTime number 我恢复到最大体力的结束时间
---@param nowPhysicalPower number 当前体力
---@param residueNowTime number 当前体力实时时间会跟着剩余时间一起变化
---@param residueTime number 一点体力增长剩余时间
---@return PhysicalPowerInfoData
function PhysicalPowerInfoData:new(maximumStrength, maximusResidueEndTime, nowPhysicalPower, residueNowTime, residueTime)
    self.maximumStrength = maximumStrength --- int
    self.maximusResidueEndTime = maximusResidueEndTime --- int
    self.nowPhysicalPower = nowPhysicalPower --- int
    self.residueNowTime = residueNowTime --- long
    self.residueTime = residueTime --- int
    return self
end

---@return number
function PhysicalPowerInfoData:protocolId()
    return 223
end

---@return string
function PhysicalPowerInfoData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            maximumStrength = self.maximumStrength,
            maximusResidueEndTime = self.maximusResidueEndTime,
            nowPhysicalPower = self.nowPhysicalPower,
            residueNowTime = self.residueNowTime,
            residueTime = self.residueTime
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return PhysicalPowerInfoData
function PhysicalPowerInfoData:read(data)

    local packet = self:new(
            data.maximumStrength,
            data.maximusResidueEndTime,
            data.nowPhysicalPower,
            data.residueNowTime,
            data.residueTime)
    return packet
end

--- 最大体力用于限制这个值会随着等级增长
---@return number 最大体力用于限制这个值会随着等级增长
function PhysicalPowerInfoData:getMaximumStrength()
    return self.maximumStrength
end
--- 我恢复到最大体力的结束时间
---@return number 我恢复到最大体力的结束时间
function PhysicalPowerInfoData:getMaximusResidueEndTime()
    return self.maximusResidueEndTime
end
--- 当前体力
---@return number 当前体力
function PhysicalPowerInfoData:getNowPhysicalPower()
    return self.nowPhysicalPower
end
--- 当前体力实时时间会跟着剩余时间一起变化
---@return number 当前体力实时时间会跟着剩余时间一起变化
function PhysicalPowerInfoData:getResidueNowTime()
    return self.residueNowTime
end
--- 一点体力增长剩余时间
---@return number 一点体力增长剩余时间
function PhysicalPowerInfoData:getResidueTime()
    return self.residueTime
end


return PhysicalPowerInfoData
