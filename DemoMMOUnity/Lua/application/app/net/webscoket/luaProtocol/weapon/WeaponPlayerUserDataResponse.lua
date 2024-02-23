---@class WeaponPlayerUserDataResponse
local WeaponPlayerUserDataResponse = class("WeaponPlayerUserDataResponse")

function WeaponPlayerUserDataResponse:ctor()
    self.usePlayerUid = -1                    ---java.lang.long
    self.weaponPlayerUserDataStructList = nil ---java.util.list<WeaponPlayerUserDataStruct>
end

---@param usePlayerUid number
---@param weaponPlayerUserDataStructList table<number,WeaponPlayerUserDataStruct>
function WeaponPlayerUserDataResponse:new(usePlayerUid, weaponPlayerUserDataStructList)
    self.usePlayerUid = usePlayerUid                                     ---java.lang.long
    self.weaponPlayerUserDataStructList = weaponPlayerUserDataStructList ---java.util.list<WeaponPlayerUserDataStruct>
end
