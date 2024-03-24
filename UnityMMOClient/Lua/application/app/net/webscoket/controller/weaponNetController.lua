---武器
---[[
---方法
--- RegisterEvent  注册 事件
--- sendAllWeaponServer [[
--- param:
--------- findUserId   查找 userID
--------- findWeaponId 需要查找的 武器id
--- field:
--------- RegisterEvent 注册事件 func
---]]
---]]

---@class WeaponNetController
local WeaponNetController = class("WeaponNetController")
---@type WeaponNetController
local instance = nil
function WeaponNetController:GetInstance()
    if not instance then
        instance = WeaponNetController()
    end
    return instance
end
function WeaponNetController:RegisterEvent()
    UIUtils.AddEventListener(GameEvent.AcquireUserIdWeaponService, self.AcquireUserIdWeaponService, self)
end

--- 根据 findUserId 服务器查找到对应的玩家的武器
---@param findUserId number
---@param findWeaponId number
function WeaponNetController:AcquireUserIdWeaponService(findUserId, findWeaponId)
    local packetData = WeaponPlayerUserDataRequest()
    if packetData == nil then
        -- body
        PrintError("当前 WeaponPlayerUserDataRequest lua 侧没有读取到 检查文件")
        return
    end
    local data = packetData:new(findUserId, findWeaponId)

    local jsonString = data:write()
    NetManager:SendMessageEvent(jsonString)
end
function WeaponNetController:AcquireWeaponBagServerList()

end

return WeaponNetController
