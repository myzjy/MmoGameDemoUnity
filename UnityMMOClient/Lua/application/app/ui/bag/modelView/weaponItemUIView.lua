---@class WeaponItemUIView
local WeaponItemUIView = class("WeaponItemUIView", LuaUIObject)
function WeaponItemUIView:ctor()
    ---@type UnityEngine.GameObject
    self.gameObject = nil
    ---@type ZJYFrameWork.UISerializable.UISerializableKeyObject
    self.uSKeyObject = nil
    ---@type UnityEngine.CanvasGroup
    self.itemCanvasGroup = nil
    ---@type UnityEngine.UI.Image
    self.itemBgImage = nil
    ---@type UnityEngine.UI.Image
    self.itemIcon = nil
    ---@type UnityEngine.UI.Text
    self.itemNumText = nil
    ---@type UnityEngine.UI.Button
    self.clickBtn = nil
end

---@param thisKeyObject UnityEngine.GameObject
---@param weaponData BagUserItemMsgData
function WeaponItemUIView:Init(thisKeyObject, weaponData)
    self.gameObject = thisKeyObject
    self.gameObject:SetActive(true)
    self.uSKeyObject = self.gameObject:GetComponent("UISerializableKeyObject")
    self.itemCanvasGroup = LuaUtils.GetUIKeyCanvasGroup(self.uSKeyObject, "itemCanvasGroup")
    self.itemBgImage = LuaUtils.GetUISerializableKeyImage(self.uSKeyObject, "itemBgImage")
    self.itemIcon = LuaUtils.GetUISerializableKeyImage(self.uSKeyObject, "itemIcon")
    self.itemNumText = LuaUtils.GetUISerializableKeyText(self.uSKeyObject, "itemNumText")
    self.clickBtn = LuaUtils.GetUISerializableKeyButton(self.uSKeyObject, "clickBtn")
    LuaUIObject:SetListener(self.clickBtn, handle(self.clickEvent, self))
    self.itemCanvasGroup.alpha = 1
    -- GetAssetBundleManager():LoadAssetAction("commonUI_Atlasa", function(t)
    --     local spriteAtlas = t or UnityEngine.U2D.SpriteAtlas
    --     self.itemBgImage.sprite = spriteAtlas:GetSprite(weaponData)
    -- end)
    --- 设置icon
    GetAssetBundleManager():LoadAssetAction(UIConfigEnum.FishConfig.BagUIConfig.weaponIconAtlasName, function(t)
        local spriteAtlas = t or UnityEngine.U2D.SpriteAtlas
        self.itemIcon.sprite = spriteAtlas:GetSprite(weaponData.weaponIcons)
    end)
    --武器默认1
    self.itemNumText.text = string.format("%d", 1)
end

--- 点击 武器 item 事件
function WeaponItemUIView:clickEvent()
    --1. 打开 对应部分按钮
    --2. 打开 面板 信息详情
    --3. 针对面板 赋值 刷新面板
end

return WeaponItemUIView