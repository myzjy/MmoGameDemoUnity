---背包 UI 相关 逻辑
---@class BagUIView:UIView
local BagUIView = class("BagUIView", UIView)
require("application.app.ui.bag.bagUIConfig")
local BagHeaderBtnPanel = require("application.app.ui.bag.bagHeaderBtnPanel")
local WeaponUIPanel = require("application.app.ui.bag.bagWeaponUIPanelView")
local bagModelUIView = require("application.app.ui.bag.modelView.bagModelUIView")

local function UIConfig()
    return {
        Config = UIEventConfig.BagUIConfig,
        viewPanel = BagUIPanelView,
        initFunc = function()
            PrintDebug("call bagUIView lua scirpt local function UIConfig called initFunc OnInit")
            BagUIView:OnInit()
        end,
        showFunc = function()
            PrintDebug("call bagUIView lua scritpt lcoal func show")
            BagUIView:OnShow()
        end,
        hideFunc = function()
            BagUIView.BagModelUIView:OnHideClearCacheDataList()
        end
    }
end
function BagUIView:ctor()
    self.bagHeaderBtnPanel = nil
    self:RegisterEvent()
end

function BagUIView:OnLoad()
    self:Load(UIConfig())
    self:LoadUI(UIConfig())
    self:InstanceOrReuse()
end

function BagUIView:OnInit()
    ---@type BagUIPanelView
    local view = self.viewPanel
    self.BagModelUIView = bagModelUIView()
    self.weaponUIView = WeaponUIPanel(view)
    self.bagHeaderBtnPanel = BagHeaderBtnPanel(self.gameObject);
    BagUIController:InitSendServerMessage()
end

function BagUIView:OnShow()

end

function BagUIView:RegisterEvent()
    -- 空的 注册事件 在BagUIController 脚本
    --UIUtils.AddEventListener(UIGameEvent.BagHeaderWeaponBtnHandler, self.OpenWeaponPanel, self)
    -- UIUtils.AddEventListener(GameEvent.AtBagHeaderWeaponBtnServiceHandler, self.OpenWeaponPanel, self)
end

function BagUIView:CreateWeaponListPanel()
end
--- 打开 背包武器 面板
function BagUIView:OpenWeaponPanel()
    coroutine.start(function()
      
        if self.weaponIconAtlas == nil then
            GetAssetBundleManager():LoadAssetAction(BagUIConfig.weaponIconAtlasName, function(t)
                local spriteAtlas = t or UnityEngine.U2D.SpriteAtlas
                self.weaponIconAtlas = spriteAtlas
            end)
            -- 等待 图集读取
            while self.weaponIconAtlas == nil do
                coroutine.wait(1)
            end
        end
        local weaponInfo = WeaponNetController:GetWeaponUserEntityList()
        local atlasData = {
            weaponIconAtlas = self.weaponIconAtlas
        }
        -- 创建武器
        self.weaponUIView:CreateItemList(weaponInfo, atlasData)
    end)

end

return BagUIView
