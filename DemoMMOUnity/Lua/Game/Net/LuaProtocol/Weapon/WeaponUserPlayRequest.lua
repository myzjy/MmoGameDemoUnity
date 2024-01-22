---@class WeaponUserPlayRequest
local WeaponUserPlayRequest = {}

function WeaponUserPlayRequest:new(findUserId, findWeaponId)
    local object = {
        findUserId = findUserId,
        findWeaponId = findWeaponId
    }
    setmetatable(object, self)
    -- 索引指向 自己
    self.__index = self
end

function WeaponUserPlayRequest:protocolId()
    return 1039
end

---comment
---@param buffer ByteBuffer
---@param packet WeaponUserPlayRequest
function WeaponUserPlayRequest:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or WeaponUserPlayRequest
    local message = {
        protocolId = data:protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function WeaponUserPlayRequest:read(buffer)
    local jsonStr = buffer:readString()
    ---@type {protocolId:number,packet:{findUserId:number,findWeaponId:number}}
    local data = JSON.decode(jsonStr)
    local jsonData = WeaponUserPlayRequest:new(data.packet.findUserId, data.packet.findWeaponId)
    return jsonData
end

return WeaponUserPlayRequest
