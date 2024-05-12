---武器相关 type
local WeaponSortType
WeaponSortType = function()
    return {
        qualitySortTpe = "qualitySortTpe"
    }
end

--- 背包UI model 数据相关 背包内数据操作
---@class BagModelUIView
local BagModelUIView = class("BagModelUIView")


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
--- 内部指定 武器 entity
local weaponList = {}
function BagModelUIView:SetAllWeaponDataList(weaponEntityList)
    weaponList = weaponEntityList
end

---清空 换成相关 在退出界面之后 清空
function BagModelUIView:OnHideClearCacheDataList()
    weaponList = {}
end

return BagModelUIView
