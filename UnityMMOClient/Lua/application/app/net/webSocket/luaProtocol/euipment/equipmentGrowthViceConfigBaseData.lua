---@class EquipmentGrowthViceConfigBaseData
local EquipmentGrowthViceConfigBaseData = class("EquipmentGrowthViceConfigBaseData")
local this = EquipmentGrowthViceConfigBaseData
function EquipmentGrowthViceConfigBaseData:ctor()
    ---@type number
    self.viceId = 0
    ---@type string
    self.viceName = string.empty
    ---@type number
    self.posGrowthType = 0
    ---@type table<integer,string>
    self.initNums = nil
end

---@param viceId number id
---@param viceName string 详细属性
---@param posGrowthType number 属性所属pos对应
---@param initNums table<integer,string>
function EquipmentGrowthViceConfigBaseData:new(viceId, viceName, posGrowthType, initNums)
    self.viceId = viceId
    self.viceName = viceName
    self.posGrowthType = posGrowthType
    self.initNums = initNums
    return self
end

function EquipmentGrowthViceConfigBaseData:protocolId()
    return 210
end

function EquipmentGrowthViceConfigBaseData:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or EquipmentGrowthViceConfigBaseData
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function EquipmentGrowthViceConfigBaseData:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {protocolId:number,packet:{viceId:number, viceName:string, posGrowthType:integer,initNums:table<integer,string>}}
    local data = JSON.decode(jsonString)
    return EquipmentGrowthViceConfigBaseData:new(
        data.packet.viceId,
        data.packet.viceName,
        data.packet.posGrowthType,
        data.packet.initNums
    )
end

function EquipmentGrowthViceConfigBaseData:readData(data)
    return EquipmentGrowthViceConfigBaseData:new(
        data.viceId,
        data.viceName,
        data.posGrowthType,
        data.initNums
    )
end

return EquipmentGrowthViceConfigBaseData
