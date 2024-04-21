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

---@param data LoginResponse
---@return LoginUserServerInfoData
function LoginUserServerInfoData:read(data)
    local playerSceneInfoDataServer = PlayerSceneInfoData();
    local loginUserInfoData=data.loginUserServerInfoData
    local playerSceneInfoData = playerSceneInfoDataServer:read(loginUserInfoData.playerSceneInfoData);
    local userMsgInfoDataServer = UserMsgInfoData();
    local userMsgInfoData = userMsgInfoDataServer:read(loginUserInfoData.userMsgInfoData)
    local jsonData = LoginUserServerInfoData:new(playerSceneInfoData, userMsgInfoData)
    return jsonData
end

return LoginUserServerInfoData
