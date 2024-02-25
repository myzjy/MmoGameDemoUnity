---武器
---[[
---方法
--- RegisterEvent  注册 事件
--- sendAllWeaponServer [[
--- param:
--------- findUserId   查找 userID
--------- findWeaponId 需要查找的 武器id
---]]
---]]

local WeaponNetController = class("WeaponNetController")

function WeaponNetController:RegisterEvent()
end

---@param findUserId number
---@param findWeaponId number
function WeaponNetController:sendAllWeaponServer(findUserId, findWeaponId)
    local packetData = WeaponPlayerUserDataRequest
    if packetData == nil then
        -- body
        printError("当前 WeaponPlayerUserDataRequest lua 侧没有读取到 检查文件")
        return
    end
    local data = packetData:new(findUserId, findWeaponId)
    local buffer = ByteBuffer:new()
    packetData:write()
    NetManager:SendMessageEvent(buffer:readString())
end

return WeaponNetController
