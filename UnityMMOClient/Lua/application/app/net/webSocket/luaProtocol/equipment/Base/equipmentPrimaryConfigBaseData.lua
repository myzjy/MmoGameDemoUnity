---@class EquipmentPrimaryConfigBaseData
local EquipmentPrimaryConfigBaseData = class("EquipmentPrimaryConfigBaseData")
function EquipmentPrimaryConfigBaseData:ctor()
    --- 圣遗物属性位置
    ---@type number
    self.growthPosInt = 0
    --- 属性名字
    ---@type string
    self.growthPosName = string.empty
    --- id
    ---@type number
    self.id = 0
    --- 1级初始属性
    ---@type string
    self.primaryGrowthInts = string.empty
    --- 品阶的最大等级的最大属性值
    ---@type string
    self.primaryGrowthMaxInt = string.empty
    --- 属性位置所属名字
    ---@type string
    self.primaryGrowthName = string.empty
    --- 圣遗物品阶
    ---@type number
    self.primaryQuality = 0
end

---@param growthPosInt number 圣遗物属性位置
---@param growthPosName string 属性名字
---@param id number id
---@param primaryGrowthInts string 1级初始属性
---@param primaryGrowthMaxInt string 品阶的最大等级的最大属性值
---@param primaryGrowthName string 属性位置所属名字
---@param primaryQuality number 圣遗物品阶
---@return EquipmentPrimaryConfigBaseData
function EquipmentPrimaryConfigBaseData:new(growthPosInt, growthPosName, id, primaryGrowthInts, primaryGrowthMaxInt, primaryGrowthName, primaryQuality)
    self.growthPosInt = growthPosInt --- int
    self.growthPosName = growthPosName --- java.lang.String
    self.id = id --- int
    self.primaryGrowthInts = primaryGrowthInts --- java.lang.String
    self.primaryGrowthMaxInt = primaryGrowthMaxInt --- java.lang.String
    self.primaryGrowthName = primaryGrowthName --- java.lang.String
    self.primaryQuality = primaryQuality --- int
    return self
end

---@return number
function EquipmentPrimaryConfigBaseData:protocolId()
    return 208
end

---@return string
function EquipmentPrimaryConfigBaseData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            growthPosInt = self.growthPosInt,
            growthPosName = self.growthPosName,
            id = self.id,
            primaryGrowthInts = self.primaryGrowthInts,
            primaryGrowthMaxInt = self.primaryGrowthMaxInt,
            primaryGrowthName = self.primaryGrowthName,
            primaryQuality = self.primaryQuality
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return EquipmentPrimaryConfigBaseData
function EquipmentPrimaryConfigBaseData:read(data)

    local packet = self:new(
            data.growthPosInt,
            data.growthPosName,
            data.id,
            data.primaryGrowthInts,
            data.primaryGrowthMaxInt,
            data.primaryGrowthName,
            data.primaryQuality)
    return packet
end

--- 圣遗物属性位置
---@return number 圣遗物属性位置
function EquipmentPrimaryConfigBaseData:getGrowthPosInt()
    return self.growthPosInt
end
--- 属性名字
---@type  string 属性名字
function EquipmentPrimaryConfigBaseData:getGrowthPosName()
    return self.growthPosName
end

--- id
---@return number id
function EquipmentPrimaryConfigBaseData:getId()
    return self.id
end
--- 1级初始属性
---@type  string 1级初始属性
function EquipmentPrimaryConfigBaseData:getPrimaryGrowthInts()
    return self.primaryGrowthInts
end

--- 品阶的最大等级的最大属性值
---@type  string 品阶的最大等级的最大属性值
function EquipmentPrimaryConfigBaseData:getPrimaryGrowthMaxInt()
    return self.primaryGrowthMaxInt
end

--- 属性位置所属名字
---@type  string 属性位置所属名字
function EquipmentPrimaryConfigBaseData:getPrimaryGrowthName()
    return self.primaryGrowthName
end

--- 圣遗物品阶
---@return number 圣遗物品阶
function EquipmentPrimaryConfigBaseData:getPrimaryQuality()
    return self.primaryQuality
end


return EquipmentPrimaryConfigBaseData
