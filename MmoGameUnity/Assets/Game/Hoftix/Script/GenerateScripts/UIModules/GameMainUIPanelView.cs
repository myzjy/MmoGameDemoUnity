using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.Hotfix.UI.GameMain;

namespace ZJYFrameWork.UISerializable
{
    public class GameMainUIPanelView:UIViewInterface
    {
        public ZJYFrameWork.UISerializable.UISerializableKeyObject GemsTim_UISerializableKeyObject=null;
		public ZJYFrameWork.UISerializable.UISerializableKeyObject Gem_UISerializableKeyObject=null;
		public UnityEngine.GameObject top_head=null;
		public UnityEngine.UI.Image headIcon=null;
		public UnityEngine.UI.Image beadFrame=null;
		public UnityEngine.UI.Button headImgClick=null;
		public UnityEngine.UI.Text top_head_Name_Text=null;
		public UnityEngine.UI.Button storeBtn=null;
		public UnityEngine.UI.Button HuodongBtn=null;
		public UnityEngine.UI.Button FuLiBtn=null;
		public UnityEngine.GameObject middle_Right=null;
		public UnityEngine.UI.Button PVEBtn_Button=null;
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
		public GameMainUIController GMUIController=null;
		


        public void Init(UIView _view)
        {
            GemsTim_UISerializableKeyObject=_view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("GemsTim_UISerializableKeyObject");
			Gem_UISerializableKeyObject=_view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("Gem_UISerializableKeyObject");
			top_head=_view.GetObjType<UnityEngine.GameObject>("top_head");
			headIcon=_view.GetObjType<UnityEngine.UI.Image>("headIcon");
			beadFrame=_view.GetObjType<UnityEngine.UI.Image>("beadFrame");
			headImgClick=_view.GetObjType<UnityEngine.UI.Button>("headImgClick");
			top_head_Name_Text=_view.GetObjType<UnityEngine.UI.Text>("top_head_Name_Text");
			storeBtn=_view.GetObjType<UnityEngine.UI.Button>("storeBtn");
			HuodongBtn=_view.GetObjType<UnityEngine.UI.Button>("HuodongBtn");
			FuLiBtn=_view.GetObjType<UnityEngine.UI.Button>("FuLiBtn");
			middle_Right=_view.GetObjType<UnityEngine.GameObject>("middle_Right");
			PVEBtn_Button=_view.GetObjType<UnityEngine.UI.Button>("PVEBtn_Button");
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
			GMUIController=_view.GetObjType<GameMainUIController>("GMUIController");
			
        }
    }
}