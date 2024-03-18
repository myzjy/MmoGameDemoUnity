--- 背包 top head button list
---  since: zjy
---@class BagHeaderBtnPanel :LuaUIObject
local BagHeaderBtnPanel = class("BagHeaderBtnPanel", LuaUIObject)
local BagBtnConfig = {
    { type = UIConfigEnum.BagHeaderBtnConfig.Type.WeaponType, icon = "UI_BagTabIcon_Weapon" }
}

---@param gameObject UnityEngine.GameObject
function BagHeaderBtnPanel:ctor(gameObject)
    self.gameObject = gameObject
    ---@type UnityEngine.GameObject
    self.headerGrids = LuaUtils.GetGameObject(self.gameObject, "TabHeadListGrid")
    self.headersUIGrids = LuaUtils.GetUIGrid(self.gameObject, "TabHeadListGrid")
    self.bagScene = string.empty;
    -- 存放 list button
    self.bagButtonObjList = {}
    --- 当前 按钮配置 器
    self.bagConfig = {}
    -- 按钮
    self.bagBtnObj = LuaUtils.GetGameObject(self.gameObject, "headerBtn")
    self:SetItemActive(self.bagBtnObj, false)
end

function BagHeaderBtnPanel:CreateBagBtn()
    for index, value in ipairs(BagBtnConfig) do
        ---@type UnityEngine.GameObject
        local item = UnityEngine.GameObject:Instantiate(self.bagBtnObj)
        item.transform:SetParent(self.headerGrids.transform)
    end
end

return BagHeaderBtnPanel
