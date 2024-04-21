---@class GameMainUIPanelView
GameMainUIPanelView = class("GameMainUIPanelView")
function GameMainUIPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.top_head = self._UIView:GetObjTypeStr("top_head") or UnityEngine.GameObject
	self.LvBgIcon = self._UIView:GetObjTypeStr("LvBgIcon") or UnityEngine.UI.Image
	self.beadFrame = self._UIView:GetObjTypeStr("beadFrame") or UnityEngine.UI.Image
	self.headImgClick = self._UIView:GetObjTypeStr("headImgClick") or UnityEngine.UI.Button
	self.top_head_Name_Text = self._UIView:GetObjTypeStr("top_head_Name_Text") or UnityEngine.UI.Text
	self.top_head_id_Text = self._UIView:GetObjTypeStr("top_head_id_Text") or UnityEngine.UI.Text
	self.top_head_Lv_Image = self._UIView:GetObjTypeStr("top_head_Lv_Image") or UnityEngine.UI.Image
	self.top_head_Lv_LvNum_Text = self._UIView:GetObjTypeStr("top_head_Lv_LvNum_Text") or UnityEngine.UI.Text
	self.GemsTim_UISerializableKeyObject = self._UIView:GetObjTypeStr("GemsTim_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.Gem_UISerializableKeyObject = self._UIView:GetObjTypeStr("Gem_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.glod_UISerializableKeyObject = self._UIView:GetObjTypeStr("glod_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.middle = self._UIView:GetObjTypeStr("middle") or UnityEngine.GameObject
	self.TimeShow_Text = self._UIView:GetObjTypeStr("TimeShow_Text") or UnityEngine.UI.Text
	self.middle_hero = self._UIView:GetObjTypeStr("middle_hero") or UnityEngine.GameObject
	self.storeBtn = self._UIView:GetObjTypeStr("storeBtn") or UnityEngine.UI.Button
	self.HuodongBtn = self._UIView:GetObjTypeStr("HuodongBtn") or UnityEngine.UI.Button
	self.FuLiBtn = self._UIView:GetObjTypeStr("FuLiBtn") or UnityEngine.UI.Button
	self.newplayer_Button = self._UIView:GetObjTypeStr("newplayer_Button") or UnityEngine.UI.Button
	self.middle_Right = self._UIView:GetObjTypeStr("middle_Right") or UnityEngine.GameObject
	self.middle_Right_PVEBtn_Button = self._UIView:GetObjTypeStr("middle_Right_PVEBtn_Button") or UnityEngine.UI.Button
	self.physicalPower_Text = self._UIView:GetObjTypeStr("physicalPower_Text") or UnityEngine.UI.Text
	self.physicalPowerTip_Text = self._UIView:GetObjTypeStr("physicalPowerTip_Text") or UnityEngine.UI.Text
	self.physicaButton = self._UIView:GetObjTypeStr("physicaButton") or UnityEngine.UI.Button
	self.ruqin_Button = self._UIView:GetObjTypeStr("ruqin_Button") or UnityEngine.UI.Button
	self.wujinBtn_Button = self._UIView:GetObjTypeStr("wujinBtn_Button") or UnityEngine.UI.Button
	self.pvpBtn_Button = self._UIView:GetObjTypeStr("pvpBtn_Button") or UnityEngine.UI.Button
	self.migongBtn_Button = self._UIView:GetObjTypeStr("migongBtn_Button") or UnityEngine.UI.Button
	self.middle_downRight = self._UIView:GetObjTypeStr("middle_downRight") or UnityEngine.GameObject
	self.TaskBtn = self._UIView:GetObjTypeStr("TaskBtn") or UnityEngine.UI.Button
	self.UniversityBtn = self._UIView:GetObjTypeStr("UniversityBtn") or UnityEngine.UI.Button
	self.RecruitBtn = self._UIView:GetObjTypeStr("RecruitBtn") or UnityEngine.UI.Button
	self.HeroBtn = self._UIView:GetObjTypeStr("HeroBtn") or UnityEngine.UI.Button
	self.ArmsBtn = self._UIView:GetObjTypeStr("ArmsBtn") or UnityEngine.UI.Button
	self.SkillButton = self._UIView:GetObjTypeStr("SkillButton") or UnityEngine.UI.Button
	self.BagButton = self._UIView:GetObjTypeStr("BagButton") or UnityEngine.UI.Button
	self.settingBtn = self._UIView:GetObjTypeStr("settingBtn") or UnityEngine.UI.Button
	self.MailBtn = self._UIView:GetObjTypeStr("MailBtn") or UnityEngine.UI.Button
	self.MailBtn_tips = self._UIView:GetObjTypeStr("MailBtn_tips") or UnityEngine.GameObject
	self.selectCharacter = self._UIView:GetObjTypeStr("selectCharacter") or UnityEngine.GameObject
	self.selectCharacterBtn = self._UIView:GetObjTypeStr("selectCharacterBtn") or UnityEngine.GameObject
end

