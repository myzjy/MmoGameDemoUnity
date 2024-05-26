---@class EquipmentGrowthViceConfigBaseData
local EquipmentGrowthViceConfigBaseData = class("EquipmentGrowthViceConfigBaseData")
function EquipmentGrowthViceConfigBaseData:ctor()
    --- 副属性的初始值数组
    ---@type  table<number,string>
    self.initNums = {}
    --- 属性所属pos对应
    ---@type number
    self.posGrowthType = 0
    --- id
    ---@type number
    self.viceId = 0
    --- 详细属性
    ---@type string
    self.viceName = string.empty
end

---@param initNums table<number,string> 副属性的初始值数组
---@param posGrowthType number 属性所属pos对应
---@param viceId number id
---@param viceName string 详细属性
---@return EquipmentGrowthViceConfigBaseData
function EquipmentGrowthViceConfigBaseData:new(initNums, posGrowthType, viceId, viceName)
    self.initNums = initNums --- java.util.List<java.lang.String>
    self.posGrowthType = posGrowthType --- int
    self.viceId = viceId --- int
    self.viceName = viceName --- java.lang.String
    return self
end

---@return number
function EquipmentGrowthViceConfigBaseData:protocolId()
    return 210
end

---@return string
function EquipmentGrowthViceConfigBaseData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            initNums = self.initNums,
            posGrowthType = self.posGrowthType,
            viceId = self.viceId,
            viceName = self.viceName
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return EquipmentGrowthViceConfigBaseData
function EquipmentGrowthViceConfigBaseData:read(data)
    local initNums = {}
    for index, value in ipairs(data.initNums) do
        local packetData = value
        table.insert(initNums,packetData)
    end

    local packet = self:new(
            initNums,
            data.posGrowthType,
            data.viceId,
            data.viceName)
    return packet
end

--- 副属性的初始值数组
---@type  table<number,string> 副属性的初始值数组
function EquipmentGrowthViceConfigBaseData:getInitNums()
    return self.initNums
end
--- 属性所属pos对应
---@return number 属性所属pos对应
function EquipmentGrowthViceConfigBaseData:getPosGrowthType()
    return self.posGrowthType
end
--- id
---@return number id
function EquipmentGrowthViceConfigBaseData:getViceId()
    return self.viceId
end
--- 详细属性
---@type  string 详细属性
function EquipmentGrowthViceConfigBaseData:getViceName()
    return self.viceName
end



return EquipmentGrowthViceConfigBaseData
