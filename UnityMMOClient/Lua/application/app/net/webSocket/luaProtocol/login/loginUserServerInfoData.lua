---@class LoginUserServerInfoData
local LoginUserServerInfoData = class("LoginUserServerInfoData")
function LoginUserServerInfoData:ctor()
    --- 角色在场景中得位置信息
    ---@type PlayerSceneInfoData
    self.playerSceneInfoData = {}
    --- 玩家得一些信息 金币 砖石 付费 砖石
    ---@type UserMsgInfoData
    self.userMsgInfoData = {}
end

---@param playerSceneInfoData PlayerSceneInfoData 角色在场景中得位置信息
---@param userMsgInfoData UserMsgInfoData 玩家得一些信息 金币 砖石 付费 砖石
---@return LoginUserServerInfoData
function LoginUserServerInfoData:new(playerSceneInfoData, userMsgInfoData)
    self.playerSceneInfoData = playerSceneInfoData --- com.gameServer.common.protocol.playerUser.PlayerSceneInfoData
    self.userMsgInfoData = userMsgInfoData --- com.gameServer.common.protocol.playerUser.UserMsgInfoData
    return self
end

---@return number
function LoginUserServerInfoData:protocolId()
    return 220
end

---@return string
function LoginUserServerInfoData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            playerSceneInfoData = self.playerSceneInfoData,
            userMsgInfoData = self.userMsgInfoData
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return LoginUserServerInfoData
function LoginUserServerInfoData:read(data)
    local playerSceneInfoDataPacket = PlayerSceneInfoData()
    local playerSceneInfoData = playerSceneInfoDataPacket:read(data.playerSceneInfoData)
    local userMsgInfoDataPacket = UserMsgInfoData()
    local userMsgInfoData = userMsgInfoDataPacket:read(data.userMsgInfoData)

    local packet = self:new(
            playerSceneInfoData,
            userMsgInfoData)
    return packet
end

--- 角色在场景中得位置信息
---@return PlayerSceneInfoData 角色在场景中得位置信息
function LoginUserServerInfoData:getPlayerSceneInfoData()
    return self.playerSceneInfoData
end
--- 玩家得一些信息 金币 砖石 付费 砖石
---@return UserMsgInfoData 玩家得一些信息 金币 砖石 付费 砖石
function LoginUserServerInfoData:getUserMsgInfoData()
    return self.userMsgInfoData
end


return LoginUserServerInfoData
