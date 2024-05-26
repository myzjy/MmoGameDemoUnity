---@class AcquireCharacterResponse
local AcquireCharacterResponse = class("AcquireCharacterResponse")
function AcquireCharacterResponse:ctor()
    --- 玩家的角色所有数据获取list
    ---@type  table<number,CharacterBaseData>
    self.characterBaseDataList = {}
end

---@param characterBaseDataList table<number,CharacterBaseData> 玩家的角色所有数据获取list
---@return AcquireCharacterResponse
function AcquireCharacterResponse:new(characterBaseDataList)
    self.characterBaseDataList = characterBaseDataList --- java.util.List<com.gameServer.common.protocol.character.CharacterBaseData>
    return self
end

---@return number
function AcquireCharacterResponse:protocolId()
    return 1046
end

---@return string
function AcquireCharacterResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            characterBaseDataList = self.characterBaseDataList
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return AcquireCharacterResponse
function AcquireCharacterResponse:read(data)
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

--- 玩家的角色所有数据获取list
---@type  table<number,CharacterBaseData> 玩家的角色所有数据获取list
function AcquireCharacterResponse:getCharacterBaseDataList()
    return self.characterBaseDataList
end


return AcquireCharacterResponse
