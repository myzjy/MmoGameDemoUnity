---@class PhysicalPowerUserPropsRequest
local PhysicalPowerUserPropsRequest = class("PhysicalPowerUserPropsRequest")
function PhysicalPowerUserPropsRequest:ctor()
    --- 使用体力会被扣除
    ---@type number
    self.usePropNum = 0
end

---@param usePropNum number 使用体力会被扣除
---@return PhysicalPowerUserPropsRequest
function PhysicalPowerUserPropsRequest:new(usePropNum)
    self.usePropNum = usePropNum --- int
    return self
end

---@return number
function PhysicalPowerUserPropsRequest:protocolId()
    return 1025
end

---@return string
function PhysicalPowerUserPropsRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            usePropNum = self.usePropNum
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return PhysicalPowerUserPropsRequest
function PhysicalPowerUserPropsRequest:read(data)

    local packet = self:new(
            data.usePropNum)
    return packet
end

--- 使用体力会被扣除
---@return number 使用体力会被扣除
function PhysicalPowerUserPropsRequest:getUsePropNum()
    return self.usePropNum
end


return PhysicalPowerUserPropsRequest
