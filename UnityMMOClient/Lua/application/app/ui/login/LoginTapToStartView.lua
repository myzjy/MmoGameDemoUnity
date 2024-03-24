--- 登录成功之后点击 开始游戏
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/12/10 21:43
---

---@class LoginTapToStartView:UIBaseView
local LoginTapToStartView = class("LoginTapToStartView", UIBaseView())

function LoginTapToStartView:ctor(view)
	self.LoginStartButton = LuaUtils.GetKeyButtonGameObject(view, "LoginStartButton")
	---关闭按钮
	self.LoginStartMaxButton = LuaUtils.GetKeyButtonGameObject(view, "LoginStartMaxButton")
	--  view:GetObjTypeStr("LoginStartMaxButton") or UnityEngine.UI.Button
	self.SteamLoginCanvasGroup = LuaUtils.GetKeyCanvaGroupGameObject(view, "LoginStart_CanvasGroup")
	-- view:GetObjTypeStr("LoginStart_CanvasGroup") or UnityEngine.CanvasGroup

	if self.LoginStartButton == nil then
		if Debug > 0 then
			PrintError("请检查[" .. view.name .. "]物体配置下面是否有[LoginStartButton]组件")
		end
	end
	self:SetListener(self.LoginStartButton, function()
		-- #GameEvent.LoginTapToStart()
		LoginNetController:LoginTapToStart()
	end)
	self:SetListener(self.LoginStartMaxButton, function()
		LoginUIController:GetInstance():OnClose()
	end)
end

function LoginTapToStartView:OnShow()
	UICommonViewController:GetInstance().LoadingRotate:OnClose()
	self.SteamLoginCanvasGroup.alpha = 1
	self.SteamLoginCanvasGroup.interactable = true
	self.SteamLoginCanvasGroup.ignoreParentGroups = true
	self.SteamLoginCanvasGroup.blocksRaycasts = true
end

function LoginTapToStartView:OnHide()
	if self.SteamLoginCanvasGroup.alpha < 1 then
		return
	end
	self.SteamLoginCanvasGroup.alpha = 0
	self.SteamLoginCanvasGroup.interactable = false
	self.SteamLoginCanvasGroup.ignoreParentGroups = false
	self.SteamLoginCanvasGroup.blocksRaycasts = false
end

local procedureChangeSceneStr = "ZJYFrameWork.Procedure.Scene.ProcedureChangeScene"
function LoginTapToStartView:LoginSatrtGame()
	LoginUIController:GetInstance():OnClose()
	local procedureChangeScene = CS.ZJYFrameWork.Spring.Core.SpringContext.GetBean(procedureChangeSceneStr) or
		CS.ZJYFrameWork.Procedure.Scene.ProcedureChangeScene
	---跳转场景
	procedureChangeScene:ChangeScene(ZJYFrameWork.Constant.SceneEnum.GameMain, "GameMain")
end

return LoginTapToStartView
