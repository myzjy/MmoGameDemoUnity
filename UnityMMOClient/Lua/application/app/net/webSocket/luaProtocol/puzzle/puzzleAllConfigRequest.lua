---@class PuzzleAllConfigRequest
local PuzzleAllConfigRequest = class("PuzzleAllConfigRequest")
function PuzzleAllConfigRequest:ctor()
    --- 事件ID 也可以说是 活动id
    ---@type number
    self.eventId = 0
end

---@param eventId number 事件ID 也可以说是 活动id
---@return PuzzleAllConfigRequest
function PuzzleAllConfigRequest:new(eventId)
    self.eventId = eventId --- int
    return self
end

---@return number
function PuzzleAllConfigRequest:protocolId()
    return 1035
end

---@return string
function PuzzleAllConfigRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            eventId = self.eventId
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return PuzzleAllConfigRequest
function PuzzleAllConfigRequest:read(data)

    local packet = self:new(
            data.eventId)
    return packet
end

--- 事件ID 也可以说是 活动id
---@return number 事件ID 也可以说是 活动id
function PuzzleAllConfigRequest:getEventId()
    return self.eventId
end


return PuzzleAllConfigRequest
