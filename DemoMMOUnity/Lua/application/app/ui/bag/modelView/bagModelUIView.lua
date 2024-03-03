BagUIConfig = {
    prefabName = "BagUIPanel",
    --- 当前会生成在那一层
    canvasType = UIConfigEnum.UICanvasType.UI,
    sortType = UIConfigEnum.UISortType.First,
    --- 当前 UI 交互事件 消息
    eventNotification = {
        --- 打开游戏主界面
        openbaguipanel = "openbaguipanel",
        --- 关闭 游戏主界面
        closebaguipanel = "closebaguipanel",
    },
    weaponIconAtlasName="uibagweaponicon_spriteatlas"
}
---武器相关 type
local WeaponSortType
WeaponSortType = function()
    return {
        qualitySortTpe = "qualitySortTpe"
    }
end

--- 背包UI model 数据相关 背包内数据操作
BagModelUIView = class("BagModelUIView")

function BagModelUIView:GetInstance()
    return BagModelUIView
end

local function weaponQualitySortTpe(list)

end
--- 通过相关type 针对 武器 list 进行排序
local function sortWeaponList(list, type)
    local sotrSwitchEvent = {
        [WeaponSortType().qualitySortTpe] = function(list)
            return weaponQualitySortTpe(type)
        end
    }
    return list
end
--- 内部指定 武器 enetiy
local weaponList = {}
function BagModelUIView:SetAllWeaponDataList(weaponEnetiyList)
    weaponList = weaponEnetiyList
end

---清空 换成相关 在退出界面之后 清空
function BagModelUIView:OnHideClearCacheDataList()
    weaponList = {}
end
