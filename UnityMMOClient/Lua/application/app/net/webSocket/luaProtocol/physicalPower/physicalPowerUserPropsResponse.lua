---@class PhysicalPowerUserPropsResponse
local PhysicalPowerUserPropsResponse = class("PhysicalPowerUserPropsResponse")
function PhysicalPowerUserPropsResponse:ctor()
    --- 最大体力 用于限制 这个值会随着 等级增长
    ---@type number
    self.maximumStrength = 0
    --- 我恢复到最大体力的结束时间
    ---@type number
    self.maximusResidueEndTime = 0
    --- 返回 使用体力 所扣除 当前体力
    ---@type number
    self.nowPhysicalPower = 0
    --- 当前体力实时时间 会跟着剩余时间一起变化
    ---@type number
    self.residueNowTime = 0
    --- 一点体力增长剩余时间
    ---@type number
    self.residueTime = 0
end

---@param maximumStrength number 最大体力 用于限制 这个值会随着 等级增长
---@param maximusResidueEndTime number 我恢复到最大体力的结束时间
---@param nowPhysicalPower number 返回 使用体力 所扣除 当前体力
---@param residueNowTime number 当前体力实时时间 会跟着剩余时间一起变化
---@param residueTime number 一点体力增长剩余时间
---@return PhysicalPowerUserPropsResponse
function PhysicalPowerUserPropsResponse:new(maximumStrength, maximusResidueEndTime, nowPhysicalPower, residueNowTime, residueTime)
    self.maximumStrength = maximumStrength --- int
    self.maximusResidueEndTime = maximusResidueEndTime --- int
    self.nowPhysicalPower = nowPhysicalPower --- int
    self.residueNowTime = residueNowTime --- long
    self.residueTime = residueTime --- int
    return self
end

---@return number
function PhysicalPowerUserPropsResponse:protocolId()
    return 1026
end

---@return string
function PhysicalPowerUserPropsResponse:write()
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

---@return PhysicalPowerUserPropsResponse
function PhysicalPowerUserPropsResponse:read(data)

    local packet = self:new(
            data.maximumStrength,
            data.maximusResidueEndTime,
            data.nowPhysicalPower,
            data.residueNowTime,
            data.residueTime)
    return packet
end

--- 最大体力 用于限制 这个值会随着 等级增长
---@return number 最大体力 用于限制 这个值会随着 等级增长
function PhysicalPowerUserPropsResponse:getMaximumStrength()
    return self.maximumStrength
end
--- 我恢复到最大体力的结束时间
---@return number 我恢复到最大体力的结束时间
function PhysicalPowerUserPropsResponse:getMaximusResidueEndTime()
    return self.maximusResidueEndTime
end
--- 返回 使用体力 所扣除 当前体力
---@return number 返回 使用体力 所扣除 当前体力
function PhysicalPowerUserPropsResponse:getNowPhysicalPower()
    return self.nowPhysicalPower
end
--- 当前体力实时时间 会跟着剩余时间一起变化
---@return number 当前体力实时时间 会跟着剩余时间一起变化
function PhysicalPowerUserPropsResponse:getResidueNowTime()
    return self.residueNowTime
end
--- 一点体力增长剩余时间
---@return number 一点体力增长剩余时间
function PhysicalPowerUserPropsResponse:getResidueTime()
    return self.residueTime
end


return PhysicalPowerUserPropsResponse
