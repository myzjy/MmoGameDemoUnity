---@class AllBagItemResponse
local AllBagItemResponse = class("AllBagItemResponse")

function AllBagItemResponse:ctor()
    ---@type table<number,BagUserItemMsgData>
    self.weaponUserList = {}
    ---@type string 额外协议号
    self.protocolStr = string.empty
end

function AllBagItemResponse:protocolId()
    return 1008
end

function AllBagItemResponse:read(data)
    local packet = data
    self.protocolStr = packet.protocolStr
    for _index = 1, #packet.list do
        local forData = packet.list[_index]
        ---获取到自己
        local packetData = BagUserItemMsgData()
        table.insert(self.weaponUserList, packetData:read(forData))
    end
    return self
end

return AllBagItemResponse