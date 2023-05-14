using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;
using ZJYFrameWork.AssetBundles.AssetBundlesManager;
using ZJYFrameWork.AssetBundles.AssetBundleToolsConfig;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.IAssetBundlesManagerInterface;
using ZJYFrameWork.Base.Component;
using ZJYFrameWork.Config;
using ZJYFrameWork.Spring.Core;
using Object = UnityEngine.Object;
using Path = System.IO.Path;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZJYFrameWork.XLuaScript
{
    [Hotfix]
    [LuaCallCSharp]
    public class XLuaManager : SpringComponent
    {
        Action<float, float> luaUpdate = null;
        Action luaLateUpdate = null;
        Action<float> luaFixedUpdate = null;

        /// <summary>
        /// lua 虚拟机
        /// </summary>
        LuaEnv luaEnv = null;

        // Action onLoginOk = null;

        protected override void OnAwake()
        {
            base.OnAwake();
            // InitLuaEnv();
        }

        public void InitLuaEnv()
        {
            luaEnv = new LuaEnv();
            if (luaEnv != null)
            {
                //虚拟机
                luaEnv.AddLoader(CustomLoader);
                LoadScript("BaseRequire");
                // LoadScript("Common.util.event");
                luaUpdate = luaEnv.Global.Get<Action<float, float>>("Update");
                luaLateUpdate = luaEnv.Global.Get<Action>("LateUpdate");
                luaFixedUpdate = luaEnv.Global.Get<Action<float>>("FixedUpdate");
                LoadScript("LuaMain");
                SafeDoString("LuaMain()");
            }
        }

        public LuaEnv GetLuaEnv()
        {
            return luaEnv;
        }

        public static byte[] CustomLoader(ref string filePath)
        {
            string scriptPath = string.Empty;
            filePath = filePath.Replace(".", "/") + ".lua";
#if UNITY_EDITOR
            if (AssetBundleConfig.IsEditorMode)
            {
                // string destination = Path.Combine(Application.dataPath, $"{AppConfig.GameLuaPath}");
                // scriptPath = $"{AppConfig.AssetsGameLuaPath}{filePath}";
                string source =
 Path.Combine(Application.dataPath, $"../{AssetBundleConfig.luaAssetbundleAssetName}/{filePath}");
                Debug.Log($"source:{source}");
                DirectoryInfo dirInfo = new DirectoryInfo(source);
                Debug.Log($"dirInfo:{dirInfo.FullName}");
                scriptPath = dirInfo.FullName;
                var textLua = Util.GetFileBytes(scriptPath);
                return textLua;
            }
#endif
            IBundle bundle;
            bundle = SpringContext.GetBean<AssetBundleManager>()
                .LoadXLuaAssetBundle(AppConfig.XLuaAssetBundleName, res => { bundle = res; });
            Debug.Log($"filePath:{filePath},{bundle}");

            var luaAsset = bundle.LoadAsset<TextAsset>(filePath);
            return luaAsset.bytes;
        }

        private void LoadScript(string scriptName)
        {
            SafeDoString($"require('{scriptName}')");
        }

        public void LoadOutsideFile(string filePath)
        {
            SafeDoString(File.ReadAllText(filePath));
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
    }

    public class CSSharpXLua
    {
#if UNITY_EDITOR
    [CSharpCallLua] public static List<Type> CSharpCallLua = new List<Type>()
    {
        typeof(Action),
        typeof(Action<byte[]>),
        typeof(Action<float>),
        typeof(Action<float, float>),
    };
#endif
    }
}