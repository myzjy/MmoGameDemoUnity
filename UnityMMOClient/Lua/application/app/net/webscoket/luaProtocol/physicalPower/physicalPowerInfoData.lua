--- 体力 response
---@class PhysicalPowerInfoData
local PhysicalPowerInfoData = class("PhysicalPowerInfoData")
--- 初始化
function PhysicalPowerInfoData:ctor()
    PrintDebug("Init self PhysicalPowerInfoData ing")
    ---@type number 当前体力
    self.nowPhysicalPower = 0
    ---@type number 一点体力增长剩余时间
    self.residueTime = 0;
    ---@type number 最大体力 用于限制 这个值会随着 等级增长
    self.maximumStrength = 0
    ---@type number 我恢复到最大体力的结束时间
    self.maximusResidueEndTime = 0
    ---@type number 当前体力实时时间 会跟着剩余时间一起变化
    self.residueNowTime = 0
end

--- 协议号
--- @return integer
function PhysicalPowerInfoData:protocolId()
    return 223
end

--- 赋值
--- @param nowPhysicalPower number
--- @param residueTime number
--- @param maximumStrength number
--- @param maximusResidueEndTime number
--- @param residueNowTime number
--- @return PhysicalPowerInfoData
function PhysicalPowerInfoData:new(nowPhysicalPower, residueTime, maximumStrength, maximusResidueEndTime, residueNowTime)
    self.nowPhysicalPower = nowPhysicalPower
    self.residueNowTime = residueNowTime
    self.maximumStrength = maximumStrength
    self.maximusResidueEndTime = maximusResidueEndTime
    self.residueTime = residueTime
    return self
end

--- 读取值 返回
--- @param data PhysicalPowerInfoData
--- @return PhysicalPowerInfoData
function PhysicalPowerInfoData:read(data)
    local pakcet = self:new(data.nowPhysicalPower,
        data.residueTime,
        data.maximumStrength,
        data.maximusResidueEndTime,
        data.residueTime
    )
    return pakcet
end

--- 当前体力
--- @return number 当前体力
function PhysicalPowerInfoData:getNowPhysicalPower()
    return self.nowPhysicalPower;
end

--- 一点体力增长剩余时间
--- @return number 一点体力增长剩余时间
function PhysicalPowerInfoData:getResidueTime()
    return self.residueTime;
end

--- 当前体力实时时间 会跟着剩余时间一起变化
--- @return number 当前体力实时时间 会跟着剩余时间一起变化
function PhysicalPowerInfoData:getResidueNowTime()
    return self.residueNowTime;
end

--- 最大体力 用于限制 这个值会随着 等级增长
--- @return number 最大体力 用于限制 这个值会随着 等级增长
function PhysicalPowerInfoData:getMaximumStrength()
    return self.maximumStrength;
end

--- 我恢复到最大体力的结束时间
--- @return number 我恢复到最大体力的结束时间
function PhysicalPowerInfoData:getMaximusResidueEndTime()
    return self.maximusResidueEndTime;
end

return PhysicalPowerInfoData
