---@class CharacterConfigResponse
local CharacterConfigResponse = class("CharacterConfigResponse")
function CharacterConfigResponse:ctor()
    --- 角色相关
    ---@type  table<number,CharacterConfigData>
    self.characterConfigDataList = {}
end

---@param characterConfigDataList table<number,CharacterConfigData> 角色相关
---@return CharacterConfigResponse
function CharacterConfigResponse:new(characterConfigDataList)
    self.characterConfigDataList = characterConfigDataList --- java.util.List<com.gameServer.common.protocol.character.CharacterConfigData>
    return self
end

---@return number
function CharacterConfigResponse:protocolId()
    return 1053
end

---@return string
function CharacterConfigResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            characterConfigDataList = self.characterConfigDataList
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return CharacterConfigResponse
function CharacterConfigResponse:read(data)
    local characterConfigDataList = {}
    for index, value in ipairs(data.characterConfigDataList) do
        local characterConfigDataListPacket = CharacterConfigData()
        local packetData = characterConfigDataListPacket:read(value)
        table.insert(characterConfigDataList,packetData)
    end

    local packet = self:new(
            characterConfigDataList)
    return packet
end

--- 角色相关
---@type  table<number,CharacterConfigData> 角色相关
function CharacterConfigResponse:getCharacterConfigDataList()
    return self.characterConfigDataList
end


return CharacterConfigResponse
