using System;
using System.IO;
using UnityEngine;
using XLua;
using YooAsset;

namespace FrostEngine
{
    [DisallowMultipleComponent]
    public sealed class XLuaModule : Module
    {
        Action<float, float> luaUpdate = null;
        Action luaLateUpdate = null;
        Action<float> luaFixedUpdate = null;

        /// <summary>
        /// lua 虚拟机
        /// </summary>
        LuaEnv luaEnv = null;

        private ResourceModule _mResourceManager;

        public void Start()
        {
            RootModules rootModule = ModuleSystem.GetModule<RootModules>();
            if (rootModule == null)
            {
                Debug.Fatal("Root module is invalid.");
                return;
            }

            _mResourceManager = ModuleSystem.GetModule<ResourceModule>();
            if (_mResourceManager == null)
            {
                Debug.Fatal("Resource module is invalid.");
                return;
            }

            luaEnv = new LuaEnv();
            luaEnv.AddLoader(CustomLoader);
        }

        public LuaEnv GetLuaEnv()
        {
            return luaEnv;
        }

        public void InitLuaEnv()
        {
            if (luaEnv == null)
            {
                Debug.Fatal("luaEnv  is invalid.");
                return;
            }

            LoadScript("main");
        }

        public byte[] CustomLoader(ref string filePath)
        {
            string scriptPath = string.Empty;
            var luaFilePath = filePath.Replace(".", "/") + ".lua";
            if (_mResourceManager.PlayMode == EPlayMode.EditorSimulateMode)
            {
                Debug.Log($"Load file lua script:{luaFilePath}");
                string source = Path.Combine(Application.dataPath, $"/GameScript/Lua/{luaFilePath}");
                DirectoryInfo dirInfo = new DirectoryInfo(source);
                scriptPath = dirInfo.FullName;
                Debug.Log($"scriptPath:{scriptPath}");
                var textLua = Utility.GetFileBytes(scriptPath);
                return textLua;
            }

            return null;
        }

        private void LoadScript(string scriptName)
        {
            Debug.Log($"load script:{scriptName}");
            SafeDoString($"require('{scriptName}')");
        }

        public void SafeDoString(string scriptContent, string chunkName = "chunk")
        {
            if (luaEnv != null)
            {
                try
                {
                    luaEnv.DoString(scriptContent, chunkName);
                }
                catch (Exception ex)
                {
                    var msg = $"xLua exception : {ex.Message}\n {ex.StackTrace}";
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError(msg);
#endif
                }
            }
        }

        private void Update()
        {
            if (luaEnv != null)
            {
                luaEnv.Tick();
                if (luaUpdate != null)
                {
                    try
                    {
                        luaUpdate(Time.deltaTime, Time.unscaledDeltaTime);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("luaUpdate err : " + ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (luaLateUpdate != null)
            {
                try
                {
                    luaLateUpdate();
                }
                catch (Exception ex)
                {
                    Debug.LogError("luaLateUpdate err : " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        private void FixedUpdate()
        {
            if (luaFixedUpdate != null)
            {
                try
                {
                    luaFixedUpdate(Time.fixedDeltaTime);
                }
                catch (Exception ex)
                {
                    Debug.LogError("luaFixedUpdate err : " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        //当退出游戏的时候调用
        public void OnDestroy()
        {
            Debug.Log("关闭lua虚拟机");
            luaEnv.Dispose();
            luaEnv = null;
        }

        public void CallLuaFunction(string funcName)
        {
            var luaFunc = luaEnv.LoadString(funcName);
            luaFunc.Call();
        }
    }
}