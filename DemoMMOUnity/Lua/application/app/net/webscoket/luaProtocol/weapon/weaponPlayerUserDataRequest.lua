---@class WeaponPlayerUserDataRequest
local WeaponPlayerUserDataRequest = {}

---@param findUserId number
---@param findWeaponId number
function WeaponPlayerUserDataRequest:new(findUserId, findWeaponId)
    local obj = {
        findUserId = findUserId,    ---java.lang.long
        findWeaponId = findWeaponId ---java.lang.long
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end

function WeaponPlayerUserDataRequest:protocolId()
    return 1039
end

---@param buffer ByteBuffer
---@param packet any|nil
function WeaponPlayerUserDataRequest:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or WeaponPlayerUserDataRequest
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function WeaponPlayerUserDataRequest:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    local data = JSON.decode(jsonString)
    return WeaponPlayerUserDataRequest:new(data.packet.findUserId, data.packet.findWeaponId)
end

return WeaponPlayerUserDataRequest
