---@class EquipmentAllDataRequest
local EquipmentAllDataRequest = class("EquipmentAllDataRequest")
function EquipmentAllDataRequest:ctor()
    --- 请求的背包Type
    ---@type number
    self.bagType = 0
end

---@param bagType number 请求的背包Type
---@return EquipmentAllDataRequest
function EquipmentAllDataRequest:new(bagType)
    self.bagType = bagType --- int
    return self
end

---@return number
function EquipmentAllDataRequest:protocolId()
    return 1037
end

---@return string
function EquipmentAllDataRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            bagType = self.bagType
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return EquipmentAllDataRequest
function EquipmentAllDataRequest:read(data)

    local packet = self:new(
            data.bagType)
    return packet
end

--- 请求的背包Type
---@return number 请求的背包Type
function EquipmentAllDataRequest:getBagType()
    return self.bagType
end


return EquipmentAllDataRequest
