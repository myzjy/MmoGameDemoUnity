---@class WeaponPlayerUserDataResponse
local WeaponPlayerUserDataResponse = class("WeaponPlayerUserDataResponse")
---@type WeaponPlayerUserDataResponse
local this = nil
function WeaponPlayerUserDataResponse:ctor()
    --赋值
    this = self
    self.usePlayerUid = -1                   ---java.lang.long
    self.weaponPlayerUserDataStructList = {} ---java.util.list<WeaponPlayerUserDataStruct>
end

function WeaponPlayerUserDataResponse:protocolId()
    return 1040
end

---@param usePlayerUid number
---@param weaponPlayerUserDataStructList table<number,WeaponPlayerUserDataStruct>
function WeaponPlayerUserDataResponse:new(usePlayerUid, weaponPlayerUserDataStructList)
    self.usePlayerUid = usePlayerUid                                     ---java.lang.long
    self.weaponPlayerUserDataStructList = weaponPlayerUserDataStructList ---java.util.list<WeaponPlayerUserDataStruct>
    return self
end

function WeaponPlayerUserDataResponse:read(data)

    local packet = data
    self.usePlayerUid = packet.usePlayerUid

    for _index = 1, #packet.weaponPlayerUserDataStructList do
        local forData = packet.weaponPlayerUserDataStructList[_index]
        ---获取到自己
        local packetData = WeaponPlayerUserDataStruct()
        table.insert(self.weaponPlayerUserDataStructList, packetData:readData(forData))
    end
    return self
end
