using System;
using System.Collections.Generic;
using UnityEngine;
using ZJYFrameWork.AssetBundles.AssetBundleToolsConfig;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.Bundles.LoaderBuilders;
using ZJYFrameWork.Base.Component;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Event;
using ZJYFrameWork.Log;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Procedure;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.Spring.Utils;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.Base.Component
{
    //只应许添加一个组件
    [DisallowMultipleComponent]
    [AddComponentMenu("Game/Framework/Base")]
    public sealed class BaseComponent : SpringComponent
    {
        [SerializeField] public int frameRate = 30;
        [SerializeField] public float gameSpeed = 1f;
        [SerializeField] private bool runInBackground = true;
        private const int DefaultDpi = 96; // default windows dpi

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

        [SerializeField] public string logHelperTypeName;
        // protected override void Awake()
        // {
        //
        //   
        //     // Screen.sleepTimeout = neverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
        //
        //  
        //     // StartSpring();
        // }

        protected override void OnAwake()
        {
            DontDestroyOnLoad(this);
            Debug.Initialize();
            InitLogHelper();
            ConverterUtils.ScreenDpi = Screen.dpi;
            if (ConverterUtils.ScreenDpi <= 0)
            {
                ConverterUtils.ScreenDpi = DefaultDpi;
            }

            Application.targetFrameRate = frameRate;
            Time.timeScale = gameSpeed;
            Application.runInBackground = runInBackground;
            base.OnAwake();
        }

        private int moduleSize = 0;

        private readonly List<AbstractManager> CachedModules = new List<AbstractManager>();

        public void StartSpring()
        {
            var scanPaths = new List<string>()
            {
                "ZJYFrameWork"
            };
            SpringContext.AddScanPath(scanPaths);
            Debug.Log("扫描Bean类");
            //扫描Bean类
            SpringContext.Scan();
            Debug.Log("扫描Bean类成功");

            Debug.Log("开始扫描事件");
            EventBus.Scan();
            Debug.Log("事件扫描成功");
            Debug.Log("扫描网络协议模块");
            //网络扫描
            PacketDispatcher.Scan();
            Debug.Log("扫描网络协议模块成功");
            UIComponentManager.InitUIModelComponent();
            //获取所有Module
            var moduleList = new List<AbstractManager>();
            var moduleComponents = SpringContext.GetBeans(typeof(AbstractManager));
            moduleComponents.ForEach(item => moduleList.Add((AbstractManager)item));
            //更改轮询顺序
            moduleList.Sort((a, b) => b.Priority - a.Priority);
            moduleList.ForEach(item => CachedModules.Add(item));
            //我当前module 有多少个
            moduleSize = (short)moduleList.Count;
            //初始化流程状态
            SpringContext.GetBean<NetworkManager>().Init();
            SpringContext.GetBean<ProcedureComponent>().StartProcedure();
        }

        private void LateUpdate()
        {
            for (var i = 0; i < moduleSize; i++)
            {
                CachedModules[i].Update(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        private void InitLogHelper()
        {
            Type logHelperType = AssemblyUtils.GetTypeByName(logHelperTypeName);
            if (logHelperType == null)
            {
                throw new Exception(StringUtils.Format("Can not find log helper type '{}'.", logHelperTypeName));
            }

            ILogFactory logHelper = (ILogFactory)Activator.CreateInstance(logHelperType);
            if (logHelper == null)
            {
                throw new Exception(StringUtils.Format("Can not create log helper instance '{}'.", logHelperTypeName));
            }

            Debug.SetLogHelper(logHelper);
        }

        private void OnApplicationQuit()
        {
            Application.lowMemory -= OnLowMemory;
            StopAllCoroutines();
        }

        private void OnLowMemory()
        {
            Debug.Log("Low memory reported...");

            // SpringContext.GetBean<IObjectPoolManager>().ReleaseAllUnused();
            // SpringContext.GetBean<ResourceComponent>().ForceUnloadUnusedAssets(true);
        }

        /// <summary>
        /// 关闭游戏框架。
        /// </summary>
        /// <param name="shutdownType">关闭游戏框架类型。</param>
        public void Shutdown(ShutdownType shutdownType)
        {
            Debug.Log("Shutdown Game Framework ({})...", shutdownType.ToString());

            // Destroy(gameObject);

            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            return;
        }
    }
}