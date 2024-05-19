--- 体力 response
---@class PhysicalPowerSecondsResponse
local PhysicalPowerSecondsResponse = class("PhysicalPowerSecondsResponse")
--- 初始化
function PhysicalPowerSecondsResponse:ctor()
    ---@type PhysicalPowerInfoData 体力数据 包起来
    self.physicalPowerInfoData = nil;
end

--- 协议号
--- @return integer
function PhysicalPowerSecondsResponse:protocolId()
    return 1030
end

--- 赋值
--- @param physicalPowerInfoData PhysicalPowerInfoData
--- @return PhysicalPowerSecondsResponse
function PhysicalPowerSecondsResponse:new(physicalPowerInfoData)
    self.physicalPowerInfoData = physicalPowerInfoData
    return self
end

--- comment
--- @return PhysicalPowerInfoData
function PhysicalPowerSecondsResponse:getPhysicalPowerInfoData()
    return self.physicalPowerInfoData
end

--- 读取值 返回

function PhysicalPowerSecondsResponse:read(data)
    ---@type PhysicalPowerInfoData
    local physicalPowerInfo = PhysicalPowerInfoData()
    local physicalPowerInfoData = physicalPowerInfo:read(data.physicalPowerInfoData)
    local pakcet = self:new(physicalPowerInfoData)
    return pakcet
end

return PhysicalPowerSecondsResponse
