--- 获取背包里面的所有武器
---@class WeaponPlayerBagDataRequest
local WeaponPlayerBagDataRequest = class("WeaponPlayerBagDataRequest")

---初始化 相关
function WeaponPlayerBagDataRequest:ctor()
    ---@type number
    self.serverId = 0;
end

---@param serverId number 
function WeaponPlayerBagDataRequest:new(serverId)

end

return WeaponPlayerBagDataRequest
