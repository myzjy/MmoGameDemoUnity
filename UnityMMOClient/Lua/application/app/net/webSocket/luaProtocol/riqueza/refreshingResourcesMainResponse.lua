---@class RefreshingResourcesMainResponse
local RefreshingResourcesMainResponse = class("RefreshingResourcesMainResponse")
function RefreshingResourcesMainResponse:ctor()
    --- 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
    ---@type number
    self.DiamondNum = 0
    --- 付费钻石 一般充值才有，付费钻石转换成普通钻石
    ---@type number
    self.PremiumDiamondNum = 0
    --- 金币
    ---@type number
    self.goldNum = 0
end

---@param DiamondNum number 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
---@param PremiumDiamondNum number 付费钻石 一般充值才有，付费钻石转换成普通钻石
---@param goldNum number 金币
---@return RefreshingResourcesMainResponse
function RefreshingResourcesMainResponse:new(DiamondNum, PremiumDiamondNum, goldNum)
    self.DiamondNum = DiamondNum --- long
    self.PremiumDiamondNum = PremiumDiamondNum --- long
    self.goldNum = goldNum --- long
    return self
end

---@return number
function RefreshingResourcesMainResponse:protocolId()
    return 1028
end

---@return string
function RefreshingResourcesMainResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            DiamondNum = self.DiamondNum,
            PremiumDiamondNum = self.PremiumDiamondNum,
            goldNum = self.goldNum
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return RefreshingResourcesMainResponse
function RefreshingResourcesMainResponse:read(data)

    local packet = self:new(
            data.DiamondNum,
            data.PremiumDiamondNum,
            data.goldNum)
    return packet
end

--- 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
---@return number 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
function RefreshingResourcesMainResponse:getDiamondNum()
    return self.DiamondNum
end
--- 付费钻石 一般充值才有，付费钻石转换成普通钻石
---@return number 付费钻石 一般充值才有，付费钻石转换成普通钻石
function RefreshingResourcesMainResponse:getPremiumDiamondNum()
    return self.PremiumDiamondNum
end
--- 金币
---@return number 金币
function RefreshingResourcesMainResponse:getGoldNum()
    return self.goldNum
end


return RefreshingResourcesMainResponse
