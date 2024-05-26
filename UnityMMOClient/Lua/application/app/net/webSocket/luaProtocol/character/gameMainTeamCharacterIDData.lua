---@class GameMainTeamCharacterIDData
local GameMainTeamCharacterIDData = class("GameMainTeamCharacterIDData")
function GameMainTeamCharacterIDData:ctor()
    --- 角色id
    ---@type number
    self.characterId = 0
    --- 这个角色是否出战为主控
    ---@type number
    self.fightTeamIndex = 0
    --- 这个角色在队伍的第几位
    ---@type number
    self.teamIndex = 0
    --- 玩家id
    ---@type number
    self.uid = 0
end

---@param characterId number 角色id
---@param fightTeamIndex number 这个角色是否出战为主控
---@param teamIndex number 这个角色在队伍的第几位
---@param uid number 玩家id
---@return GameMainTeamCharacterIDData
function GameMainTeamCharacterIDData:new(characterId, fightTeamIndex, teamIndex, uid)
    self.characterId = characterId --- int
    self.fightTeamIndex = fightTeamIndex --- int
    self.teamIndex = teamIndex --- int
    self.uid = uid --- long
    return self
end

---@return number
function GameMainTeamCharacterIDData:protocolId()
    return 219
end

---@return string
function GameMainTeamCharacterIDData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            characterId = self.characterId,
            fightTeamIndex = self.fightTeamIndex,
            teamIndex = self.teamIndex,
            uid = self.uid
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return GameMainTeamCharacterIDData
function GameMainTeamCharacterIDData:read(data)

    local packet = self:new(
            data.characterId,
            data.fightTeamIndex,
            data.teamIndex,
            data.uid)
    return packet
end

--- 角色id
---@return number 角色id
function GameMainTeamCharacterIDData:getCharacterId()
    return self.characterId
end
--- 这个角色是否出战为主控
---@return number 这个角色是否出战为主控
function GameMainTeamCharacterIDData:getFightTeamIndex()
    return self.fightTeamIndex
end
--- 这个角色在队伍的第几位
---@return number 这个角色在队伍的第几位
function GameMainTeamCharacterIDData:getTeamIndex()
    return self.teamIndex
end
--- 玩家id
---@return number 玩家id
function GameMainTeamCharacterIDData:getUid()
    return self.uid
end


return GameMainTeamCharacterIDData
