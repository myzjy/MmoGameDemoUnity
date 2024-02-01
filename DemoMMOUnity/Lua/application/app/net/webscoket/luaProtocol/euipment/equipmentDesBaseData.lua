---@class EquipmentDesBaseData
local EquipmentDesBaseData = {}


---@param desId number 介绍id
---@param name string 名字
---@param desStr string 介绍
---@param storyDesStr string 故事
function EquipmentDesBaseData:new(desId, name, desStr,storyDesStr)
    local obj = {
        desId = desId,
        name = name,
        desStr = desStr,
        storyDesStr = storyDesStr

    }
    setmetatable(obj, self)
    self.__index = self
    return obj
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
function EquipmentDesBaseData:readData(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {desId:number,name:string,desStr:string,toryDesStr:string}
    local data = JSON.decode(jsonString)
    return EquipmentDesBaseData:new(
        data.desId,
        data.name,
        data.desStr,
        data.toryDesStr
    )
end

return EquipmentDesBaseData
