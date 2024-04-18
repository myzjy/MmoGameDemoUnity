---@class PlayerSceneInfoData
local PlayerSceneInfoData = class("PlayerSceneInfoData")
function PlayerSceneInfoData:ctor()
    ---@type integer 场景id
    self.sceneId = 0;
    ---@type string 场景名
    self.sceneStr = string.empty;
    ---@type number 场景中 坐标 x
    self.scenePosX = 0;
    ---@type number 场景中 坐标 y
    self.scenePosY = 0;
    ---@type number 场景中 坐标 z
    self.scenePosZ = 0;
    ---@type number 角色在场景中得旋转角度 x
    self.sceneCharacterRotationX = 0;
    ---@type number 角色在场景中得旋转角度 y
    self.sceneCharacterRotationY = 0;
    ---@type number 角色在场景中得旋转角度 z
    self.sceneCharacterRotationZ = 0;
end

function PlayerSceneInfoData:protocolId()
    return 221
end

function PlayerSceneInfoData:new(sceneId, sceneStr, scenePosX, scenePosY, scenePosZ, sceneCharacterRotationX,
                                 sceneCharacterRotationY, sceneCharacterRotationZ)
    self.sceneId = sceneId
    self.sceneStr = sceneStr
    self.scenePosX = scenePosX;
    self.scenePosY = scenePosY;
    self.scenePosZ = scenePosZ;
    self.sceneCharacterRotationX = sceneCharacterRotationX;
    self.sceneCharacterRotationY = sceneCharacterRotationY;
    self.sceneCharacterRotationZ = sceneCharacterRotationZ;
    return self
end

function PlayerSceneInfoData:read(data)
    -- local jsonString = buffer:readString()
    -- ---字节读取器中存放字符
    -- local data = JSON.decode(jsonString)
    local jsonData = PlayerSceneInfoData:new(
        data.sceneId,
        data.sceneStr,
        data.scenePosX,
        data.scenePosY,
        data.scenePosZ,
        data.sceneCharacterRotationX,
        data.sceneCharacterRotationY,
        data.sceneCharacterRotationZ)
    return jsonData
end

return PlayerSceneInfoData
