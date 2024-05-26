---@class GetGameMainCharacterAllResponse
local GetGameMainCharacterAllResponse = class("GetGameMainCharacterAllResponse")
function GetGameMainCharacterAllResponse:ctor()
    --- 角色数据当前玩家拥有的角色
    ---@type  table<number,CharacterBaseData>
    self.characterBaseDataList = {}
end

---@param characterBaseDataList table<number,CharacterBaseData> 角色数据当前玩家拥有的角色
---@return GetGameMainCharacterAllResponse
function GetGameMainCharacterAllResponse:new(characterBaseDataList)
    self.characterBaseDataList = characterBaseDataList --- java.util.List<com.gameServer.common.protocol.character.CharacterBaseData>
    return self
end

---@return number
function GetGameMainCharacterAllResponse:protocolId()
    return 1050
end

---@return string
function GetGameMainCharacterAllResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            characterBaseDataList = self.characterBaseDataList
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return GetGameMainCharacterAllResponse
function GetGameMainCharacterAllResponse:read(data)
    local characterBaseDataList = {}
    for index, value in ipairs(data.characterBaseDataList) do
        local characterBaseDataListPacket = CharacterBaseData()
        local packetData = characterBaseDataListPacket:read(value)
        table.insert(characterBaseDataList,packetData)
    end

    local packet = self:new(
            characterBaseDataList)
    return packet
end

--- 角色数据当前玩家拥有的角色
---@type  table<number,CharacterBaseData> 角色数据当前玩家拥有的角色
function GetGameMainCharacterAllResponse:getCharacterBaseDataList()
    return self.characterBaseDataList
end


return GetGameMainCharacterAllResponse
