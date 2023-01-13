using System.Threading;
using UnityEngine;
#if NETFX_CORE
    using System.Threading.Tasks;
#endif

// ReSharper disable once CheckNamespace
namespace BestHTTP
{
    /// <summary>
    /// 将一些U3D调用路由到HTTPManager。
    /// </summary>
    [ExecuteInEditMode]
    [PlatformSupport.IL2CPP.Il2CppEagerStaticClassConstructionAttribute]
    public sealed class HttpUpdateDelegator : MonoBehaviour
    {
        #region HttpUpdateDelegator 公共属性

        /// <summary>
        /// HTTPUpdateDelegator的单例实例
        /// </summary>
        private static HttpUpdateDelegator Instance { get; set; }

        /// <summary>
        /// 如果Instance属性应保留有效值，则为true。
        /// </summary>
        public static bool IsCreated { get; private set; }

        /// <summary>
        /// 在任何CheckInstance()调用之前，或在任何请求发送到另一个线程上分派回调之前，将其设置为true。
        /// </summary>
        private static bool IsThreaded { get; set; }

        /// <summary>
        /// 如果调度线程正在运行，这是正确的。
        /// </summary>
        private static bool IsThreadRunning { get; set; }

        /// <summary>
        /// 插件在两次更新调用之间应该等待多长时间。默认值为100毫秒。
        /// </summary>
        private static int ThreadFrequencyInMS { get; set; }

        /// <summary>
        /// 在OnApplicationQuit函数中调用。如果这个函数返回False，插件将不会自动关闭。
        /// </summary>
#pragma warning disable CS0649
        private static System.Func<bool> _onBeforeApplicationQuit;
#pragma warning restore CS0649

        public static System.Action<bool> OnApplicationForegroundStateChanged;

        #endregion

        private static bool _isSetupCalled;
        private int _isHttpManagerOnUpdateRunning;

#if UNITY_EDITOR
        /// <summary>
        /// 在场景加载后调用以支持可配置的进入播放模式 (https://docs.unity3d.com/2019.3/Documentation/Manual/ConfigurableEnterPlayMode.html)
        /// </summary>
#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        static void ResetSetup()
        {
            _isSetupCalled = false;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("[HTTPUpdateDelegator]  [msg:重置 called!]");
#endif
            HttpManager.Logger.Information("HTTPUpdateDelegator", "Reset called!");
        }
#endif

        static HttpUpdateDelegator()
        {
            ThreadFrequencyInMS = 100;
        }

        /// <summary>
        /// 将创建HTTPUpdateDelegator实例并对其进行设置。
        /// </summary>
        public static void CheckInstance()
        {
            try
            {
                if (!IsCreated)
                {
                    GameObject go = GameObject.Find("HTTP Update Delegator");

                    if (go != null)
                        Instance = go.GetComponent<HttpUpdateDelegator>();

                    if (Instance == null)
                    {
                        go = new GameObject("HTTP Update Delegator")
                        {
                            hideFlags = HideFlags.HideAndDontSave
                        };

                        Instance = go.AddComponent<HttpUpdateDelegator>();
                    }

                    IsCreated = true;

#if UNITY_EDITOR
                    if (!UnityEditor.EditorApplication.isPlaying)
                    {
                        UnityEditor.EditorApplication.update -= Instance.Update;
                        UnityEditor.EditorApplication.update += Instance.Update;
                    }

#if UNITY_2017_2_OR_NEWER
                    UnityEditor.EditorApplication.playModeStateChanged -= Instance.OnPlayModeStateChanged;
                    UnityEditor.EditorApplication.playModeStateChanged += Instance.OnPlayModeStateChanged;
#else
                    UnityEditor.EditorApplication.playmodeStateChanged -= Instance.OnPlayModeStateChanged;
                    UnityEditor.EditorApplication.playmodeStateChanged += Instance.OnPlayModeStateChanged;
#endif
#endif

                    // https://docs.unity3d.com/ScriptReference/Application-wantsToQuit.html
                    Application.wantsToQuit -= UnityApplication_WantsToQuit;
                    Application.wantsToQuit += UnityApplication_WantsToQuit;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.Log($"[HTTPUpdateDelegator] [msg:实例创建!]");
#endif
                    // HttpManager.Logger.Information("HTTPUpdateDelegator", "");
                }
            }
            catch
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log($"[HTTPUpdateDelegator] [msg:请从Unity的事件中调用BestHttp.HTTPManager.Setup()。在发送任何请求之前，请先清醒，开始)!]");
#endif
                // HttpManager.Logger.Error("HTTPUpdateDelegator", "请从Unity的事件中调用BestHttp.HTTPManager.Setup()。在发送任何请求之前，请先清醒，开始)!");
            }
        }

        public void SwapThreadingMode()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (IsThreaded)
            {
                IsThreadRunning = false;
                IsThreaded = false;
            }
            else
            {
                IsThreaded = true;
                PlatformSupport.Threading.ThreadedRunner.RunLongLiving(ThreadFunc);
            }
#endif
        }

        private void Setup()
        {
            if (_isSetupCalled)
                return;
            _isSetupCalled = true;

            HttpManager.Setup();

#if UNITY_WEBGL && !UNITY_EDITOR
            // 线程在WEBGL构建中没有实现，现在禁用它。
            IsThreaded = false;
#endif
            if (IsThreaded)
            {
                PlatformSupport.Threading.ThreadedRunner.RunLongLiving(ThreadFunc);
            }

            // Unity不能很好地容忍DontDestroyOnLoad在纯编辑模式下调用。因此，我们将只在播放时设置标志，或者不在编辑器中设置。
            if (!Application.isEditor || Application.isPlaying)
            {
                DontDestroyOnLoad(this.gameObject);
            }

            HttpManager.Logger.Information("HTTPUpdateDelegator", "Setup done!");
        }

        void ThreadFunc()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("[HTTPUpdateDelegator] [msg:更新线程已启动]");
#endif
            // HttpManager.Logger.Information("HTTPUpdateDelegator", "Update Thread Started");

            try
            {
                IsThreadRunning = true;
                while (IsThreadRunning)
                {
                    CallOnUpdate();

#if NETFX_CORE
	                await Task.Delay(ThreadFrequencyInMS);
#else
                    Thread.Sleep(ThreadFrequencyInMS);
#endif
                }
            }
            finally
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log("[HTTPUpdateDelegator] [msg:更新线程结束]");
#endif
                // HttpManager.Logger.Information("HTTPUpdateDelegator", "Update Thread Ended");
            }
        }

        void Update()
        {
            if (!_isSetupCalled)
                Setup();

            if (!IsThreaded)
                CallOnUpdate();
        }

        private void CallOnUpdate()
        {
            // 防止OnUpdate从unity主线程和单独的线程中重复调用
            if (Interlocked.CompareExchange(ref _isHttpManagerOnUpdateRunning, 1, 0) == 0)
            {
                try
                {
                    HttpManager.OnUpdate();
                }
                finally
                {
                    Interlocked.Exchange(ref _isHttpManagerOnUpdateRunning, 0);
                }
            }
        }

#if UNITY_EDITOR
#if UNITY_2017_2_OR_NEWER
        void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange playMode)
        {
            if (playMode == UnityEditor.PlayModeStateChange.EnteredPlayMode)
            {
                UnityEditor.EditorApplication.update -= Update;
            }
            else if (playMode == UnityEditor.PlayModeStateChange.EnteredEditMode)
            {
                UnityEditor.EditorApplication.update -= Update;
                UnityEditor.EditorApplication.update += Update;

                HttpUpdateDelegator.ResetSetup();
                HttpManager.ResetSetup();
            }
        }
#else
        void OnPlayModeStateChanged()
        {
            if (UnityEditor.EditorApplication.isPlaying)
                UnityEditor.EditorApplication.update -= Update;
            else if (!UnityEditor.EditorApplication.isPlaying)
                UnityEditor.EditorApplication.update += Update;
        }

#endif
#endif

        void OnDisable()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("[HTTPUpdateDelegator] [msg:OnDisable Called!]");
#endif
            //HttpManager.Logger.Information("HTTPUpdateDelegator", "OnDisable Called!");

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
#endif
                UnityApplication_WantsToQuit();
        }

        void OnApplicationPause(bool isPaused)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[HTTPUpdateDelegator] [msg:OnApplicationPause 是否暂停-->[{isPaused}]]");
#endif
            // HttpManager.Logger.Information("HTTPUpdateDelegator", "OnApplicationPause isPaused: " + isPaused);

            if (HttpUpdateDelegator.OnApplicationForegroundStateChanged != null)
            {
                HttpUpdateDelegator.OnApplicationForegroundStateChanged(isPaused);
            }
        }

        private static bool UnityApplication_WantsToQuit()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[HTTPUpdateDelegator] [msg:UnityApplication_WantsToQuit Called!!!]");
#endif
            //HttpManager.Logger.Information("HTTPUpdateDelegator", "UnityApplication_WantsToQuit Called!");

            if (_onBeforeApplicationQuit != null)
            {
                try
                {
                    if (!_onBeforeApplicationQuit())
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log($"[HTTPUpdateDelegator] [msg:OnBeforeApplicationQuit调用返回false，延迟插件和应用程序关闭。]");
#endif
                        // HttpManager.Logger.Information("HTTPUpdateDelegator",
                        //     "OnBeforeApplicationQuit call returned false, postponing plugin and application shutdown.");
                        return false;
                    }
                }
                catch (System.Exception ex)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.Log($"[HTTPUpdateDelegator] [msg:{ex}]");
#endif
                    //              HttpManager.Logger.Exception("HTTPUpdateDelegator", string.Empty, ex);
                }
            }

            IsThreadRunning = false;

            if (!IsCreated)
                return true;

            IsCreated = false;

            HttpManager.OnQuit();

            return true;
        }
    }
}