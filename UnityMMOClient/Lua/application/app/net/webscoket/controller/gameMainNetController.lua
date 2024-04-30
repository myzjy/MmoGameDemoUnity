---@class GameMainNetController
local GameMainNetController = class("GameMainNetController")

---@type GameMainNetController
local instance = nil

function GameMainNetController.GetInstance()
    if not instance then
        instance = GameMainNetController()
    end
    return instance
end

function GameMainNetController:init()
end

function GameMainNetController:RegisterEvent()

end

function GameMainNetController:SendPhysicalPowerService(uid)
    local packet = PhysicalPowerRequest(uid)
    local json = packet:write()
    NetManager:SendMessageEvent(json)
end

function GameMainNetController:SendGameMainUIPanelRequest()
    local packet = GameMainUIPanelRequest()
    local json = packet:write()
    NetManager:SendMessageEvent(json)
end

return GameMainNetController
