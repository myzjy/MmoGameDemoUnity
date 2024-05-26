---@class LoginTapToStartResponse
local LoginTapToStartResponse = class("LoginTapToStartResponse")
function LoginTapToStartResponse:ctor()
    --- 是否能进入游戏
    ---@type boolean
    self.accessGame = 0
    --- 当不能进入游戏时，提示相关信息
    ---@type string
    self.message = string.empty
end

---@param boolean boolean 是否能进入游戏
---@param message string 当不能进入游戏时，提示相关信息
---@return LoginTapToStartResponse
function LoginTapToStartResponse:new(accessGame, message)
    self.accessGame = accessGame --- boolean
    self.message = message --- java.lang.String
    return self
end

---@return number
function LoginTapToStartResponse:protocolId()
    return 1014
end

---@return string
function LoginTapToStartResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            accessGame = self.accessGame,
            message = self.message
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return LoginTapToStartResponse
function LoginTapToStartResponse:read(data)

    local packet = self:new(
            data.accessGame,
            data.message)
    return packet
end

--- 是否能进入游戏
---@return boolean 是否能进入游戏
function LoginTapToStartResponse:getAccessGame()
    return self.accessGame
end
--- 当不能进入游戏时，提示相关信息
---@type  string 当不能进入游戏时，提示相关信息
function LoginTapToStartResponse:getMessage()
    return self.message
end



return LoginTapToStartResponse
