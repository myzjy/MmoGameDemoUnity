---@class PuzzleChapter
local PuzzleChapter = class("PuzzleChapter")
function PuzzleChapter:ctor()
    --- 章节名
    ---@type string
    self.chapterName = string.empty
    --- 完成的最大关卡id 只有 关卡完成之后 才会更新
    ---@type number
    self.doneMaxPuzzleId = 0
    --- id
    ---@type number
    self.id = 0
    --- 当前章节 最大 关卡id
    ---@type number
    self.maxPuzzle = 0
    --- 当前章节 最小 关卡id
    ---@type number
    self.minPuzzle = 0
    --- 当前 进行中的 关卡id
    ---@type number
    self.nowCarryOutPuzzleId = 0
end

---@param chapterName string 章节名
---@param doneMaxPuzzleId number 完成的最大关卡id 只有 关卡完成之后 才会更新
---@param id number id
---@param maxPuzzle number 当前章节 最大 关卡id
---@param minPuzzle number 当前章节 最小 关卡id
---@param nowCarryOutPuzzleId number 当前 进行中的 关卡id
---@return PuzzleChapter
function PuzzleChapter:new(chapterName, doneMaxPuzzleId, id, maxPuzzle, minPuzzle, nowCarryOutPuzzleId)
    self.chapterName = chapterName --- java.lang.String
    self.doneMaxPuzzleId = doneMaxPuzzleId --- int
    self.id = id --- int
    self.maxPuzzle = maxPuzzle --- int
    self.minPuzzle = minPuzzle --- int
    self.nowCarryOutPuzzleId = nowCarryOutPuzzleId --- int
    return self
end

---@return number
function PuzzleChapter:protocolId()
    return 204
end

---@return string
function PuzzleChapter:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            chapterName = self.chapterName,
            doneMaxPuzzleId = self.doneMaxPuzzleId,
            id = self.id,
            maxPuzzle = self.maxPuzzle,
            minPuzzle = self.minPuzzle,
            nowCarryOutPuzzleId = self.nowCarryOutPuzzleId
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return PuzzleChapter
function PuzzleChapter:read(data)

    local packet = self:new(
            data.chapterName,
            data.doneMaxPuzzleId,
            data.id,
            data.maxPuzzle,
            data.minPuzzle,
            data.nowCarryOutPuzzleId)
    return packet
end

--- 章节名
---@type  string 章节名
function PuzzleChapter:getChapterName()
    return self.chapterName
end

--- 完成的最大关卡id 只有 关卡完成之后 才会更新
---@return number 完成的最大关卡id 只有 关卡完成之后 才会更新
function PuzzleChapter:getDoneMaxPuzzleId()
    return self.doneMaxPuzzleId
end
--- id
---@return number id
function PuzzleChapter:getId()
    return self.id
end
--- 当前章节 最大 关卡id
---@return number 当前章节 最大 关卡id
function PuzzleChapter:getMaxPuzzle()
    return self.maxPuzzle
end
--- 当前章节 最小 关卡id
---@return number 当前章节 最小 关卡id
function PuzzleChapter:getMinPuzzle()
    return self.minPuzzle
end
--- 当前 进行中的 关卡id
---@return number 当前 进行中的 关卡id
function PuzzleChapter:getNowCarryOutPuzzleId()
    return self.nowCarryOutPuzzleId
end


return PuzzleChapter
