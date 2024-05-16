---@class BagWeaponUIPanelView
local BagWeaponUIPanelView = class("BagWeaponUIPanelView", LuaUIObject)
local weaponItemUIView = require("application.app.ui.bag.modelView.weaponItemUIView")
---@param viewPanel BagUIPanelView
function BagWeaponUIPanelView:ctor(viewPanel)
    ---@type table<number, WeaponPlayerUserDataStruct>
    self.weaponInfo = nil
    self.weaponPanelGroup = viewPanel.WeaponListGroup
    self.gameObject = viewPanel.weaponItem
    self.weaponItemObject = viewPanel.weaponItem
    self.uiGrid = viewPanel.weaponGrid
    --- 武器 list
    ---@type table<number,WeaponItemUIView>
    self.gridList = {}
    self:Open()
end
function BagWeaponUIPanelView:Open()
    LuaUtils.OpenOrCloseCanvasGroup(self.weaponPanelGroup, true)
end
function BagWeaponUIPanelView:Close()
    LuaUtils.OpenOrCloseCanvasGroup(self.weaponPanelGroup, false)
end
--- 传递武器数据创建 背包里面的 武器Item object
---@param weaponData WeaponPlayerUserDataStruct
---@return WeaponItemUIView
function BagWeaponUIPanelView:createWeaponItem(weaponData)
    ---@type UnityEngine.GameObject
    local itemObj = UnityEngine.GameObject.Instantiate(self.weaponItemObject) or UnityEngine.GameObject
    itemObj.transform:SetParent(self.uiGrid.transform, false)
    itemObj.transform.localScale = Vector3.one
    local obj = weaponItemUIView()
    obj:Init(itemObj, weaponData)
    return obj
end
--- 创建武器 物体
---@param weaponItemList table<number, WeaponPlayerUserDataStruct>
function BagWeaponUIPanelView:CreateItemList(weaponItemList)
    if Debug > 0 then
        PrintDebug("创建武器")
        dump(weaponItemList)
    end
    self.weaponInfo = weaponItemList
    local list = self.weaponInfo
    for i = 1, #list do
        local item = list[i]
        if self.gridList[item.id] == nil then
            local obj = self:createWeaponItem(item)

            self.gridList[item.id] = obj;
        else
            self.gridList[item.id]:RefreshUI(item)
        end

    end
end

--- 刷新
---@param weaponData table<number,WeaponPlayerUserDataStruct>
function BagWeaponUIPanelView:RefreshUIPanel(weaponData)
    if Debug > 0 then
        PrintDebug("添加或者删除武器")
        dump(weaponData)
    end
    for _, v in pairs(weaponData) do
        local item = v
        if self.gridList[v.id] == nil then
            local obj = self:createWeaponItem(item)
            self.gridList[item.id] = obj;
        else
            self.gridList[item.id]:RefreshUI(item)
        end
    end
end

function BagWeaponUIPanelView:OnDestroy()

    -- table.Clear(self.gridList)
    -- 不需要删除 只需要 制空
    self.gameObject = nil
    table.Clear(self.gridList)
    self.gridList = {}
end

return BagWeaponUIPanelView
