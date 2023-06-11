---@class GameMainUIPanelView
local GameMainUIPanelView = BaseClass()
local _UIView = {}
function GameMainUIPanelView:Init(view)
	_UIView = view:GetComponent("UIView")
	self.top_head = _UIView:GetObjType("top_head") or CS.UnityEngine.GameObject
	self.LvBgIcon = _UIView:GetObjType("LvBgIcon") or CS.UnityEngine.UI.Image
	self.beadFrame = _UIView:GetObjType("beadFrame") or CS.UnityEngine.UI.Image
	self.headImgClick = _UIView:GetObjType("headImgClick") or CS.UnityEngine.UI.Button
	self.top_head_Name_Text = _UIView:GetObjType("top_head_Name_Text") or CS.UnityEngine.UI.Text
	self.top_head_id_Text = _UIView:GetObjType("top_head_id_Text") or CS.UnityEngine.UI.Text
	self.top_head_Lv_Image = _UIView:GetObjType("top_head_Lv_Image") or CS.UnityEngine.UI.Image
	self.top_head_Lv_LvNum_Text = _UIView:GetObjType("top_head_Lv_LvNum_Text") or CS.UnityEngine.UI.Text
	self.GemsTim_UISerializableKeyObject = _UIView:GetObjType("GemsTim_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.Gem_UISerializableKeyObject = _UIView:GetObjType("Gem_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.glod_UISerializableKeyObject = _UIView:GetObjType("glod_UISerializableKeyObject") or CS.ZJYFrameWork.UISerializable.UISerializableKeyObject
	self.middle = _UIView:GetObjType("middle") or CS.UnityEngine.GameObject
	self.TimeShow_Text = _UIView:GetObjType("TimeShow_Text") or CS.UnityEngine.UI.Text
	self.middle_hero = _UIView:GetObjType("middle_hero") or CS.UnityEngine.GameObject
	self.storeBtn = _UIView:GetObjType("storeBtn") or CS.UnityEngine.UI.Button
	self.HuodongBtn = _UIView:GetObjType("HuodongBtn") or CS.UnityEngine.UI.Button
	self.FuLiBtn = _UIView:GetObjType("FuLiBtn") or CS.UnityEngine.UI.Button
	self.newplayer_Button = _UIView:GetObjType("newplayer_Button") or CS.UnityEngine.UI.Button
	self.middle_Right = _UIView:GetObjType("middle_Right") or CS.UnityEngine.GameObject
	self.middle_Right_PVEBtn_Button = _UIView:GetObjType("middle_Right_PVEBtn_Button") or CS.UnityEngine.UI.Button
	self.physicaButton = _UIView:GetObjType("physicaButton") or CS.UnityEngine.UI.Button
	self.ruqin_Button = _UIView:GetObjType("ruqin_Button") or CS.UnityEngine.UI.Button
	self.wujinBtn_Button = _UIView:GetObjType("wujinBtn_Button") or CS.UnityEngine.UI.Button
	self.pvpBtn_Button = _UIView:GetObjType("pvpBtn_Button") or CS.UnityEngine.UI.Button
	self.migongBtn_Button = _UIView:GetObjType("migongBtn_Button") or CS.UnityEngine.UI.Button
	self.middle_downRight = _UIView:GetObjType("middle_downRight") or CS.UnityEngine.GameObject
	self.TaskBtn = _UIView:GetObjType("TaskBtn") or CS.UnityEngine.UI.Button
	self.UniversityBtn = _UIView:GetObjType("UniversityBtn") or CS.UnityEngine.UI.Button
	self.RecruitBtn = _UIView:GetObjType("RecruitBtn") or CS.UnityEngine.UI.Button
	self.HeroBtn = _UIView:GetObjType("HeroBtn") or CS.UnityEngine.UI.Button
	self.ArmsBtn = _UIView:GetObjType("ArmsBtn") or CS.UnityEngine.UI.Button
	self.SkillButton = _UIView:GetObjType("SkillButton") or CS.UnityEngine.UI.Button
	self.BagButton = _UIView:GetObjType("BagButton") or CS.UnityEngine.UI.Button
	self.settingBtn = _UIView:GetObjType("settingBtn") or CS.UnityEngine.UI.Button
	self.MailBtn = _UIView:GetObjType("MailBtn") or CS.UnityEngine.UI.Button
	self.MailBtn_tips = _UIView:GetObjType("MailBtn_tips") or CS.UnityEngine.GameObject
	self.GMUIController = _UIView:GetObjType("GMUIController") or CS.ZJYFrameWork.Hotfix.UI.GameMain.GameMainUIController
end

return GameMainUIPanelView