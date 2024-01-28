---@class GameMainUIPanelView
GameMainUIPanelView = class("GameMainUIPanelView")
function GameMainUIPanelView:Init(view)
	self._UIView = view:GetComponent("UIView")
	self.top_head = self._UIView:GetObjType("top_head") or UnityEngine.GameObject
	self.LvBgIcon = self._UIView:GetObjType("LvBgIcon") or UnityEngine.UI.Image
	self.beadFrame = self._UIView:GetObjType("beadFrame") or UnityEngine.UI.Image
	self.headImgClick = self._UIView:GetObjType("headImgClick") or UnityEngine.UI.Button
	self.top_head_Name_Text = self._UIView:GetObjType("top_head_Name_Text") or UnityEngine.UI.Text
	self.top_head_id_Text = self._UIView:GetObjType("top_head_id_Text") or UnityEngine.UI.Text
	self.top_head_Lv_Image = self._UIView:GetObjType("top_head_Lv_Image") or UnityEngine.UI.Image
	self.top_head_Lv_LvNum_Text = self._UIView:GetObjType("top_head_Lv_LvNum_Text") or UnityEngine.UI.Text
	self.GemsTim_UISerializableKeyObject = self._UIView:GetObjType("GemsTim_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.Gem_UISerializableKeyObject = self._UIView:GetObjType("Gem_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.glod_UISerializableKeyObject = self._UIView:GetObjType("glod_UISerializableKeyObject") or ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.middle = self._UIView:GetObjType("middle") or UnityEngine.GameObject
	self.TimeShow_Text = self._UIView:GetObjType("TimeShow_Text") or UnityEngine.UI.Text
	self.middle_hero = self._UIView:GetObjType("middle_hero") or UnityEngine.GameObject
	self.storeBtn = self._UIView:GetObjType("storeBtn") or UnityEngine.UI.Button
	self.HuodongBtn = self._UIView:GetObjType("HuodongBtn") or UnityEngine.UI.Button
	self.FuLiBtn = self._UIView:GetObjType("FuLiBtn") or UnityEngine.UI.Button
	self.newplayer_Button = self._UIView:GetObjType("newplayer_Button") or UnityEngine.UI.Button
	self.middle_Right = self._UIView:GetObjType("middle_Right") or UnityEngine.GameObject
	self.middle_Right_PVEBtn_Button = self._UIView:GetObjType("middle_Right_PVEBtn_Button") or UnityEngine.UI.Button
	self.physicalPower_Text = self._UIView:GetObjType("physicalPower_Text") or UnityEngine.UI.Text
	self.physicalPowerTip_Text = self._UIView:GetObjType("physicalPowerTip_Text") or UnityEngine.UI.Text
	self.physicaButton = self._UIView:GetObjType("physicaButton") or UnityEngine.UI.Button
	self.ruqin_Button = self._UIView:GetObjType("ruqin_Button") or UnityEngine.UI.Button
	self.wujinBtn_Button = self._UIView:GetObjType("wujinBtn_Button") or UnityEngine.UI.Button
	self.pvpBtn_Button = self._UIView:GetObjType("pvpBtn_Button") or UnityEngine.UI.Button
	self.migongBtn_Button = self._UIView:GetObjType("migongBtn_Button") or UnityEngine.UI.Button
	self.middle_downRight = self._UIView:GetObjType("middle_downRight") or UnityEngine.GameObject
	self.TaskBtn = self._UIView:GetObjType("TaskBtn") or UnityEngine.UI.Button
	self.UniversityBtn = self._UIView:GetObjType("UniversityBtn") or UnityEngine.UI.Button
	self.RecruitBtn = self._UIView:GetObjType("RecruitBtn") or UnityEngine.UI.Button
	self.HeroBtn = self._UIView:GetObjType("HeroBtn") or UnityEngine.UI.Button
	self.ArmsBtn = self._UIView:GetObjType("ArmsBtn") or UnityEngine.UI.Button
	self.SkillButton = self._UIView:GetObjType("SkillButton") or UnityEngine.UI.Button
	self.BagButton = self._UIView:GetObjType("BagButton") or UnityEngine.UI.Button
	self.settingBtn = self._UIView:GetObjType("settingBtn") or UnityEngine.UI.Button
	self.MailBtn = self._UIView:GetObjType("MailBtn") or UnityEngine.UI.Button
	self.MailBtn_tips = self._UIView:GetObjType("MailBtn_tips") or UnityEngine.GameObject
end

