using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CustomDataStruct
{
    static public class Helper
    {
        public static void Startup()
        {
            CustomDataStructHelper.Instance.Startup();
        }

        public static void Cleanup()
        {
#if UNITY_EDITOR
            Debug.Log("CustomDataStruct Cleanup!");
#endif
            BetterDelegate.Cleanup();
            BetterStringBuilder.Cleanup();
            ValueObject.Cleanup();
            ObjPoolBase.Cleanup();
#if UNITY_EDITOR
            MemoryLeakDetecter.Cleanup();
#endif
        }

#if UNITY_EDITOR
        public static void ClearDetecterUsingData()
        {
            List<MemoryLeakDetecter> deteters = MemoryLeakDetecter.detecters;
            for (int i = 0; i < deteters.Count; i++)
            {
                deteters[i].ClearUsingData();
            }
        }
#endif

        public static string HandleTypeFullName(string name)
        {
            string[] list = name.Split(',');
            StringBuilder sb = new StringBuilder();
            foreach (var cur in list)
            {
                if (cur.Contains("Assembly") || cur.Contains("mscorlib") || cur.Contains("Version") ||
                    cur.Contains("Culture")) continue;
                if (cur.Contains("PublicKeyToken"))
                {
                    var startIndex = cur.IndexOf(']');
                    if (startIndex >= 0)
                    {
                        sb.Append(cur.Substring(startIndex));
                    }
                }
                else
                {
                    sb.Append(cur);
                }
            }
            return sb.ToString();
        }
    }

    sealed class CustomDataStructHelper : MonoSingleton<CustomDataStructHelper>
    {
#if UNITY_EDITOR
        private const float LOGInterval = 1.0f;
        public bool debug = true;
        public bool log = false;
        private float _nextLogTime = 0f;
#endif
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
            _nextLogTime = Time.realtimeSinceStartup + LOGInterval;
#endif
        }

#if UNITY_EDITOR
        void Update()
        {
            if (debug)
            {
                List<MemoryLeakDetecter> deteters = MemoryLeakDetecter.detecters;
                foreach (var t in deteters)
                {
                    t.DetectMemoryLeaks();
                }
            }

            log = debug ? log : debug;
            if (!log || !(_nextLogTime < Time.realtimeSinceStartup)) return;
            {
                StringBuilder sb = new StringBuilder();
                _nextLogTime = Time.realtimeSinceStartup + LOGInterval;
                List<MemoryLeakDetecter> deteters = MemoryLeakDetecter.detecters;
                foreach (var t in deteters)
                {
                    sb.AppendLine(t.ToLogString());
                }
                Debug.Log(sb.ToString());
            }
        }
#endif

        public void OnLevelWasLoaded()
        {
            Helper.Cleanup();
        }
    }
}