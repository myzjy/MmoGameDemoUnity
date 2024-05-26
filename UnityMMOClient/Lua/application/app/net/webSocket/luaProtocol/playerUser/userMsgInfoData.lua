---@class UserMsgInfoData
local UserMsgInfoData = class("UserMsgInfoData")
function UserMsgInfoData:ctor()
    --- 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
    ---@type number
    self.diamondNum = 0
    --- 经验
    ---@type number
    self.exp = 0
    --- 金币
    ---@type number
    self.goldNum = 0
    --- 等级
    ---@type number
    self.lv = 0
    --- 当前等级的最大经验值
    ---@type number
    self.maxExp = 0
    --- 最大等级
    ---@type number
    self.maxLv = 0
    --- 付费钻石 一般充值才有，付费钻石转换成普通钻石
    ---@type number
    self.premiumDiamondNum = 0
    --- 玩家名
    ---@type string
    self.userName = string.empty
end

---@param diamondNum number 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
---@param exp number 经验
---@param goldNum number 金币
---@param lv number 等级
---@param maxExp number 当前等级的最大经验值
---@param maxLv number 最大等级
---@param premiumDiamondNum number 付费钻石 一般充值才有，付费钻石转换成普通钻石
---@param userName string 玩家名
---@return UserMsgInfoData
function UserMsgInfoData:new(diamondNum, exp, goldNum, lv, maxExp, maxLv, premiumDiamondNum, userName)
    self.diamondNum = diamondNum --- long
    self.exp = exp --- int
    self.goldNum = goldNum --- long
    self.lv = lv --- int
    self.maxExp = maxExp --- int
    self.maxLv = maxLv --- int
    self.premiumDiamondNum = premiumDiamondNum --- long
    self.userName = userName --- java.lang.String
    return self
end

---@return number
function UserMsgInfoData:protocolId()
    return 222
end

---@return string
function UserMsgInfoData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            diamondNum = self.diamondNum,
            exp = self.exp,
            goldNum = self.goldNum,
            lv = self.lv,
            maxExp = self.maxExp,
            maxLv = self.maxLv,
            premiumDiamondNum = self.premiumDiamondNum,
            userName = self.userName
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return UserMsgInfoData
function UserMsgInfoData:read(data)

    local packet = self:new(
            data.diamondNum,
            data.exp,
            data.goldNum,
            data.lv,
            data.maxExp,
            data.maxLv,
            data.premiumDiamondNum,
            data.userName)
    return packet
end

--- 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
---@return number 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
function UserMsgInfoData:getDiamondNum()
    return self.diamondNum
end
--- 经验
---@return number 经验
function UserMsgInfoData:getExp()
    return self.exp
end
--- 金币
---@return number 金币
function UserMsgInfoData:getGoldNum()
    return self.goldNum
end
--- 等级
---@return number 等级
function UserMsgInfoData:getLv()
    return self.lv
end
--- 当前等级的最大经验值
---@return number 当前等级的最大经验值
function UserMsgInfoData:getMaxExp()
    return self.maxExp
end
--- 最大等级
---@return number 最大等级
function UserMsgInfoData:getMaxLv()
    return self.maxLv
end
--- 付费钻石 一般充值才有，付费钻石转换成普通钻石
---@return number 付费钻石 一般充值才有，付费钻石转换成普通钻石
function UserMsgInfoData:getPremiumDiamondNum()
    return self.premiumDiamondNum
end
--- 玩家名
---@type  string 玩家名
function UserMsgInfoData:getUserName()
    return self.userName
end



return UserMsgInfoData
