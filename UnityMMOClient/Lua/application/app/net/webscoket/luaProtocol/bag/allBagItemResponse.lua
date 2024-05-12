---@class AllBagItemResponse
local AllBagItemResponse = class("AllBagItemResponse")

function AllBagItemResponse:ctor()
    ---@type table<number,WeaponPlayerUserDataStruct>
    self.weaponUserList = {}
    ---@type string 额外协议号
    self.protocolStr = string.empty
end

function AllBagItemResponse:protocolId()
    return 1008
end
--- 读取数据创建 response
--- @param data any
--- @return AllBagItemResponse
function AllBagItemResponse:read(data)
    local packet = data
    self.protocolStr = packet.protocolStr
    for _index = 1, #packet.list do
        local forData = packet.list[_index]
        ---获取到自己
        local packetData = WeaponPlayerUserDataStruct()
        table.insert(self.weaponUserList, packetData:read(forData))
    end
    return self
end

return AllBagItemResponse