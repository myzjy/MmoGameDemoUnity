---@class PlayerSceneInfoData
local PlayerSceneInfoData = class("PlayerSceneInfoData")
function PlayerSceneInfoData:ctor()
    --- 角色在场景中得旋转角度 x
    ---@type number
    self.sceneCharacterRotationX = 0
    --- 角色在场景中得旋转角度 Y
    ---@type number
    self.sceneCharacterRotationY = 0
    --- 角色在场景中得旋转角度 z
    ---@type number
    self.sceneCharacterRotationZ = 0
    --- 场景id
    ---@type number
    self.sceneId = 0
    --- 场景中 坐标 x
    ---@type number
    self.scenePosX = 0
    --- 场景中 坐标 y
    ---@type number
    self.scenePosY = 0
    --- 场景中 坐标 z
    ---@type number
    self.scenePosZ = 0
    --- 场景名
    ---@type string
    self.sceneStr = string.empty
end

---@param sceneCharacterRotationX number 角色在场景中得旋转角度 x
---@param sceneCharacterRotationY number 角色在场景中得旋转角度 Y
---@param sceneCharacterRotationZ number 角色在场景中得旋转角度 z
---@param sceneId number 场景id
---@param scenePosX number 场景中 坐标 x
---@param scenePosY number 场景中 坐标 y
---@param scenePosZ number 场景中 坐标 z
---@param sceneStr string 场景名
---@return PlayerSceneInfoData
function PlayerSceneInfoData:new(sceneCharacterRotationX, sceneCharacterRotationY, sceneCharacterRotationZ, sceneId, scenePosX, scenePosY, scenePosZ, sceneStr)
    self.sceneCharacterRotationX = sceneCharacterRotationX --- float
    self.sceneCharacterRotationY = sceneCharacterRotationY --- float
    self.sceneCharacterRotationZ = sceneCharacterRotationZ --- float
    self.sceneId = sceneId --- int
    self.scenePosX = scenePosX --- float
    self.scenePosY = scenePosY --- float
    self.scenePosZ = scenePosZ --- float
    self.sceneStr = sceneStr --- java.lang.String
    return self
end

---@return number
function PlayerSceneInfoData:protocolId()
    return 221
end

---@return string
function PlayerSceneInfoData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            sceneCharacterRotationX = self.sceneCharacterRotationX,
            sceneCharacterRotationY = self.sceneCharacterRotationY,
            sceneCharacterRotationZ = self.sceneCharacterRotationZ,
            sceneId = self.sceneId,
            scenePosX = self.scenePosX,
            scenePosY = self.scenePosY,
            scenePosZ = self.scenePosZ,
            sceneStr = self.sceneStr
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return PlayerSceneInfoData
function PlayerSceneInfoData:read(data)

    local packet = self:new(
            data.sceneCharacterRotationX,
            data.sceneCharacterRotationY,
            data.sceneCharacterRotationZ,
            data.sceneId,
            data.scenePosX,
            data.scenePosY,
            data.scenePosZ,
            data.sceneStr)
    return packet
end

--- 角色在场景中得旋转角度 x
---@return number 角色在场景中得旋转角度 x
function PlayerSceneInfoData:getSceneCharacterRotationX()
    return self.sceneCharacterRotationX
end
--- 角色在场景中得旋转角度 Y
---@return number 角色在场景中得旋转角度 Y
function PlayerSceneInfoData:getSceneCharacterRotationY()
    return self.sceneCharacterRotationY
end
--- 角色在场景中得旋转角度 z
---@return number 角色在场景中得旋转角度 z
function PlayerSceneInfoData:getSceneCharacterRotationZ()
    return self.sceneCharacterRotationZ
end
--- 场景id
---@return number 场景id
function PlayerSceneInfoData:getSceneId()
    return self.sceneId
end
--- 场景中 坐标 x
---@return number 场景中 坐标 x
function PlayerSceneInfoData:getScenePosX()
    return self.scenePosX
end
--- 场景中 坐标 y
---@return number 场景中 坐标 y
function PlayerSceneInfoData:getScenePosY()
    return self.scenePosY
end
--- 场景中 坐标 z
---@return number 场景中 坐标 z
function PlayerSceneInfoData:getScenePosZ()
    return self.scenePosZ
end
--- 场景名
---@type  string 场景名
function PlayerSceneInfoData:getSceneStr()
    return self.sceneStr
end



return PlayerSceneInfoData
