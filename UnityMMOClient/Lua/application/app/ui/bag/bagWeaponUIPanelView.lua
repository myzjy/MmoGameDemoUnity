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
    self.gridList = {}
    self:Open()
end
function BagWeaponUIPanelView:Open()
    LuaUtils.OpenOrCloseCanvasGroup(self.weaponPanelGroup, true)
end
function BagWeaponUIPanelView:Close()
    LuaUtils.OpenOrCloseCanvasGroup(self.weaponPanelGroup, false)
end

---@param weaponItemList table<number, WeaponPlayerUserDataStruct>
function BagWeaponUIPanelView:CreateItemList(weaponItemList)
    self.weaponInfo = weaponItemList
    local list = self.weaponInfo
    for i = 1, #list do
        local item = list[i]
        ---@type UnityEngine.GameObject
        local itemObj = UnityEngine.GameObject.Instantiate(self.weaponItemObject) or UnityEngine.GameObject
        itemObj.transform:SetParent(self.uiGrid.transform, false)
        itemObj.transform.localScale = Vector3.one
        local obj = weaponItemUIView()
        obj:Init(itemObj, item)
        table.insert(self.gridList, obj)
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
