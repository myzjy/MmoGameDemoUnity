---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2023/5/16 0:10
---
---@class GameMainVIew:UIView
GameMainView = class("GameMainView", UIView)
require("Game.GenerateScripts.UIModules.GameMainUIPanelView")

function GameMainView:OnLoad()
    self.UIConfig = {
        Config = GameMainConfig,
        viewPanel = GameMainUIPanelView,
        initFunc = function()
            printDebug("GameMainVIew:Init()")
            GameMainView:OnInit()
        end,
        showFunc = function()
            printDebug("GameMainVIew:showUI()-->showFunc")
            GameMainView:OnShow()
        end,
        hideFunc = function()

        end
    }
    self:Load(self.UIConfig)
    self:LoadUI(self.UIConfig)
    self:InstanceOrReuse()
end

function GameMainView:GetInstance()
    return GameMainView
end

function GameMainView:OnInit()
    printDebug("GameMain:OnInit")
    --- GameMain ViewPanel
    ---@type GameMainUIPanelView
    local ViewPanel = self.viewPanel
    --- 转换水晶 按钮
    self.GemsTimeButton = ViewPanel.GemsTim_UISerializableKeyObject:GetObjType("click") or CS.UnityEngine.UI.Button
    self.GemsText = ViewPanel.GemsTim_UISerializableKeyObject:GetObjType("numText") or CS.UnityEngine.UI.Text
    local data = CS.ZJYFrameWork.Spring.Core.SpringContext.GetBean("PlayerUserCaCheData") or
            CS.ZJYFrameWork.Hotfix.Common.PlayerUserCaCheData
    --- 显示名字
    ViewPanel.top_head_Name_Text.text = data.userName
    --- 付费水晶 商店充值跳转按钮
    self.GemButton = ViewPanel.Gem_UISerializableKeyObject:GetObjType("click") or CS.UnityEngine.UI.Button
    --- 显示付费水晶
    self.GemText = ViewPanel.Gem_UISerializableKeyObject:GetObjType("numText") or CS.UnityEngine.UI.Text
    --- 显示金币
    self.GoldCoinText = ViewPanel.Gem_UISerializableKeyObject:GetObjType("numText") or CS.UnityEngine.UI.Text
    --- 购买金币，第一跳转商场兑换
    self.GoldTimButton = ViewPanel.glod_UISerializableKeyObject:GetObjType("click") or CS.UnityEngine.UI.Button

    self:SetListener(ViewPanel.headImgClick, function()
        --- 点击头像
        CS.Debug.Log("点击头像，进行更换")
        --- 目前来说还不知道需不需要
    end)
    self:SetListener(self.GoldTimButton, function()
        --- 购买金币 这个是需要 去兑换
        --- 先写 但是不知道需要不要 明确
    end)
    self:SetListener(ViewPanel.settingBtn, function()
        PhysicalPowerService:SubPhysicalPowerService(10)
    end)
    local handle = UpdateBeat:CreateListener(GameMainView.UpdatePhysical, 0)
    UpdateBeat:AddListener(handle)

    GameMainView:OnShow()

end
local nowPhysicalPowerUpdateNum = 0

function GameMainView.UpdatePhysical(number)
    if timestamp < 1 then
        return
    end
    local nowTime = timestamp + 1000
    if nowTime - nowPhysicalPowerUpdateNum >= 1000 then
        nowPhysicalPowerUpdateNum = nowTime
        if PhysicalPowerCacheData:getNowPhysicalPower() >= PhysicalPowerCacheData:getMaximumStrength() then
            --- 体力满了
            return
        end
        PhysicalPowerService:SendPhysicalPowerSecondsRequest()
    end

end

function GameMainView:OnShow()
    printDebug("GameMain:OnShow")
    self.UIView:OnShow()
    PhysicalPowerService:SendPhysicalPower()
    ---此处 需要请求一系列 协议或者http 请求 以便刷新界面
end

---@param dateTime string
function GameMainView:ShowNowTime(dateTime)
    local ViewPanel = self.viewPanel
    if not self.viewPanel then
        return
    end
    if ViewPanel == nil then
        return
    end
    self.viewPanel.TimeShow_Text.text = dateTime
end

function GameMainView:ShowPhysicalPowerPanel(responseData)
    ---@type GameMainUIPanelView
    local ViewPanel = self.viewPanel
    if not self.viewPanel then
        return
    end
    if ViewPanel == nil then
        return
    end

    self:SetText(ViewPanel.physicalPower_Text, responseData.nowPhysicalPower .. "")
end

---显示金币数量
---@param num number
function GameMainView:ShowGoldNumTextAction(num)
    self.GoldCoinText.text = tostring(num)
end

function GameMainView:Notification()
    ---不能直接返回，直接返回在内部拿不到表
    local data = {
        [GameMainConfig.eventNotification.OPEN_GAMEMAIN_PANEL] = GameMainConfig.eventNotification.OPEN_GAMEMAIN_PANEL,
        [GameMainConfig.eventNotification.CLOSE_GAMEMAIN_PANEL] = GameMainConfig.eventNotification.CLOSE_GAMEMAIN_PANEL,
        [GameMainConfig.eventNotification.TIME_GAMEMAIN_PANEL] = GameMainConfig.eventNotification.TIME_GAMEMAIN_PANEL,
    }
    return data
end

function GameMainView:NotificationHandler(_eventNotification)
    local eventSwitch = {
        [GameMainConfig.eventNotification.OPEN_GAMEMAIN_PANEL] = function()
            if self.reUse then
                self:InstanceOrReuse()
            else
                self:OnLoad()
            end
        end,
        [GameMainConfig.eventNotification.CLOSE_GAMEMAIN_PANEL] = function(obj)
            self:OnHide()
        end,
        [GameMainConfig.eventNotification.TIME_GAMEMAIN_PANEL] = function(obj)
            if self.reUse == false then
                return
            end
            local timeString = obj or string
            ---显示时间
            self:ShowNowTime(timeString)
        end
    }
    local switchAction = eventSwitch[_eventNotification.eventName]
    if eventSwitch then
        return switchAction(_eventNotification.eventBody)
    end
end
