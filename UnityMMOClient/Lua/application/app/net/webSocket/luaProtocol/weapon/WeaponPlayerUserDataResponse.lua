---@class WeaponPlayerUserDataResponse
local WeaponPlayerUserDataResponse = class("WeaponPlayerUserDataResponse")
function WeaponPlayerUserDataResponse:ctor()
    --- 当前玩家调用查询到的是谁的装备
    ---@type number
    self.usePlayerUid = 0
    --- 玩家武器数据
    ---@type  table<number,WeaponPlayerUserDataStruct>
    self.weaponPlayerUserDataStructList = {}
end

---@param usePlayerUid number 当前玩家调用查询到的是谁的装备
---@param weaponPlayerUserDataStructList table<number,WeaponPlayerUserDataStruct> 玩家武器数据
---@return WeaponPlayerUserDataResponse
function WeaponPlayerUserDataResponse:new(usePlayerUid, weaponPlayerUserDataStructList)
    self.usePlayerUid = usePlayerUid --- long
    self.weaponPlayerUserDataStructList = weaponPlayerUserDataStructList --- java.util.List<com.gameServer.common.protocol.weapon.WeaponPlayerUserDataStruct>
    return self
end

---@return number
function WeaponPlayerUserDataResponse:protocolId()
    return 1040
end

---@return string
function WeaponPlayerUserDataResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            usePlayerUid = self.usePlayerUid,
            weaponPlayerUserDataStructList = self.weaponPlayerUserDataStructList
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return WeaponPlayerUserDataResponse
function WeaponPlayerUserDataResponse:read(data)
    local weaponPlayerUserDataStructList = {}
    for index, value in ipairs(data.weaponPlayerUserDataStructList) do
        local weaponPlayerUserDataStructListPacket = WeaponPlayerUserDataStruct()
        local packetData = weaponPlayerUserDataStructListPacket:read(value)
        table.insert(weaponPlayerUserDataStructList,packetData)
    end

    local packet = self:new(
            data.usePlayerUid,
            weaponPlayerUserDataStructList)
    return packet
end

--- 当前玩家调用查询到的是谁的装备
---@return number 当前玩家调用查询到的是谁的装备
function WeaponPlayerUserDataResponse:getUsePlayerUid()
    return self.usePlayerUid
end
--- 玩家武器数据
---@type  table<number,WeaponPlayerUserDataStruct> 玩家武器数据
function WeaponPlayerUserDataResponse:getWeaponPlayerUserDataStructList()
    return self.weaponPlayerUserDataStructList
end


return WeaponPlayerUserDataResponse
