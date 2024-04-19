---@class LoginUserServerInfoData
local LoginUserServerInfoData = class("LoginUserServerInfoData")
function LoginUserServerInfoData:ctor()
    ---@type PlayerSceneInfoData
    self.playerSceneInfoData = nil;
    ---@type UserMsgInfoData
    self.userMsgInfoData = nil;
end

---@param playerSceneInfoData PlayerSceneInfoData
---@param userMsgInfoData UserMsgInfoData
function LoginUserServerInfoData:new(playerSceneInfoData, userMsgInfoData)
    self.playerSceneInfoData = playerSceneInfoData;
    self.userMsgInfoData = userMsgInfoData;
    return self;
end

---@param data {playerSceneInfoData:PlayerSceneInfoData,userMsgInfoData:UserMsgInfoData}
---@return LoginUserServerInfoData
function LoginUserServerInfoData:read(data)
    local playerSceneInfoDataServer = PlayerSceneInfoData();
    local playerSceneInfoData = playerSceneInfoDataServer:read(data.playerSceneInfoData);
    local userMsgInfoDataServer = UserMsgInfoData();
    local userMsgInfoData = userMsgInfoDataServer:read(data.userMsgInfoData)
    local jsonData = LoginUserServerInfoData:new(playerSceneInfoData, userMsgInfoData)
    return jsonData
end

return LoginUserServerInfoData
