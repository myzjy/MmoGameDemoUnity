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
            type = UIConfig.BagHeaderBtnConfig.Type.WeaponType,
            icon = "UI_BagTabIcon_Weapon",
            name = UIConfig.BagHeaderBtnConfig.Name,
            openConfig = UIConfig.BagHeaderBtnConfig.IconConfig
        },
    }
}

---@param gameObject UnityEngine.GameObject
function BagHeaderBtnPanel:ctor(gameObject)
    self.gameObject = gameObject
    ---@type UnityEngine.GameObject
    self.headerGrids = LuaUtils.GetKeyGameObject(self.gameObject, "TabHeadList")
    self.headersUIGrids = LuaUtils.GetKeyUIGrid(self.gameObject, "TabHeadList")
    self.bagScene = string.empty;
    -- 存放 list button
    ---@type table<number,BagHeaderItem>
    self.bagButtonObjList = {}
    --- 当前 按钮配置 器
    self.bagConfig = {}
    -- 按钮
    ---@type UnityEngine.GameObject
    self.bagBtnObj = LuaUtils.GetKeyGameObject(self.gameObject, "headerBtnButtons")
    self:DeleteBagHeaderBtn()
    self:CreateBagBtn()
    self.bagBtnObj:SetActive(false)
    self:RegisterEvent()
end

function BagHeaderBtnPanel:RegisterEvent()
    PrintDebug("init registerEvent add bagHeaderBtnPanel function")
    UIUtils.AddEventListener(UIGameEvent.OnSelectBagHeaderBtnHandler, self.OnSelectHeaderBtn, self)
    UIUtils.AddEventListener(UIGameEvent.CreateBagHeaderBtnHandler, self.CreateBagBtn, self)
    UIUtils.AddEventListener(UIGameEvent.DeleteBagHeaderBtnHandler, self.DeleteBagHeaderBtn, self)
end

function BagHeaderBtnPanel:CreateBagBtn()
    PrintDebug("call init create BagHeader Btn function not delete gameObject list")
    for index, value in ipairs(BagBtnConfig.BagHeaderBtnConfig) do
        ---@type UnityEngine.GameObject
        local item = UnityEngine.GameObject.Instantiate(self.bagBtnObj)
        item.transform:SetParent(self.headerGrids.transform)
        LuaUtils.SetScale(item, UnityEngine.Vector3.New(1, 1, 1))

        self.bagButtonObjList[value.type] = BagHeaderItem(item, value, BagBtnConfig.PrefabConfig)
    end
end

function BagHeaderBtnPanel:DeleteBagHeaderBtn()
    PrintDebug("call init delte bagHeader Btn function is delete gameObject childs")
    for index, value in ipairs(self.bagButtonObjList) do
        value:OnDestroy();
    end
    self.bagButtonObjList = {}
end

--- 选择了其他 按钮
---@param __selectIndex any
function BagHeaderBtnPanel:OnSelectHeaderBtn(__selectIndex)
    PrintDebug("select click event handler bagHeader")
    for index, value in ipairs(self.bagButtonObjList) do
        if __selectIndex ~= index then
            value:OnHide()
        else
            -- 明确点击的按钮
            value:OnEnterFish()
        end
    end
end

function BagHeaderBtnPanel:OnDestroy()
    UIGameEvent.DeleteBagHeaderBtnHandler()
    UIUtils.RemoveAllEventListener(self)
end

return BagHeaderBtnPanel