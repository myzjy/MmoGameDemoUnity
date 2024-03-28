---@class BagWeaponUIPanelView
local BagWeaponUIPanelView = class("BagWeaponUIPanelView", LuaUIObject)
local weaponItemUIView = require("application.app.ui.bag.modelView.weaponItemUIView")
---@param weaponInfo AllBagItemResponse
---@param goConfig {gameObject:UnityEngine.GameObject,itemObj:UnityEngine.GameObject}
function BagWeaponUIPanelView:ctor(weaponInfo, goConfig)
    self.weaponInfo = weaponInfo
    self.gameObject = goConfig.gameObject
    self.weaponItemObject = goConfig.itemObj
    self.uiGrid = LuaUtils.GetKeyUIGrid(self.gameObject, "grid")
    self.gridList = {}
end

function BagWeaponUIPanelView:CreateItemList()
    local list = self.weaponInfo.list
    for i = 1, #list do
        local item = list[i]
        ---@type UnityEngine.GameObject
        local itemObj = UnityEngine.GameObject.Instantiate(self.weaponItemObject) or UnityEngine.GameObject
        itemObj.transform:SetParent(self.uiGrid.transform, false)
        itemObj.transform.localScale = Vector3.one
        local obj = weaponItemUIView(itemObj)
        obj:Init()
        table.insert(self.gridList, obj)
    end
end

return BagWeaponUIPanelView
