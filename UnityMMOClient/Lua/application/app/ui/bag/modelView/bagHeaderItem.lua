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
    self.openObj = LuaUtils.GetKeyCanvasGroupGameObject(self.gameObject, self.prefabConfig.__headerBtnSelectObj)
    self.hideObj = LuaUtils.GetKeyCanvasGroupGameObject(self.gameObject, self.prefabConfig.__headerBtnObj)
    self.openBtn = LuaUtils.GetKeyButtonGameObject(self.gameObject, self.prefabConfig.__headerBtnSelectObj)
    self.hideBtn = LuaUtils.GetKeyButtonGameObject(self.gameObject, self.prefabConfig.__headerBtnObj)
    self.isSelectClick = false;
    self:OnHide()

    self:SetListener(self.openBtn, function()
        self:OnEnterFish()
    end)
    self:SetListener(self.hideBtn, function()
        self:OnEnterFish()
    end)
end

--- 点击事件
function BagHeaderItem:OnEnterFish()
    PrintDebug("click bag header item enterFish")
    -- 不应徐 重复进入
    -- if self.isSelectClick then
    --     return
    -- end
    if self.config.type == 1 then
        self:OnOpen();
        -- 点击 背包 武器按钮
        GameEvent.ClickBagHeaderBtnHandlerServer(self.config.type,"c001")
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
    self.isSelectClick = false;
end

function BagHeaderItem:OnDestroy()
    self.transform = nil
    self.config = nil
    self.prefabConfig = nil
    self.openObj = nil
    self.hideObj = nil
    self.openBtn = nil
    self.hideBtn = nil
    self.isSelectClick = nil;
    UnityEngine.GameObject.Destroy(self.gameObject)
end

return BagHeaderItem
