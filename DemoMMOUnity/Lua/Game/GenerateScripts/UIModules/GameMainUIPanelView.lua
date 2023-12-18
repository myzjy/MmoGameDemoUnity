---@class GameMainUIPanelView
GameMainUIPanelView = class("GameMainUIPanelView")
function GameMainUIPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.top_head = self._UIView:GetObjTypeStr("top_head") or CS.UnityEngine.GameObject
	self.LvBgIcon = self._UIView:GetObjTypeStr("LvBgIcon") or CS.UnityEngine.UI.Image
	self.beadFrame = self._UIView:GetObjTypeStr("beadFrame") or CS.UnityEngine.UI.Image
	self.headImgClick = self._UIView:GetObjTypeStr("headImgClick") or CS.UnityEngine.UI.Button
	self.top_head_Name_Text = self._UIView:GetObjTypeStr("top_head_Name_Text") or CS.UnityEngine.UI.Text
	self.top_head_id_Text = self._UIView:GetObjTypeStr("top_head_id_Text") or CS.UnityEngine.UI.Text
	self.top_head_Lv_Image = self._UIView:GetObjTypeStr("top_head_Lv_Image") or CS.UnityEngine.UI.Image
	self.top_head_Lv_LvNum_Text = self._UIView:GetObjTypeStr("top_head_Lv_LvNum_Text") or CS.UnityEngine.UI.Text
	self.GemsTim_UISerializableKeyObject = self._UIView:GetObjTypeStr("GemsTim_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.Gem_UISerializableKeyObject = self._UIView:GetObjTypeStr("Gem_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.glod_UISerializableKeyObject = self._UIView:GetObjTypeStr("glod_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.middle = self._UIView:GetObjTypeStr("middle") or CS.UnityEngine.GameObject
	self.TimeShow_Text = self._UIView:GetObjTypeStr("TimeShow_Text") or CS.UnityEngine.UI.Text
	self.middle_hero = self._UIView:GetObjTypeStr("middle_hero") or CS.UnityEngine.GameObject
	self.storeBtn = self._UIView:GetObjTypeStr("storeBtn") or CS.UnityEngine.UI.Button
	self.HuodongBtn = self._UIView:GetObjTypeStr("HuodongBtn") or CS.UnityEngine.UI.Button
	self.FuLiBtn = self._UIView:GetObjTypeStr("FuLiBtn") or CS.UnityEngine.UI.Button
	self.newplayer_Button = self._UIView:GetObjTypeStr("newplayer_Button") or CS.UnityEngine.UI.Button
	self.middle_Right = self._UIView:GetObjTypeStr("middle_Right") or CS.UnityEngine.GameObject
	self.middle_Right_PVEBtn_Button = self._UIView:GetObjTypeStr("middle_Right_PVEBtn_Button") or CS.UnityEngine.UI.Button
	self.physicalPower_Text = self._UIView:GetObjTypeStr("physicalPower_Text") or CS.UnityEngine.UI.Text
	self.physicalPowerTip_Text = self._UIView:GetObjTypeStr("physicalPowerTip_Text") or CS.UnityEngine.UI.Text
	self.physicaButton = self._UIView:GetObjTypeStr("physicaButton") or CS.UnityEngine.UI.Button
	self.ruqin_Button = self._UIView:GetObjTypeStr("ruqin_Button") or CS.UnityEngine.UI.Button
	self.wujinBtn_Button = self._UIView:GetObjTypeStr("wujinBtn_Button") or CS.UnityEngine.UI.Button
	self.pvpBtn_Button = self._UIView:GetObjTypeStr("pvpBtn_Button") or CS.UnityEngine.UI.Button
	self.migongBtn_Button = self._UIView:GetObjTypeStr("migongBtn_Button") or CS.UnityEngine.UI.Button
	self.middle_downRight = self._UIView:GetObjTypeStr("middle_downRight") or CS.UnityEngine.GameObject
	self.TaskBtn = self._UIView:GetObjTypeStr("TaskBtn") or CS.UnityEngine.UI.Button
	self.UniversityBtn = self._UIView:GetObjTypeStr("UniversityBtn") or CS.UnityEngine.UI.Button
	self.RecruitBtn = self._UIView:GetObjTypeStr("RecruitBtn") or CS.UnityEngine.UI.Button
	self.HeroBtn = self._UIView:GetObjTypeStr("HeroBtn") or CS.UnityEngine.UI.Button
	self.ArmsBtn = self._UIView:GetObjTypeStr("ArmsBtn") or CS.UnityEngine.UI.Button
	self.SkillButton = self._UIView:GetObjTypeStr("SkillButton") or CS.UnityEngine.UI.Button
	self.BagButton = self._UIView:GetObjTypeStr("BagButton") or CS.UnityEngine.UI.Button
	self.settingBtn = self._UIView:GetObjTypeStr("settingBtn") or CS.UnityEngine.UI.Button
	self.MailBtn = self._UIView:GetObjTypeStr("MailBtn") or CS.UnityEngine.UI.Button
	self.MailBtn_tips = self._UIView:GetObjTypeStr("MailBtn_tips") or CS.UnityEngine.GameObject
end

