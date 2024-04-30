--- 体力 response
---@class PhysicalPowerResponse
local PhysicalPowerResponse = class("PhysicalPowerResponse")
--- 初始化
function PhysicalPowerResponse:ctor()
    ---@type PhysicalPowerInfoData 体力数据 包起来
    self.physicalPowerInfoData = nil;
end

--- 协议号
--- @return integer
function PhysicalPowerResponse:protocolId()
    return 1024
end

--- 赋值
--- @param physicalPowerInfoData PhysicalPowerInfoData
--- @return PhysicalPowerResponse
function PhysicalPowerResponse:new(physicalPowerInfoData)
    self.physicalPowerInfoData = physicalPowerInfoData
    return self
end

--- comment
--- @return PhysicalPowerInfoData
function PhysicalPowerResponse:getPhysicalPowerInfoData()
    return self.physicalPowerInfoData
end

--- 读取值 返回
--- @param data PhysicalPowerResponse
--- @return PhysicalPowerResponse
function PhysicalPowerResponse:read(data)
    local physicalPowerInfo = PhysicalPowerInfoData()

    local pakcet = self:new(physicalPowerInfo:read(data.physicalPowerInfoData))
    return pakcet
end

return PhysicalPowerResponse
