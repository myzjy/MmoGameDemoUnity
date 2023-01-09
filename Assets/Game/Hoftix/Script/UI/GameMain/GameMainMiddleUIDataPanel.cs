using System;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.Collection.Reference;

namespace ZJYFrameWork.UISerializable
{
    public class GameMainMiddleRightUIPanel : IReference
    {
        /// <summary>
        /// 迷宫
        /// </summary>
        private Button _migongBtnButton;

        /// <summary>
        /// PVE 按钮
        /// </summary>
        private Button _pveBtnButton;

        /// <summary>
        /// PVP按钮
        /// </summary>
        private Button _pvpBtnButton;

        /// <summary>
        /// 入侵按钮 pvp
        /// </summary>
        private Button _ruqinButton;

        /// <summary>
        ///  爬塔按钮
        /// </summary>
        private Button _wujinBtnButton;

        public Button PveBtnButton
        {
            get => _pveBtnButton;
            set => _pveBtnButton = value;
        }

        public Button RuqinButton
        {
            get => _ruqinButton;
            set => _ruqinButton = value;
        }

        public Button WujinBtnButton
        {
            get => _wujinBtnButton;
            set => _wujinBtnButton = value;
        }

        public Button PvpBtnButton
        {
            get => _pvpBtnButton;
            set => _pvpBtnButton = value;
        }

        public Button MigongBtnButton
        {
            get => _migongBtnButton;
            set => _migongBtnButton = value;
        }

        public void Clear()
        {
            _pveBtnButton = null;
            _ruqinButton = null;
            _wujinBtnButton = null;
            _pvpBtnButton = null;
            _migongBtnButton = null;
        }

        public static GameMainMiddleRightUIPanel ValueOf(UISerializableKeyObject serializableKeyObject)
        {
            var item = ReferenceCache.Acquire<GameMainMiddleRightUIPanel>();
            item._pveBtnButton = serializableKeyObject.GetObjType<Button>("PVEBtn_Button");
            item._ruqinButton = serializableKeyObject.GetObjType<Button>("ruqin_Button");
            item._wujinBtnButton = serializableKeyObject.GetObjType<Button>("wujinBtn_Button");
            item._pvpBtnButton = serializableKeyObject.GetObjType<Button>("pvpBtn_Button");
            item._migongBtnButton = serializableKeyObject.GetObjType<Button>("migongBtn_Button");
            return item;
        }
    }

    public class GameMainMiddleDownRightUIPanel : IReference
    {
        /// <summary>
        /// 编制
        /// </summary>
        private Button ArmsBtn;

        /// <summary>
        /// 背包
        /// </summary>
        private Button BagButton;

        /// <summary>
        /// 英雄
        /// </summary>
        private Button HeroBtn;

        /// <summary>
        ///  抽卡界面按钮
        /// </summary>
        private Button RecruitBtn;

        /// <summary>
        /// 技能
        /// </summary>
        private Button skillButton;

        /// <summary>
        /// 任务按钮
        /// </summary>
        private Button TaskButton;

        /// <summary>
        /// 科技按钮
        /// </summary>
        private Button UniversityBtn;

        public void Clear()
        {
            TaskButton = null;
            UniversityBtn = null;
            RecruitBtn = null;
            HeroBtn = null;
            ArmsBtn = null;
            skillButton = null;
            BagButton = null;
        }

        public static GameMainMiddleDownRightUIPanel ValueOf(UISerializableKeyObject serializableKeyObject)
        {
            var item = ReferenceCache.Acquire<GameMainMiddleDownRightUIPanel>();
            item.ArmsBtn = serializableKeyObject.GetObjType<Button>("ArmsBtn");
            item.BagButton = serializableKeyObject.GetObjType<Button>("BagButton");
            item.HeroBtn = serializableKeyObject.GetObjType<Button>("HeroBtn");
            item.RecruitBtn = serializableKeyObject.GetObjType<Button>("RecruitBtn");
            item.skillButton = serializableKeyObject.GetObjType<Button>("skillButton");
            item.TaskButton = serializableKeyObject.GetObjType<Button>("TaskButton");
            item.UniversityBtn = serializableKeyObject.GetObjType<Button>("UniversityBtn");
            return item;
        }
    }

    /// <summary>
    /// 包含
    /// </summary>
    public class GameMainMiddleUIDataPanel
    {
        /// <summary>
        /// 中间部分right边按钮
        /// </summary>
        private GameMainMiddleRightUIPanel _gameMainMiddleRightUIPanel;

        private UISerializableKeyObject downRight_UISerializableKeyObject;

        /// <summary>
        /// 福利按钮
        /// </summary>
        private Button FuLiBtn;

        private GameMainMiddleDownRightUIPanel gameMainMiddleDownRightUIPanel;


        /// <summary>
        /// 生成角色
        /// </summary>
        private GameObject heroHead;

        /// <summary>
        /// 活动按钮
        /// </summary>
        private Button HuodongBtn;

        private UISerializableKeyObject Right_UISerializableKeyObject;

        /// <summary>
        /// 商城按钮
        /// </summary>
        private Button storeButton;

        public GameMainMiddleRightUIPanel GameMainMiddleRightUIPanel
        {
            get
            {
                try
                {
                    if (_gameMainMiddleRightUIPanel == null)
                    {
                        _gameMainMiddleRightUIPanel =
                            GameMainMiddleRightUIPanel.ValueOf(Right_UISerializableKeyObject);
                    }
                }
                catch (Exception e)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.Log($"{e.Message}");
#endif
                    // Console.WriteLine(e);
                    throw new NullReferenceException(e.Message);
                }


                return _gameMainMiddleRightUIPanel;
            }
        }

        public GameMainMiddleDownRightUIPanel GameMainMiddleDownRightUIPanel
        {
            get
            {
                if (gameMainMiddleDownRightUIPanel == null)
                {
                    gameMainMiddleDownRightUIPanel =
                        GameMainMiddleDownRightUIPanel.ValueOf(downRight_UISerializableKeyObject);
                }

                return gameMainMiddleDownRightUIPanel;
            }
        }

        public void Init(UISerializableKeyObject uiSerializableKeyObject)
        {
            heroHead = uiSerializableKeyObject.GetObjType<GameObject>("heroHead");
            storeButton = uiSerializableKeyObject.GetObjType<Button>("storeButton");
            HuodongBtn = uiSerializableKeyObject.GetObjType<Button>("HuodongBtn");
            FuLiBtn = uiSerializableKeyObject.GetObjType<Button>("FuLiBtn");
            Right_UISerializableKeyObject =
                uiSerializableKeyObject.GetObjType<UISerializableKeyObject>("Right_UISerializableKeyObject");
            downRight_UISerializableKeyObject =
                uiSerializableKeyObject.GetObjType<UISerializableKeyObject>("downRight_UISerializableKeyObject");
        }
    }
}