---@class BagHeaderItem:LuaUIObject
local BagHeaderItem = class("BagHeaderItem", LuaUIObject())


---@param goRoot any
---@param config {type:number,icon:string, name:string, openConfig:{ Open:string,Hide:string}}
---@param prefabConfig {__hiderIcon:string,__openIcon:string,__headerBtnObj:string,__headerBtnSelectObj:string}
function BagHeaderItem:ctor(goRoot, config, prefabConfig)
    self.gameObject = goRoot
    self.transform = goRoot.transform
    self.config = config
    self.prefabConfig = prefabConfig
    self.openObj = LuaUtils.GetKeyCanvaGroupGameObject(self.gameObject, self.prefabConfig.__headerBtnSelectObj)
    self.hideObj = LuaUtils.GetKeyCanvaGroupGameObject(self.gameObject, self.prefabConfig.__headerBtnObj)
    self.openBtn = LuaUtils.GetKeyButtonGameObject(self.gameObject, self.prefabConfig.__headerBtnSelectObj)
    self.hideBtn = LuaUtils.GetKeyButtonGameObject(self.gameObject, self.prefabConfig.__headerBtnObj)
    self.isSelectClick = false;
    self:OnHide()

    self:SetListener(self.openBtn, function()
        self:OnEnterFish()
    end)
end

--- 点击事件
function BagHeaderItem:OnEnterFish()
    if self.config.type == 1 then
        self:OnOpen();
        -- 点击 背包 武器按钮
        UIGameEvent.BagHeaderBtnWeaponHandler()
    end
end

function BagHeaderItem:OnOpen()
    self.openObj.alpha = 1
    self.hideObj.alpha = 0
    self.isSelectClick = true;
end

function BagHeaderItem:OnHide()
    self.openObj.alpha = 0
    self.hideObj.alpha = 1
end

return BagHeaderItem
