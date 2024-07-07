using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using BestHTTP.PlatformSupport.IL2CPP;
using BestHTTP.PlatformSupport.Threading;
using UnityEditor;
using UnityEngine;
#if NETFX_CORE
    using System.Threading.Tasks;
#endif
using Debug = FrostEngine.Debug;

// ReSharper disable once CheckNamespace
namespace BestHTTP
{
    /// <summary>
    /// 将一些U3D调用路由到HTTPManager。
    /// </summary>
    [ExecuteInEditMode]
    [Il2CppEagerStaticClassConstruction]
    public sealed class HttpUpdateDelegator : MonoBehaviour
    {
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
        private static Func<bool> _onBeforeApplicationQuit;
#pragma warning restore CS0649

        public static Action<bool> OnApplicationForegroundStateChanged;


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
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            Debug.Log("[HTTPUpdateDelegator]  [msg:重置 called!]");
#endif
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
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}]");
                    sb.Append($"{sf.GetMethod().Name}");
                    sb.Append($"Line:{sf.GetFileLineNumber()}");
                    sb.Append($"[msg] 将创建HTTPUpdateDelegator实例并对其进行设置。IsCreated:{IsCreated}");
                    Debug.Log($"{sb}");
                }
                if (IsCreated) return;
                var go = GameObject.Find("HTTP Update Delegator");

                if (go != null)
                {
                    Instance = go.GetComponent<HttpUpdateDelegator>();
                }

                if (Instance == null)
                {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                    {
                        var st = new StackTrace(new StackFrame(true));
                        var sf = st.GetFrame(0);
                        var sb = new StringBuilder(6);
                        sb.Append($"[{sf.GetFileName()}]");
                        sb.Append($"[method:{sf.GetMethod().Name}]");
                        sb.Append($"{sf.GetMethod().Name}");
                        sb.Append($"Line:{sf.GetFileLineNumber()}");
                        sb.Append("[msg] Instance 为空，创建 HTTP Update Delegator 物体");
                        Debug.Log($"{sb}");
                    }
#endif
                    go = new GameObject("HTTP Update Delegator")
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };

                    Instance = go.AddComponent<HttpUpdateDelegator>();
                }

                IsCreated = true;

#if UNITY_EDITOR
                if (!EditorApplication.isPlaying)
                {
                    // UnityEditor.EditorApplication.update += Instance.Update;
                    EditorApplication.update -= Instance.Update;
                }

#if UNITY_2017_2_OR_NEWER
                EditorApplication.playModeStateChanged += Instance.OnPlayModeStateChanged;
                EditorApplication.playModeStateChanged -= Instance.OnPlayModeStateChanged;
#else
                    UnityEditor.EditorApplication.playmodeStateChanged -= Instance.OnPlayModeStateChanged;
                    UnityEditor.EditorApplication.playmodeStateChanged += Instance.OnPlayModeStateChanged;
#endif
#endif

                // https://docs.unity3d.com/ScriptReference/Application-wantsToQuit.html
                Application.wantsToQuit -= UnityApplication_WantsToQuit;
                Application.wantsToQuit += UnityApplication_WantsToQuit;
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}]");
                    sb.Append($"{sf.GetMethod().Name}");
                    sb.Append($"Line:{sf.GetFileLineNumber()}");
                    sb.Append("[msg] 实例创建。");
                    Debug.Log($"{sb}");
                }
#endif
                // HttpManager.Logger.Information("HTTPUpdateDelegator", "");
            }
            catch
            {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}]");
                    sb.Append($"{sf.GetMethod().Name}");
                    sb.Append($"Line:{sf.GetFileLineNumber()}");
                    sb.Append("[msg] 请从Unity的事件中调用BestHttp.HTTPManager.Setup()。在发送任何请求之前，请先清醒，开始)!");
                    Debug.LogError($"{sb}");
                }
#endif
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
                ThreadedRunner.RunLongLiving(ThreadFunc);
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
                ThreadedRunner.RunLongLiving(ThreadFunc);
            }

            // Unity不能很好地容忍DontDestroyOnLoad在纯编辑模式下调用。因此，我们将只在播放时设置标志，或者不在编辑器中设置。
            if (!Application.isEditor || Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}]");
                sb.Append($"{sf.GetMethod().Name}");
                sb.Append($"Line:{sf.GetFileLineNumber()}");
                sb.Append($"[msg{sf.GetMethod().Name}] Setup done!");
                Debug.LogError($"{sb}");
            }
#endif
        }

        private void ThreadFunc()
        {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}] 更新线程已启动");
                Debug.Log($"{sb}");
            }
#endif
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
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}] ");
                    sb.Append($"{sf.GetMethod().Name} ");
                    sb.Append($"Line:{sf.GetFileLineNumber()} ");
                    sb.Append($"[msg{sf.GetMethod().Name}] 更新线程结束");
                    Debug.Log($"{sb}");
                }
#endif
            }
        }

        private void Update()
        {
            if (!_isSetupCalled)
            {
                Setup();
            }

            if (!IsThreaded)
            {
                CallOnUpdate();
            }
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
        void OnPlayModeStateChanged(PlayModeStateChange playMode)
        {
            if (playMode == PlayModeStateChange.EnteredPlayMode)
            {
                EditorApplication.update -= Update;
            }
            else if (playMode == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.update -= Update;
                EditorApplication.update += Update;

                ResetSetup();
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
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}] OnDisable 调用!");
                Debug.Log($"{sb}");
            }
#endif
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
#endif
            {
                UnityApplication_WantsToQuit();
            }
        }

        void OnApplicationPause(bool isPaused)
        {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}] OnApplicationPause 是否暂停-->[{isPaused}]");
                Debug.Log($"{sb}");
            }
#endif
            if (OnApplicationForegroundStateChanged != null)
            {
                OnApplicationForegroundStateChanged(isPaused);
            }
        }

        private static bool UnityApplication_WantsToQuit()
        {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}] ");
                sb.Append($"{sf.GetMethod().Name} ");
                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                sb.Append($"[msg{sf.GetMethod().Name}] UnityApplication_WantsToQuit Called!!!");
                Debug.Log($"{sb}");
            }
#endif
            if (_onBeforeApplicationQuit != null)
            {
                try
                {
                    if (!_onBeforeApplicationQuit())
                    {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                        {
                            var st = new StackTrace(new StackFrame(true));
                            var sf = st.GetFrame(0);
                            var sb = new StringBuilder(6);
                            sb.Append($"[{sf.GetFileName()}]");
                            sb.Append($"[method:{sf.GetMethod().Name}] ");
                            sb.Append($"{sf.GetMethod().Name} ");
                            sb.Append($"Line:{sf.GetFileLineNumber()} ");
                            sb.Append($"[msg{sf.GetMethod().Name}] OnBeforeApplicationQuit调用返回false，延迟插件和应用程序关闭。");
                            Debug.Log($"{sb}");
                        }
#endif
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    // ignored
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                    {
                        var st = new StackTrace(new StackFrame(true));
                        var sf = st.GetFrame(0);
                        var sb = new StringBuilder(6);
                        sb.Append($"[{sf.GetFileName()}]");
                        sb.Append($"[method:{sf.GetMethod().Name}] ");
                        sb.Append($"{sf.GetMethod().Name} ");
                        sb.Append($"Line:{sf.GetFileLineNumber()} ");
                        sb.Append($"[msg{sf.GetMethod().Name}] {ex}");
                        Debug.LogError($"{sb}");
                    }
#endif
                }
            }

            IsThreadRunning = false;

            if (!IsCreated)
            {
                return true;
            }

            IsCreated = false;

            HttpManager.OnQuit();

            return true;
        }
    }
}