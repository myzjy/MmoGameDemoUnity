---@class PhysicalPowerRequest
local PhysicalPowerRequest = class("PhysicalPowerRequest")
function PhysicalPowerRequest:ctor()
    --- 玩家uid传过来，可能会用到
    ---@type number
    self.uid = 0
end

---@param uid number 玩家uid传过来，可能会用到
---@return PhysicalPowerRequest
function PhysicalPowerRequest:new(uid)
    self.uid = uid --- long
    return self
end

---@return number
function PhysicalPowerRequest:protocolId()
    return 1023
end

---@return string
function PhysicalPowerRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            uid = self.uid
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return PhysicalPowerRequest
function PhysicalPowerRequest:read(data)

    local packet = self:new(
            data.uid)
    return packet
end

--- 玩家uid传过来，可能会用到
---@return number 玩家uid传过来，可能会用到
function PhysicalPowerRequest:getUid()
    return self.uid
end


return PhysicalPowerRequest
