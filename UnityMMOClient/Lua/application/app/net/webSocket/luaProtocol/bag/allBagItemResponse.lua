---@class AllBagItemResponse
local AllBagItemResponse = class("AllBagItemResponse")
function AllBagItemResponse:ctor()
    --- 返回协议号
    ---@type string
    self.protocolStr = string.empty
    --- 武器
    ---@type  table<number,WeaponPlayerUserDataStruct>
    self.weaponUserList = {}
end

---@param protocolStr string 返回协议号
---@param weaponUserList table<number,WeaponPlayerUserDataStruct> 武器
---@return AllBagItemResponse
function AllBagItemResponse:new(protocolStr, weaponUserList)
    self.protocolStr = protocolStr --- java.lang.String
    self.weaponUserList = weaponUserList --- java.util.List<com.gameServer.common.protocol.weapon.WeaponPlayerUserDataStruct>
    return self
end

---@return number
function AllBagItemResponse:protocolId()
    return 1008
end

---@return string
function AllBagItemResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            protocolStr = self.protocolStr,
            weaponUserList = self.weaponUserList
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return AllBagItemResponse
function AllBagItemResponse:read(data)
    local weaponUserList = {}
    for index, value in ipairs(data.weaponUserList) do
        local weaponUserListPacket = WeaponPlayerUserDataStruct()
        local packetData = weaponUserListPacket:read(value)
        table.insert(weaponUserList,packetData)
    end

    local packet = self:new(
            data.protocolStr,
            weaponUserList)
    return packet
end

--- 返回协议号
---@type  string 返回协议号
function AllBagItemResponse:getProtocolStr()
    return self.protocolStr
end

--- 武器
---@type  table<number,WeaponPlayerUserDataStruct> 武器
function AllBagItemResponse:getWeaponUserList()
    return self.weaponUserList
end


return AllBagItemResponse
