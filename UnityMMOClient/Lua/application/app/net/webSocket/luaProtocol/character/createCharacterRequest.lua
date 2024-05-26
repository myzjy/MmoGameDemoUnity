---@class CreateCharacterRequest
local CreateCharacterRequest = class("CreateCharacterRequest")
function CreateCharacterRequest:ctor()
    --- 角色id
    ---@type number
    self.characterId = 0
end

---@param characterId number 角色id
---@return CreateCharacterRequest
function CreateCharacterRequest:new(characterId)
    self.characterId = characterId --- int
    return self
end

---@return number
function CreateCharacterRequest:protocolId()
    return 1047
end

---@return string
function CreateCharacterRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            characterId = self.characterId
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return CreateCharacterRequest
function CreateCharacterRequest:read(data)

    local packet = self:new(
            data.characterId)
    return packet
end

--- 角色id
---@return number 角色id
function CreateCharacterRequest:getCharacterId()
    return self.characterId
end


return CreateCharacterRequest
