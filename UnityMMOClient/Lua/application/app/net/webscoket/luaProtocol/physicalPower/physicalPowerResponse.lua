--- 体力 response
---@class PhysicalPowerResponse
local PhysicalPowerResponse = class("PhysicalPowerResponse")
--- 初始化
function PhysicalPowerResponse:ctor()
    ---@type number 当前体力
    self.nowPhysicalPower = 0
    ---@type number 一点体力增长剩余时间
    self.residuTime = 0;
    ---@type number 最大体力 用于限制 这个值会随着 等级增长
    self.maximumStrength = 0
    ---@type number 我恢复到最大体力的结束时间
    self.maximusResidueEndTime = 0
    ---@type number 当前体力实时时间 会跟着剩余时间一起变化
    self.residuNowTime = 0
end

--- 协议号
--- @return integer
function PhysicalPowerResponse:protocolId()
    return 1024
end

--- 赋值
--- @param nowPhysicalPower number
--- @param residuTime number
--- @param maximumStrength number
--- @param maximusResidueEndTime number
--- @param residuNowTime number
--- @return PhysicalPowerResponse
function PhysicalPowerResponse:new(nowPhysicalPower, residuTime, maximumStrength, maximusResidueEndTime, residuNowTime)
    self.nowPhysicalPower = nowPhysicalPower
    self.residuNowTime = residuNowTime
    self.maximumStrength = maximumStrength
    self.maximusResidueEndTime = maximusResidueEndTime
    self.residuTime = residuTime
    return self
end

--- 读取值 返回
--- @param data PhysicalPowerResponse
--- @return PhysicalPowerResponse
function PhysicalPowerResponse:read(data)
    local pakcet = self:new(data.nowPhysicalPower,
        data.residuNowTime,
        data.maximumStrength,
        data.maximusResidueEndTime,
        data.residuTime
    )
    return pakcet
end

return PhysicalPowerResponse
