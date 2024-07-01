---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2024/6/29 下午1:52
---

---@class FrostUIButton:FrostGameObject
---@field uButton UnityEngine.UI.Button
local FrostUIButton = class("FrostUIButton", FrostGameObject)

function FrostUIButton:ctor()
	self.isClick = false
	self._OnClick = false
	self._button = false
	self._LongPressOnClick = false
end

function FrostUIButton:Init(argument)
	self._button = argument.button
	self.uObject = self._button.gameObject
	self.uGameObject = self._button.gameObject
	self.uTransform = self.uGameObject.transform
end

function FrostUIButton:OnPointerOnClick()
	if self.uButton.isPressed and Time.unity_time then
		-- 长按
		self._LongPressOnClick()
	else
		-- 短按
		self._OnClick()
	end
end

return FrostUIButton
