---@class PuzzleRewardsData
local PuzzleRewardsData = class("PuzzleRewardsData")
function PuzzleRewardsData:ctor()
    --- 奖励数量
    ---@type number
    self.num = 0
    --- 奖励icon
    ---@type string
    self.rewardIcon = string.empty
    --- 奖励的物品id
    ---@type number
    self.rewardId = 0
    --- 奖励资源
    ---@type string
    self.rewardResource = string.empty
    --- 奖励type
    ---@type number
    self.rewardType = 0
end

---@param num number 奖励数量
---@param rewardIcon string 奖励icon
---@param rewardId number 奖励的物品id
---@param rewardResource string 奖励资源
---@param rewardType number 奖励type
---@return PuzzleRewardsData
function PuzzleRewardsData:new(num, rewardIcon, rewardId, rewardResource, rewardType)
    self.num = num --- int
    self.rewardIcon = rewardIcon --- java.lang.String
    self.rewardId = rewardId --- int
    self.rewardResource = rewardResource --- java.lang.String
    self.rewardType = rewardType --- int
    return self
end

---@return number
function PuzzleRewardsData:protocolId()
    return 203
end

---@return string
function PuzzleRewardsData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            num = self.num,
            rewardIcon = self.rewardIcon,
            rewardId = self.rewardId,
            rewardResource = self.rewardResource,
            rewardType = self.rewardType
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return PuzzleRewardsData
function PuzzleRewardsData:read(data)

    local packet = self:new(
            data.num,
            data.rewardIcon,
            data.rewardId,
            data.rewardResource,
            data.rewardType)
    return packet
end

--- 奖励数量
---@return number 奖励数量
function PuzzleRewardsData:getNum()
    return self.num
end
--- 奖励icon
---@type  string 奖励icon
function PuzzleRewardsData:getRewardIcon()
    return self.rewardIcon
end

--- 奖励的物品id
---@return number 奖励的物品id
function PuzzleRewardsData:getRewardId()
    return self.rewardId
end
--- 奖励资源
---@type  string 奖励资源
function PuzzleRewardsData:getRewardResource()
    return self.rewardResource
end

--- 奖励type
---@return number 奖励type
function PuzzleRewardsData:getRewardType()
    return self.rewardType
end


return PuzzleRewardsData
