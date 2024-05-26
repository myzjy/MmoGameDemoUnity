---@class EquipmentConfigBaseData
local EquipmentConfigBaseData = class("EquipmentConfigBaseData")
function EquipmentConfigBaseData:ctor()
    --- 强化到这个等级强化获取额外属性条或者升级附属性条
    ---@type number
    self.lv1 = 0
    --- 强化到这个等级强化获取额外属性条或者升级附属性条
    ---@type number
    self.lv2 = 0
    --- 强化到这个等级强化获取额外属性条或者升级附属性条
    ---@type number
    self.lv3 = 0
    --- 强化到这个等级强化获取额外属性条或者升级附属性条
    ---@type number
    self.lv4 = 0
    --- quality
    ---@type number
    self.quality = 0
end

---@param lv1 number 强化到这个等级强化获取额外属性条或者升级附属性条
---@param lv2 number 强化到这个等级强化获取额外属性条或者升级附属性条
---@param lv3 number 强化到这个等级强化获取额外属性条或者升级附属性条
---@param lv4 number 强化到这个等级强化获取额外属性条或者升级附属性条
---@param quality number quality
---@return EquipmentConfigBaseData
function EquipmentConfigBaseData:new(lv1, lv2, lv3, lv4, quality)
    self.lv1 = lv1 --- int
    self.lv2 = lv2 --- int
    self.lv3 = lv3 --- int
    self.lv4 = lv4 --- int
    self.quality = quality --- int
    return self
end

---@return number
function EquipmentConfigBaseData:protocolId()
    return 207
end

---@return string
function EquipmentConfigBaseData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            lv1 = self.lv1,
            lv2 = self.lv2,
            lv3 = self.lv3,
            lv4 = self.lv4,
            quality = self.quality
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return EquipmentConfigBaseData
function EquipmentConfigBaseData:read(data)

    local packet = self:new(
            data.lv1,
            data.lv2,
            data.lv3,
            data.lv4,
            data.quality)
    return packet
end

--- 强化到这个等级强化获取额外属性条或者升级附属性条
---@return number 强化到这个等级强化获取额外属性条或者升级附属性条
function EquipmentConfigBaseData:getLv1()
    return self.lv1
end
--- 强化到这个等级强化获取额外属性条或者升级附属性条
---@return number 强化到这个等级强化获取额外属性条或者升级附属性条
function EquipmentConfigBaseData:getLv2()
    return self.lv2
end
--- 强化到这个等级强化获取额外属性条或者升级附属性条
---@return number 强化到这个等级强化获取额外属性条或者升级附属性条
function EquipmentConfigBaseData:getLv3()
    return self.lv3
end
--- 强化到这个等级强化获取额外属性条或者升级附属性条
---@return number 强化到这个等级强化获取额外属性条或者升级附属性条
function EquipmentConfigBaseData:getLv4()
    return self.lv4
end
--- quality
---@return number quality
function EquipmentConfigBaseData:getQuality()
    return self.quality
end


return EquipmentConfigBaseData
