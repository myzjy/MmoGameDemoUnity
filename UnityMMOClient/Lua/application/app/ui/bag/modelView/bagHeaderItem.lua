---@class BagHeaderItem:LuaUIObject
local BagHeaderItem = class("BagHeaderItem", LuaUIObject())

function BagHeaderItem:ctor(goRoot, config, prefabConfig)
    self.gameObject = goRoot
    self.transform = goRoot.transform
    self.config = config
    self.prefabConfig = prefabConfig
    self.openObj=LuaUtils.GetUICanvaGroup(self.gameObject,self.prefabConfig.__headerBtnSelectObj)
    self.hideObj=LuaUtils.GetUICanvaGroup(self.gameObject,self.prefabConfig.__headerBtnObj)
end

--- 点击事件
function BagHeaderItem:OnEnterFish()
    if self.config.type == 1 then
        self:OnClickWeaponBtnHandler()
    end
end

function BagHeaderItem:OnClickWeaponBtnHandler()
    -- 点击 背包 武器按钮
    UIGameEvent.BagHeaderBtnWeaponHandler()
end

return BagHeaderItem
