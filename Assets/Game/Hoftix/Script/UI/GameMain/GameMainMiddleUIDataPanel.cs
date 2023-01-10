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
        /// PVP按钮
        /// </summary>
        private Button _pvpBtnButton;

        /// <summary>
        ///  爬塔按钮
        /// </summary>
        private Button _wujinBtnButton;

        /// <summary>
        /// PVE 按钮
        /// </summary>
        public Button PveBtnButton { get; private set; }

        /// <summary>
        /// 入侵按钮 pvp
        /// </summary>
        public Button RuqinButton { get; protected set; }

        /// <summary>
        ///  爬塔按钮
        /// </summary>
        public Button WujinBtnButton
        {
            get => _wujinBtnButton;
            protected set => _wujinBtnButton = value;
        }

        public Button PvpBtnButton
        {
            get => _pvpBtnButton;
            protected set => _pvpBtnButton = value;
        }

        public Button MigongBtnButton
        {
            get => _migongBtnButton;
            protected set => _migongBtnButton = value;
        }

        public void Clear()
        {
            PveBtnButton = null;
            RuqinButton = null;
            _wujinBtnButton = null;
            _pvpBtnButton = null;
            _migongBtnButton = null;
        }

        public static GameMainMiddleRightUIPanel ValueOf(UISerializableKeyObject serializableKeyObject)
        {
            var item = ReferenceCache.Acquire<GameMainMiddleRightUIPanel>();
            item.PveBtnButton = serializableKeyObject.GetObjType<Button>("PVEBtn_Button");
            item.RuqinButton = serializableKeyObject.GetObjType<Button>("ruqin_Button");
            item._wujinBtnButton = serializableKeyObject.GetObjType<Button>("wujinBtn_Button");
            item._pvpBtnButton = serializableKeyObject.GetObjType<Button>("pvpBtn_Button");
            item._migongBtnButton = serializableKeyObject.GetObjType<Button>("migongBtn_Button");
            return item;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GameMainMiddleDownRightUIPanel : IReference
    {
        /// <summary>
        /// 编制
        /// </summary>
        public Button ArmsBtn { get; protected set; }

        /// <summary>
        /// 背包
        /// </summary>
        public Button BagButton { get; protected set; }

        /// <summary>
        /// 英雄
        /// </summary>
        public Button HeroBtn { get; protected set; }

        /// <summary>
        ///  抽卡界面按钮
        /// </summary>
        public Button RecruitBtn { get; protected set; }

        /// <summary>
        /// 技能
        /// </summary>
        public Button skillButton { get; protected set; }

        /// <summary>
        /// 任务按钮
        /// </summary>
        public Button TaskButton { get; protected set; }

        /// <summary>
        /// 科技按钮
        /// </summary>
        public Button UniversityBtn { get; protected set; }

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

        /// <summary>
        /// 中间部分包括pve按钮，爬塔按钮
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public GameMainMiddleRightUIPanel GameMainMiddleRightUIPanel
        {
            get
            {
                try
                {
                    _gameMainMiddleRightUIPanel ??= GameMainMiddleRightUIPanel.ValueOf(Right_UISerializableKeyObject);
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

        /// <summary>
        /// 编制  背包 角色界面 技能 抽卡界面按钮 科技按钮
        /// </summary>
        public GameMainMiddleDownRightUIPanel GameMainMiddleDownRightUIPanel
        {
            get
            {
                try
                {
                    gameMainMiddleDownRightUIPanel ??=
                        GameMainMiddleDownRightUIPanel.ValueOf(downRight_UISerializableKeyObject);
                }
                catch (Exception e)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.Log($"{e}");
                    //可能消息需要传递给服务器，错误消息进行保存
#endif
                    throw;
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