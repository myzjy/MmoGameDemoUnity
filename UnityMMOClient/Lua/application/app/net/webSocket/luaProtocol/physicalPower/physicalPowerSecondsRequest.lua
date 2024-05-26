---@class PhysicalPowerSecondsRequest
local PhysicalPowerSecondsRequest = class("PhysicalPowerSecondsRequest")
function PhysicalPowerSecondsRequest:ctor()
    --- 当前时间
    ---@type number
    self.nowTime = 0
end

---@param nowTime number 当前时间
---@return PhysicalPowerSecondsRequest
function PhysicalPowerSecondsRequest:new(nowTime)
    self.nowTime = nowTime --- long
    return self
end

---@return number
function PhysicalPowerSecondsRequest:protocolId()
    return 1029
end

---@return string
function PhysicalPowerSecondsRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            nowTime = self.nowTime
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return PhysicalPowerSecondsRequest
function PhysicalPowerSecondsRequest:read(data)

    local packet = self:new(
            data.nowTime)
    return packet
end

--- 当前时间
---@return number 当前时间
function PhysicalPowerSecondsRequest:getNowTime()
    return self.nowTime
end


return PhysicalPowerSecondsRequest
