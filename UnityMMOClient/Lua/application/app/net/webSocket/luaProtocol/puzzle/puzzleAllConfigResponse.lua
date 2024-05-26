---@class PuzzleAllConfigResponse
local PuzzleAllConfigResponse = class("PuzzleAllConfigResponse")
function PuzzleAllConfigResponse:ctor()
    --- 配置
    ---@type  table<number,PuzzleChapter>
    self.puzzleChapterConfigList = {}
    --- 地图配置
    ---@type  table<number,Puzzle>
    self.puzzleConfigList = {}
end

---@param puzzleChapterConfigList table<number,PuzzleChapter> 配置
---@param puzzleConfigList table<number,Puzzle> 地图配置
---@return PuzzleAllConfigResponse
function PuzzleAllConfigResponse:new(puzzleChapterConfigList, puzzleConfigList)
    self.puzzleChapterConfigList = puzzleChapterConfigList --- java.util.List<com.gameServer.common.protocol.Puzzle.PuzzleChapter>
    self.puzzleConfigList = puzzleConfigList --- java.util.List<com.gameServer.common.protocol.Puzzle.Puzzle>
    return self
end

---@return number
function PuzzleAllConfigResponse:protocolId()
    return 1036
end

---@return string
function PuzzleAllConfigResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            puzzleChapterConfigList = self.puzzleChapterConfigList,
            puzzleConfigList = self.puzzleConfigList
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return PuzzleAllConfigResponse
function PuzzleAllConfigResponse:read(data)
    local puzzleChapterConfigList = {}
    for index, value in ipairs(data.puzzleChapterConfigList) do
        local puzzleChapterConfigListPacket = PuzzleChapter()
        local packetData = puzzleChapterConfigListPacket:read(value)
        table.insert(puzzleChapterConfigList,packetData)
    end
    local puzzleConfigList = {}
    for index, value in ipairs(data.puzzleConfigList) do
        local puzzleConfigListPacket = Puzzle()
        local packetData = puzzleConfigListPacket:read(value)
        table.insert(puzzleConfigList,packetData)
    end

    local packet = self:new(
            puzzleChapterConfigList,
            puzzleConfigList)
    return packet
end

--- 配置
---@type  table<number,PuzzleChapter> 配置
function PuzzleAllConfigResponse:getPuzzleChapterConfigList()
    return self.puzzleChapterConfigList
end
--- 地图配置
---@type  table<number,Puzzle> 地图配置
function PuzzleAllConfigResponse:getPuzzleConfigList()
    return self.puzzleConfigList
end


return PuzzleAllConfigResponse
