---@class Puzzle
local Puzzle = class("Puzzle")
function Puzzle:ctor()
    --- Icon 图片资源名
    ---@type string
    self.icon = string.empty
    --- id
    ---@type number
    self.id = 0
    --- 关联到 上一个关卡id
    ---@type number
    self.lastPuzzleID = 0
    --- 下一个 关卡id 可用在 跳下一关
    ---@type number
    self.nextPuzzleID = 0
    --- 关卡名字
    ---@type string
    self.puzzleName = string.empty
    --- 关卡 奖励
    ---@type  table<number,PuzzleRewardsData>
    self.puzzleRewardsDatas = {}
    --- 资源路径
    ---@type string
    self.resourcePath = string.empty
end

---@param icon string Icon 图片资源名
---@param id number id
---@param lastPuzzleID number 关联到 上一个关卡id
---@param nextPuzzleID number 下一个 关卡id 可用在 跳下一关
---@param puzzleName string 关卡名字
---@param puzzleRewardsDatas table<number,PuzzleRewardsData> 关卡 奖励
---@param resourcePath string 资源路径
---@return Puzzle
function Puzzle:new(icon, id, lastPuzzleID, nextPuzzleID, puzzleName, puzzleRewardsDatas, resourcePath)
    self.icon = icon --- java.lang.String
    self.id = id --- int
    self.lastPuzzleID = lastPuzzleID --- int
    self.nextPuzzleID = nextPuzzleID --- int
    self.puzzleName = puzzleName --- java.lang.String
    self.puzzleRewardsDatas = puzzleRewardsDatas --- java.util.List<com.gameServer.common.protocol.Puzzle.PuzzleRewardsData>
    self.resourcePath = resourcePath --- java.lang.String
    return self
end

---@return number
function Puzzle:protocolId()
    return 202
end

---@return string
function Puzzle:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            icon = self.icon,
            id = self.id,
            lastPuzzleID = self.lastPuzzleID,
            nextPuzzleID = self.nextPuzzleID,
            puzzleName = self.puzzleName,
            puzzleRewardsDatas = self.puzzleRewardsDatas,
            resourcePath = self.resourcePath
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return Puzzle
function Puzzle:read(data)
    local puzzleRewardsDatas = {}
    for index, value in ipairs(data.puzzleRewardsDatas) do
        local puzzleRewardsDatasPacket = PuzzleRewardsData()
        local packetData = puzzleRewardsDatasPacket:read(value)
        table.insert(puzzleRewardsDatas,packetData)
    end

    local packet = self:new(
            data.icon,
            data.id,
            data.lastPuzzleID,
            data.nextPuzzleID,
            data.puzzleName,
            puzzleRewardsDatas,
            data.resourcePath)
    return packet
end

--- Icon 图片资源名
---@type  string Icon 图片资源名
function Puzzle:getIcon()
    return self.icon
end

--- id
---@return number id
function Puzzle:getId()
    return self.id
end
--- 关联到 上一个关卡id
---@return number 关联到 上一个关卡id
function Puzzle:getLastPuzzleID()
    return self.lastPuzzleID
end
--- 下一个 关卡id 可用在 跳下一关
---@return number 下一个 关卡id 可用在 跳下一关
function Puzzle:getNextPuzzleID()
    return self.nextPuzzleID
end
--- 关卡名字
---@type  string 关卡名字
function Puzzle:getPuzzleName()
    return self.puzzleName
end

--- 关卡 奖励
---@type  table<number,PuzzleRewardsData> 关卡 奖励
function Puzzle:getPuzzleRewardsDatas()
    return self.puzzleRewardsDatas
end
--- 资源路径
---@type  string 资源路径
function Puzzle:getResourcePath()
    return self.resourcePath
end



return Puzzle
