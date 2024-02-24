---@class EquipmentConfigBaseData
local EquipmentConfigBaseData = class("EquipmentConfigBaseData")
local this = EquipmentConfigBaseData
function EquipmentConfigBaseData:ctor()
    ---@type number
    self.quality = 0
    ---@type number
    self.lv1 = 0
    ---@type number
    self.lv2 = 0
    ---@type number
    self.lv3 = 0
    ---@type number
    self.lv4 = 0
end

---@param quality number 品阶
---@param lv1 number 强化到这个等级 强化获取额外属性条或者升级附属性条
---@param lv2 number 强化到这个等级 强化获取额外属性条或者升级附属性条
---@param lv3 number 强化到这个等级 强化获取额外属性条或者升级附属性条
---@param lv4 number 强化到这个等级 强化获取额外属性条或者升级附属性条
function EquipmentConfigBaseData:new(quality, lv1, lv2, lv3, lv4)
    self.quality = quality
    self.lv1 = lv1
    self.lv2 = lv2
    self.lv3 = lv3
    self.lv4 = lv4

    return self
end

function EquipmentConfigBaseData:protocolId()
    return 207
end

function EquipmentConfigBaseData:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or EquipmentConfigBaseData
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function EquipmentConfigBaseData:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {protocolId:number,packet:{quality:number, lv1:number, lv2:number, lv3:number, lv4:number}}
    local data = JSON.decode(jsonString)
    return EquipmentConfigBaseData:new(
        data.packet.quality,
        data.packet.lv1,
        data.packet.lv2,
        data.packet.lv3,
        data.packet.lv4
    )
end

function EquipmentConfigBaseData:readData(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {quality:number, lv1:number, lv2:number, lv3:number, lv4:number}
    local data = JSON.decode(jsonString)
    return EquipmentConfigBaseData:new(
        data.quality,
        data.lv1,
        data.lv2,
        data.lv3,
        data.lv4
    )
end

return EquipmentConfigBaseData
