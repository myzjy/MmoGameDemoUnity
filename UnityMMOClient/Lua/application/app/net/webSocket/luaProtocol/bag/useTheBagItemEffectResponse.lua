---@class UseTheBagItemEffectResponse
local UseTheBagItemEffectResponse = class("UseTheBagItemEffectResponse")
function UseTheBagItemEffectResponse:ctor()
    --- 玩家背包数据使用
    ---@type  table<number,BagUserItemData>
    self.bagUserItemDataList = {}
end

---@param bagUserItemDataList table<number,BagUserItemData> 玩家背包数据使用
---@return UseTheBagItemEffectResponse
function UseTheBagItemEffectResponse:new(bagUserItemDataList)
    self.bagUserItemDataList = bagUserItemDataList --- java.util.List<com.gameServer.common.protocol.bag.BagUserItemData>
    return self
end

---@return number
function UseTheBagItemEffectResponse:protocolId()
    return 1034
end

---@return string
function UseTheBagItemEffectResponse:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            bagUserItemDataList = self.bagUserItemDataList
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return UseTheBagItemEffectResponse
function UseTheBagItemEffectResponse:read(data)
    local bagUserItemDataList = {}
    for index, value in ipairs(data.bagUserItemDataList) do
        local bagUserItemDataListPacket = BagUserItemData()
        local packetData = bagUserItemDataListPacket:read(value)
        table.insert(bagUserItemDataList,packetData)
    end

    local packet = self:new(
            bagUserItemDataList)
    return packet
end

--- 玩家背包数据使用
---@type  table<number,BagUserItemData> 玩家背包数据使用
function UseTheBagItemEffectResponse:getBagUserItemDataList()
    return self.bagUserItemDataList
end


return UseTheBagItemEffectResponse
