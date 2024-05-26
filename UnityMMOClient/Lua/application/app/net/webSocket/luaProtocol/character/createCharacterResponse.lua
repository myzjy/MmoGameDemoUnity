---@class CreateCharacterResponse
local CreateCharacterResponse = class("CreateCharacterResponse")
function CreateCharacterResponse:ctor()
    --- 角色数据
    ---@type CharacterBaseData
    self.characterBaseData = {}
end

---@param characterBaseData CharacterBaseData 角色数据
---@return CreateCharacterResponse
function CreateCharacterResponse:new(characterBaseData)
    self.characterBaseData = characterBaseData --- com.gameServer.common.protocol.character.CharacterBaseData
    return self
end

---@return number
function CreateCharacterResponse:protocolId()
    return 1048
end

---@return string
function CreateCharacterResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            characterBaseData = self.characterBaseData
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return CreateCharacterResponse
function CreateCharacterResponse:read(data)
    local characterBaseDataPacket = CharacterBaseData()
    local characterBaseData = characterBaseDataPacket:read(data.characterBaseData)

    local packet = self:new(
            characterBaseData)
    return packet
end

--- 角色数据
---@return CharacterBaseData 角色数据
function CreateCharacterResponse:getCharacterBaseData()
    return self.characterBaseData
end


return CreateCharacterResponse
