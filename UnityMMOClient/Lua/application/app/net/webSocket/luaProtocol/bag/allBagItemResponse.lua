---``` lua
--- weaponUserList ---- 武器 数据
--- ctor()         ----初始 初始化 内部需要用到相关 值
--- protocolId()   ---- 协议id
--- red(data)          ---- 传递过来的 data 数据 赋值具体字段
---```
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
    for _index = 1, #packet.weaponUserList do
        local forData = packet.weaponUserList[_index]
        ---获取到自己
        local packetData = WeaponPlayerUserDataStruct()
        table.insert(self.weaponUserList, packetData:readData(forData))
    end
    return self
end

return AllBagItemResponse