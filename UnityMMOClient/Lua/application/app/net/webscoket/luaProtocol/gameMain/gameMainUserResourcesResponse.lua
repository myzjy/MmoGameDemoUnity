---@class GameMainUserResourcesResponse
local GameMainUserResourcesResponse = class("GameMainUserResourcesResponse")

function GameMainUserResourcesResponse:ctor()
    ---@type UserMsgInfoData
    self.userMsgInfoData = nil;
end

function GameMainUserResourcesResponse:protocolId()
    return 1052
end

function GameMainUserResourcesResponse:new(userMsgInfoData)
    self.userMsgInfoData = userMsgInfoData;
    return self;
end

function GameMainUserResourcesResponse:read(data)
    local data = UserMsgInfoData()
    local packetData = data:read(data.userMsgInfoData)
    return self:new(packetData)
end

return GameMainUserResourcesResponse
