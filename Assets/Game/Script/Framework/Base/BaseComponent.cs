using System.Collections.Generic;
using Framework.AssetBundles.Config;
using UnityEngine;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Base
{
    //只应许添加一个组件
    [DisallowMultipleComponent]
    public sealed class BaseComponent : SpringComponent
    {
        [SerializeField] public int frameRate = 30;
        [SerializeField] public float gameSpeed = 1f;
        [SerializeField] private bool runInBackground = true;

        /// <summary>
        /// 获取或设置游戏帧率。
        /// </summary>
        public int FrameRate
        {
            get => frameRate;
            set => Application.targetFrameRate = frameRate = value;
        }

        /// <summary>
        /// 获取或设置游戏速度。
        /// </summary>
        public float GameSpeed
        {
            get => gameSpeed;
            set => Time.timeScale = gameSpeed = value >= 0f ? value : 0f;
        }

        /// <summary>
        /// 获取是否正常游戏速度。
        /// </summary>
        public bool IsNormalGameSpeed => gameSpeed == 1f;

        /// <summary>
        /// 获取或设置是否允许后台运行。
        /// </summary>
        public bool RunInBackground
        {
            get => runInBackground;
            set => Application.runInBackground = runInBackground = value;
        }

        protected override void Awake()
        {
            if (AssetBundleConfig.IsEditorMode)
            {
                //编辑器下
            }

            Application.targetFrameRate = frameRate;
            Time.timeScale = gameSpeed;
            Application.runInBackground = runInBackground;
            // Screen.sleepTimeout = neverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;

            base.Awake();
        }

        private int moduleSize = 0;

        private readonly List<AbstractManager> CachedModules = new List<AbstractManager>();

        public void StartSpring()
        {
            var scanPaths = new List<string>()
            {
                "Game"
            };
            SpringContext.AddScanPath(scanPaths);

            //扫描Bean类
            SpringContext.Scan();

            //网络扫描
            PacketDispatcher.Scan();
            //获取所有Module
            var moduleList = new List<AbstractManager>();
            var moduleComponents = SpringContext.GetBeans(typeof(AbstractManager));
            moduleComponents.ForEach(item => moduleList.Add((AbstractManager)item));
            //更改轮询顺序
            moduleList.Sort((a, b) => b.Priority - a.Priority);
            moduleList.ForEach(item => CachedModules.Add(item));
            //我当前module 有多少个
            moduleSize = (short)moduleList.Count;
        }
    }
}