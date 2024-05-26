---@class GameMainUserInfoToResponse
local GameMainUserInfoToResponse = class("GameMainUserInfoToResponse")
function GameMainUserInfoToResponse:ctor()
    --- 钻石
    ---@type number
    self.diamondsNum = 0
    --- 金币
    ---@type number
    self.goldCoinNum = 0
    --- 最大经验
    ---@type number
    self.maxExp = 0
    --- 最大等级
    ---@type number
    self.maxLv = 0
    --- 当前经验
    ---@type number
    self.nowExp = 0
    --- 当前等级
    ---@type number
    self.nowLv = 0
    --- 付费钻石
    ---@type number
    self.paidDiamondsNum = 0
end

---@param diamondsNum number 钻石
---@param goldCoinNum number 金币
---@param maxExp number 最大经验
---@param maxLv number 最大等级
---@param nowExp number 当前经验
---@param nowLv number 当前等级
---@param paidDiamondsNum number 付费钻石
---@return GameMainUserInfoToResponse
function GameMainUserInfoToResponse:new(diamondsNum, goldCoinNum, maxExp, maxLv, nowExp, nowLv, paidDiamondsNum)
    self.diamondsNum = diamondsNum --- long
    self.goldCoinNum = goldCoinNum --- long
    self.maxExp = maxExp --- int
    self.maxLv = maxLv --- int
    self.nowExp = nowExp --- int
    self.nowLv = nowLv --- int
    self.paidDiamondsNum = paidDiamondsNum --- long
    return self
end

---@return number
function GameMainUserInfoToResponse:protocolId()
    return 1032
end

---@return string
function GameMainUserInfoToResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            diamondsNum = self.diamondsNum,
            goldCoinNum = self.goldCoinNum,
            maxExp = self.maxExp,
            maxLv = self.maxLv,
            nowExp = self.nowExp,
            nowLv = self.nowLv,
            paidDiamondsNum = self.paidDiamondsNum
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return GameMainUserInfoToResponse
function GameMainUserInfoToResponse:read(data)

    local packet = self:new(
            data.diamondsNum,
            data.goldCoinNum,
            data.maxExp,
            data.maxLv,
            data.nowExp,
            data.nowLv,
            data.paidDiamondsNum)
    return packet
end

--- 钻石
---@return number 钻石
function GameMainUserInfoToResponse:getDiamondsNum()
    return self.diamondsNum
end
--- 金币
---@return number 金币
function GameMainUserInfoToResponse:getGoldCoinNum()
    return self.goldCoinNum
end
--- 最大经验
---@return number 最大经验
function GameMainUserInfoToResponse:getMaxExp()
    return self.maxExp
end
--- 最大等级
---@return number 最大等级
function GameMainUserInfoToResponse:getMaxLv()
    return self.maxLv
end
--- 当前经验
---@return number 当前经验
function GameMainUserInfoToResponse:getNowExp()
    return self.nowExp
end
--- 当前等级
---@return number 当前等级
function GameMainUserInfoToResponse:getNowLv()
    return self.nowLv
end
--- 付费钻石
---@return number 付费钻石
function GameMainUserInfoToResponse:getPaidDiamondsNum()
    return self.paidDiamondsNum
end


return GameMainUserInfoToResponse
