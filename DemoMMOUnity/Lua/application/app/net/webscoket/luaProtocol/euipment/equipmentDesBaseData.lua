---@class EquipmentDesBaseData
local EquipmentDesBaseData = class("EquipmentDesBaseData")
local this = EquipmentDesBaseData
function EquipmentDesBaseData:ctor()
    ---@type number
    self.desId = 0
    ---@type string
    self.name = string.empty
    ---@type string
    self.desStr = string.empty
    ---@type string
    self.storyDesStr = string.empty
end

---@param desId number 介绍id
---@param name string 名字
---@param desStr string 介绍
---@param storyDesStr string 故事
function EquipmentDesBaseData:new(desId, name, desStr, storyDesStr)
    self.desId = desId
    self.name = name
    self.desStr = desStr
    self.storyDesStr = storyDesStr

    return self
end

function EquipmentDesBaseData:protocolId()
    return 212
end

function EquipmentDesBaseData:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or EquipmentDesBaseData
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function EquipmentDesBaseData:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {protocolId:number,packet:{desId:number,name:string,desStr:string,toryDesStr:string}}
    local data = JSON.decode(jsonString)
    return EquipmentDesBaseData:new(
        data.packet.desId,
        data.packet.name,
        data.packet.desStr,
        data.packet.toryDesStr
    )
end

function EquipmentDesBaseData:readData(data)

    return EquipmentDesBaseData:new(
        data.desId,
        data.name,
        data.desStr,
        data.toryDesStr
    )
end

return EquipmentDesBaseData
