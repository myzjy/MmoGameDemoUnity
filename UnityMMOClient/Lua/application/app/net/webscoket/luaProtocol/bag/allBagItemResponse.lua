---@class AllBagItemResponse
local AllBagItemResponse = class("AllBagItemResponse")

function AllBagItemResponse:ctor()
    ---@type table<integer,BagUserItemMsgData>
    self.list = nil
end
function AllBagItemResponse:protocolId()
    return 1007
end

function AllBagItemResponse:read(data)
    local id = data.protocolId
    local packet = data.packet

    for _index = 1, #packet.list do
        local forData = packet.list[_index]
        ---获取到自己
        local packetData = BagUserItemMsgData()
        table.insert(self.list, packetData:read(forData))
    end
    return self
end


return AllBagItemResponse
