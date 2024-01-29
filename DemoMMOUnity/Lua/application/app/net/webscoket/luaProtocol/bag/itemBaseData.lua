---@class ItemBaseData
local ItemBaseData = {}



---@param id integer
---@param name string
---@param icon string
---@param minNum integer
---@param maxNum integer
---@param type integer
---@param des string
function ItemBaseData:new(id,
                          name,
                          icon,
                          minNum,
                          maxNum,
                          type,
                          des)
    local obj = {
        id = id,
        name = name,
        icon = icon,
        minNum = minNum,
        maxNum = maxNum,
        type = type,
        des = des
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end

function ItemBaseData:protocolId()
    return 201
end

function ItemBaseData:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or ItemBaseData
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function ItemBaseData:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {protocolId:number,packet:{id:integer,name:string,icon:string,minNum:integer,maxNum:integer,type:integer,des:string}}
    local data = JSON.decode(jsonString)
    return ItemBaseData:new(
        data.packet.id,
        data.packet.name,
        data.packet.icon,
        data.packet.minNum,
        data.packet.maxNum,
        data.packet.type,
        data.packet.des)
end

return ItemBaseData
