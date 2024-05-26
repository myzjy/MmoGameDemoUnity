---@class GameMainUserResourcesResponse
local GameMainUserResourcesResponse = class("GameMainUserResourcesResponse")
function GameMainUserResourcesResponse:ctor()
    --- user 相关 资源信息
    ---@type UserMsgInfoData
    self.userMsgInfoData = {}
end

---@param userMsgInfoData UserMsgInfoData user 相关 资源信息
---@return GameMainUserResourcesResponse
function GameMainUserResourcesResponse:new(userMsgInfoData)
    self.userMsgInfoData = userMsgInfoData --- com.gameServer.common.protocol.playerUser.UserMsgInfoData
    return self
end

---@return number
function GameMainUserResourcesResponse:protocolId()
    return 1052
end

---@return string
function GameMainUserResourcesResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            userMsgInfoData = self.userMsgInfoData
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return GameMainUserResourcesResponse
function GameMainUserResourcesResponse:read(data)
    local userMsgInfoDataPacket = UserMsgInfoData()
    local userMsgInfoData = userMsgInfoDataPacket:read(data.userMsgInfoData)

    local packet = self:new(
            userMsgInfoData)
    return packet
end

--- user 相关 资源信息
---@return UserMsgInfoData user 相关 资源信息
function GameMainUserResourcesResponse:getUserMsgInfoData()
    return self.userMsgInfoData
end


return GameMainUserResourcesResponse
