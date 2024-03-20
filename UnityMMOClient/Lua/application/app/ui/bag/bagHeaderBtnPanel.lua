--- 背包 top head button list
---  since: zjy
---@class BagHeaderBtnPanel :LuaUIObject
local BagHeaderBtnPanel = class("BagHeaderBtnPanel", LuaUIObject)
local BagHeaderItem = require("application.app.ui.bag.modelView.bagHeaderItem")
local BagBtnConfig = {
    PrefabConfig = {
        __hiderIcon = "headerBtnIconHide",
        __openIcon = "headerBtnOpenIcon",
        __headerBtnObj = "headerBtnObj",
        __headerBtnSelectObj = "headerBtnSelectObj"
    },
    BagHeaderBtnConfig = {
        {
            type = UIConfigEnum.BagHeaderBtnConfig.Type.WeaponType,
            icon = "UI_BagTabIcon_Weapon",
            name = UIConfigEnum.BagHeaderBtnConfig.Name,
            openConfig = UIConfigEnum.BagHeaderBtnConfig.IconConfig
        },
    }
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
    for index, value in ipairs(BagBtnConfig.BagHeaderBtnConfig) do
        ---@type UnityEngine.GameObject
        local item = UnityEngine.GameObject:Instantiate(self.bagBtnObj)
        item.transform:SetParent(self.headerGrids.transform)
        LuaUtils.SetScale(item, UnityEngine.Vector3.New(1, 1, 1))

        self.bagButtonObjList[value.type] = BagHeaderItem(item, value,BagBtnConfig.PrefabConfig)
    end
end

return BagHeaderBtnPanel
