--- 背包 top head button list
---  since: zjy
---@class BagHeaderBtnPanel :LuaUIObject
local BagHeaderBtnPanel = class("BagHeaderBtnPanel", LuaUIObject)
local BagHeaderItem = require("application.app.ui.bag.modelView.bagHeaderItem")
local BagBtnConfig = {
    { type = UIConfigEnum.BagHeaderBtnConfig.Type.WeaponType, icon = "UI_BagTabIcon_Weapon" }
}

---@param gameObject UnityEngine.GameObject
function BagHeaderBtnPanel:ctor(gameObject)
    self.gameObject = gameObject
    ---@type UnityEngine.GameObject
    self.headerGrids = LuaUtils.GetKeyGameObject(self.gameObject, "TabHeadListGrid")
    self.headersUIGrids = LuaUtils.GetUIGrid(self.gameObject, "TabHeadListGrid")
    self.bagScene = string.empty;
    -- 存放 list button
    self.bagButtonObjList = {}
    --- 当前 按钮配置 器
    self.bagConfig = {}
    -- 按钮
    ---@type UnityEngine.GameObject
    self.bagBtnObj = LuaUtils.GetKeyGameObject(self.gameObject, "headerBtn")
    self.bagBtnObj:SetActive(false)
end

function BagHeaderBtnPanel:CreateBagBtn()
    for index, value in ipairs(BagBtnConfig) do
        ---@type UnityEngine.GameObject
        local item = UnityEngine.GameObject:Instantiate(self.bagBtnObj)
        item.transform:SetParent(self.headerGrids.transform)
        LuaUtils.SetScale(item, UnityEngine.Vector3.New(1, 1, 1))
 
        self.bagButtonObjList[value.type] = BagHeaderItem(item, value)
    end
end

return BagHeaderBtnPanel
