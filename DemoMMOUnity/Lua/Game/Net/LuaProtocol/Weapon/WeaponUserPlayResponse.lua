---@class WeaponUserPlayResponse
local WeaponUserPlayResponse = {}

function WeaponUserPlayResponse:new(usePlayerUid, weaponPlayerUserDataStructList)
    local object = {
        usePlayerUid = usePlayerUid,                                    ---java.lang
        weaponPlayerUserDataStructList = weaponPlayerUserDataStructList ---list
    }
    setmetatable(object, self)
    -- 索引指向 自己
    self.__index = self
end

function WeaponUserPlayResponse:protocolId()
    return 1040
end

---comment
---@param buffer ByteBuffer
---@param packet
function WeaponUserPlayResponse:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or WeaponUserPlayResponse
    local message = {
        protocolId = data:protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function WeaponUserPlayResponse:read(buffer)
    local jsonStr = buffer:readString()
    ---@type {protocolId:number,packet:{usePlayerUid:number,weaponPlayerUserDataStructList:WeaponPlayerUserDataStruct[]}}
    local data = JSON.decode(jsonStr)
    local jsonData = WeaponUserPlayResponse:new(data.packet.usePlayerUid, data.packet.weaponPlayerUserDataStructList)
    return jsonData
end

return WeaponUserPlayResponse
