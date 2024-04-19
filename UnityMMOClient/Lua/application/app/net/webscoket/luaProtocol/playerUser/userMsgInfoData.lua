---@class UserMsgInfoData
local UserMsgInfoData = class("UserMsgInfoData")

function UserMsgInfoData:ctor()
    ---@type string 玩家名字
    self.userName = string.empty;
    ---@type number 金币
    self.goldNum = 0;
    ---@type number 付费钻石 一般充值才有，付费钻石转换成普通钻石
    self.premiumDiamondNum = 0;
    ---@type number 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
    self.diamondNum = 0;
    ---@type integer 等级
    self.lv = 0;
    ---@type integer 最大等级
    self.maxLv = 0;
    ---@type integer 经验
    self.exp = 0;
    ---@type integer 最大经验
    self.maxExp = 0;
end

function UserMsgInfoData:protocolId()
    return 222
end

function UserMsgInfoData:new(userName, goldNum, premiumDiamondNum, diamondNum, lv, maxLv,
                             exp, maxExp)
    self.userName = userName
    self.goldNum = goldNum
    self.premiumDiamondNum = premiumDiamondNum;
    self.diamondNum = diamondNum;
    self.lv = lv;
    self.maxLv = maxLv;
    self.exp = exp;
    self.maxExp = maxExp;
    return self
end

function UserMsgInfoData:read(data)
    -- local jsonString = buffer:readString()
    -- ---字节读取器中存放字符
    -- local data = JSON.decode(jsonString)
    local jsonData = UserMsgInfoData:new(
        data.userName, data.goldNum, data.premiumDiamondNum, data.diamondNum, data.lv, data.maxLv,
        data.exp, data.maxExp)
    return jsonData
end

return UserMsgInfoData
