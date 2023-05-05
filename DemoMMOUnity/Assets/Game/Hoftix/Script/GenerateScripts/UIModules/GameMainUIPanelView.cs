using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public class GameMainUIPanelView:UIViewInterface
    {
        public UnityEngine.GameObject top_head=null;
		public UnityEngine.UI.Image LvBgIcon=null;
		public UnityEngine.UI.Image beadFrame=null;
		public UnityEngine.UI.Button headImgClick=null;
		public UnityEngine.UI.Text top_head_Name_Text=null;
		public UnityEngine.UI.Text top_head_id_Text=null;
		public UnityEngine.UI.Image top_head_Lv_Image=null;
		public UnityEngine.UI.Text top_head_Lv_LvNum_Text=null;
		public ZJYFrameWork.UISerializable.UISerializableKeyObject GemsTim_UISerializableKeyObject=null;
		public ZJYFrameWork.UISerializable.UISerializableKeyObject Gem_UISerializableKeyObject=null;
		public ZJYFrameWork.UISerializable.UISerializableKeyObject glod_UISerializableKeyObject=null;
		public UnityEngine.GameObject middle=null;
		public UnityEngine.GameObject middle_hero=null;
		public UnityEngine.UI.Button storeBtn=null;
		public UnityEngine.UI.Button HuodongBtn=null;
		public UnityEngine.UI.Button FuLiBtn=null;
		public UnityEngine.UI.Button newplayer_Button=null;
		public UnityEngine.GameObject middle_Right=null;
		public UnityEngine.UI.Button middle_Right_PVEBtn_Button=null;
		public UnityEngine.UI.Button physicaButton=null;
		public UnityEngine.UI.Button ruqin_Button=null;
		public UnityEngine.UI.Button wujinBtn_Button=null;
		public UnityEngine.UI.Button pvpBtn_Button=null;
		public UnityEngine.UI.Button migongBtn_Button=null;
		public UnityEngine.GameObject middle_downRight=null;
		public UnityEngine.UI.Button TaskBtn=null;
		public UnityEngine.UI.Button UniversityBtn=null;
		public UnityEngine.UI.Button RecruitBtn=null;
		public UnityEngine.UI.Button HeroBtn=null;
		public UnityEngine.UI.Button ArmsBtn=null;
		public UnityEngine.UI.Button SkillButton=null;
		public UnityEngine.UI.Button BagButton=null;
		public UnityEngine.UI.Button settingBtn=null;
		public UnityEngine.UI.Button MailBtn=null;
		public UnityEngine.GameObject MailBtn_tips=null;
		public ZJYFrameWork.Hotfix.UI.GameMain.GameMainUIController GMUIController=null;
		


        public void Init(UIView _view)
        {
            top_head=_view.GetObjType<UnityEngine.GameObject>("top_head");
			LvBgIcon=_view.GetObjType<UnityEngine.UI.Image>("LvBgIcon");
			beadFrame=_view.GetObjType<UnityEngine.UI.Image>("beadFrame");
			headImgClick=_view.GetObjType<UnityEngine.UI.Button>("headImgClick");
			top_head_Name_Text=_view.GetObjType<UnityEngine.UI.Text>("top_head_Name_Text");
			top_head_id_Text=_view.GetObjType<UnityEngine.UI.Text>("top_head_id_Text");
			top_head_Lv_Image=_view.GetObjType<UnityEngine.UI.Image>("top_head_Lv_Image");
			top_head_Lv_LvNum_Text=_view.GetObjType<UnityEngine.UI.Text>("top_head_Lv_LvNum_Text");
			GemsTim_UISerializableKeyObject=_view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("GemsTim_UISerializableKeyObject");
			Gem_UISerializableKeyObject=_view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("Gem_UISerializableKeyObject");
			glod_UISerializableKeyObject=_view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("glod_UISerializableKeyObject");
			middle=_view.GetObjType<UnityEngine.GameObject>("middle");
			middle_hero=_view.GetObjType<UnityEngine.GameObject>("middle_hero");
			storeBtn=_view.GetObjType<UnityEngine.UI.Button>("storeBtn");
			HuodongBtn=_view.GetObjType<UnityEngine.UI.Button>("HuodongBtn");
			FuLiBtn=_view.GetObjType<UnityEngine.UI.Button>("FuLiBtn");
			newplayer_Button=_view.GetObjType<UnityEngine.UI.Button>("newplayer_Button");
			middle_Right=_view.GetObjType<UnityEngine.GameObject>("middle_Right");
			middle_Right_PVEBtn_Button=_view.GetObjType<UnityEngine.UI.Button>("middle_Right_PVEBtn_Button");
			physicaButton=_view.GetObjType<UnityEngine.UI.Button>("physicaButton");
			ruqin_Button=_view.GetObjType<UnityEngine.UI.Button>("ruqin_Button");
			wujinBtn_Button=_view.GetObjType<UnityEngine.UI.Button>("wujinBtn_Button");
			pvpBtn_Button=_view.GetObjType<UnityEngine.UI.Button>("pvpBtn_Button");
			migongBtn_Button=_view.GetObjType<UnityEngine.UI.Button>("migongBtn_Button");
			middle_downRight=_view.GetObjType<UnityEngine.GameObject>("middle_downRight");
			TaskBtn=_view.GetObjType<UnityEngine.UI.Button>("TaskBtn");
			UniversityBtn=_view.GetObjType<UnityEngine.UI.Button>("UniversityBtn");
			RecruitBtn=_view.GetObjType<UnityEngine.UI.Button>("RecruitBtn");
			HeroBtn=_view.GetObjType<UnityEngine.UI.Button>("HeroBtn");
			ArmsBtn=_view.GetObjType<UnityEngine.UI.Button>("ArmsBtn");
			SkillButton=_view.GetObjType<UnityEngine.UI.Button>("SkillButton");
			BagButton=_view.GetObjType<UnityEngine.UI.Button>("BagButton");
			settingBtn=_view.GetObjType<UnityEngine.UI.Button>("settingBtn");
			MailBtn=_view.GetObjType<UnityEngine.UI.Button>("MailBtn");
			MailBtn_tips=_view.GetObjType<UnityEngine.GameObject>("MailBtn_tips");
			GMUIController=_view.GetObjType<ZJYFrameWork.Hotfix.UI.GameMain.GameMainUIController>("GMUIController");
			
        }
    }
}