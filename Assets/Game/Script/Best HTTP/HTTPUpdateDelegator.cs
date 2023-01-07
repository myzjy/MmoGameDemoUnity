using System.Threading;
using UnityEngine;
#if NETFX_CORE
    using System.Threading.Tasks;
#endif

namespace BestHTTP
{
    /// <summary>
    /// Will route some U3D calls to the HTTPManager.
    /// </summary>
    [ExecuteInEditMode]
    [BestHTTP.PlatformSupport.IL2CPP.Il2CppEagerStaticClassConstructionAttribute]
    public sealed class HttpUpdateDelegator : MonoBehaviour
    {
        #region Public Properties

        /// <summary>
        /// The singleton instance of the HTTPUpdateDelegator
        /// </summary>
        public static HttpUpdateDelegator Instance { get; private set; }

        /// <summary>
        /// True, if the Instance property should hold a valid value.
        /// </summary>
        public static bool IsCreated { get; private set; }

        /// <summary>
        /// Set it true before any CheckInstance() call, or before any request sent to dispatch callbacks on another thread.
        /// </summary>
        public static bool IsThreaded { get; set; }

        /// <summary>
        /// It's true if the dispatch thread running.
        /// </summary>
        public static bool IsThreadRunning { get; private set; }

        /// <summary>
        /// How much time the plugin should wait between two update call. Its default value 100 ms.
        /// </summary>
        public static int ThreadFrequencyInMS { get; set; }

        /// <summary>
        /// Called in the OnApplicationQuit function. If this function returns False, the plugin will not start to
        /// shut down itself.
        /// </summary>
        public static System.Func<bool> OnBeforeApplicationQuit;

        public static System.Action<bool> OnApplicationForegroundStateChanged;

        #endregion

        private static bool _isSetupCalled;
        private int _isHttpManagerOnUpdateRunning;

#if UNITY_EDITOR
        /// <summary>
        /// Called after scene loaded to support Configurable Enter Play Mode (https://docs.unity3d.com/2019.3/Documentation/Manual/ConfigurableEnterPlayMode.html)
        /// </summary>
#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        static void ResetSetup()
        {
            _isSetupCalled = false;
            HttpManager.Logger.Information("HTTPUpdateDelegator", "Reset called!");
        }
#endif

        static HttpUpdateDelegator()
        {
            ThreadFrequencyInMS = 100;
        }

        /// <summary>
        /// Will create the HTTPUpdateDelegator instance and set it up.
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
                        go = new GameObject("HTTP Update Delegator");
                        go.hideFlags = HideFlags.HideAndDontSave;

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

                    HttpManager.Logger.Information("HTTPUpdateDelegator", "Instance Created!");
                }
            }
            catch
            {
                HttpManager.Logger.Error("HTTPUpdateDelegator",
                    "Please call the BestHTTP.HTTPManager.Setup() from one of Unity's event(eg. awake, start) before you send any request!");
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
            // Threads are not implemented in WEBGL builds, disable it for now.
            IsThreaded = false;
#endif
            if (IsThreaded)
                PlatformSupport.Threading.ThreadedRunner.RunLongLiving(ThreadFunc);

            // Unity doesn't tolerate well if the DontDestroyOnLoad called when purely in editor mode. So, we will set the flag
            //  only when we are playing, or not in the editor.
            if (!Application.isEditor || Application.isPlaying)
                GameObject.DontDestroyOnLoad(this.gameObject);

            HttpManager.Logger.Information("HTTPUpdateDelegator", "Setup done!");
        }

        void ThreadFunc()
        {
            HttpManager.Logger.Information("HTTPUpdateDelegator", "Update Thread Started");

            try
            {
                IsThreadRunning = true;
                while (IsThreadRunning)
                {
                    CallOnUpdate();

#if NETFX_CORE
	                await Task.Delay(ThreadFrequencyInMS);
#else
                    System.Threading.Thread.Sleep(ThreadFrequencyInMS);
#endif
                }
            }
            finally
            {
                HttpManager.Logger.Information("HTTPUpdateDelegator", "Update Thread Ended");
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
            // Prevent overlapping call of OnUpdate from unity's main thread and a separate thread
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
            HttpManager.Logger.Information("HTTPUpdateDelegator", "OnDisable Called!");

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
#endif
                UnityApplication_WantsToQuit();
        }

        void OnApplicationPause(bool isPaused)
        {
            HttpManager.Logger.Information("HTTPUpdateDelegator", "OnApplicationPause isPaused: " + isPaused);

            if (HttpUpdateDelegator.OnApplicationForegroundStateChanged != null)
                HttpUpdateDelegator.OnApplicationForegroundStateChanged(isPaused);
        }

        private static bool UnityApplication_WantsToQuit()
        {
            HttpManager.Logger.Information("HTTPUpdateDelegator", "UnityApplication_WantsToQuit Called!");

            if (OnBeforeApplicationQuit != null)
            {
                try
                {
                    if (!OnBeforeApplicationQuit())
                    {
                        HttpManager.Logger.Information("HTTPUpdateDelegator",
                            "OnBeforeApplicationQuit call returned false, postponing plugin and application shutdown.");
                        return false;
                    }
                }
                catch (System.Exception ex)
                {
                    HttpManager.Logger.Exception("HTTPUpdateDelegator", string.Empty, ex);
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