using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X9;
using Unity.VisualScripting;
using UnityEngine;
using XLua;
using ZJYFrameWork.AssetBundles.IAssetBundlesManagerInterface;
using ZJYFrameWork.Spring.Core;
using Object = UnityEngine.Object;

namespace ZJYFrameWork.Common
{
    // [LuaCallCSharp]
    public class CommonManager : MonoBehaviour
    {
        private static CommonManager instance = null;

        public static CommonManager Instance
        {
            get
            {
                Debug.Log($"{instance}");
                return instance;
            }
            set { instance = value; }
        }

        private void Awake()
        {
            instance = this;
        }

        public Object LoadAsset(string assetPath)
        {
            return SpringContext.GetBean<IAssetBundleManager>().LoadAsset(assetPath);
        }

        public void LoadAsset(string assetPath, Action<Object> action)
        {
            SpringContext.GetBean<IAssetBundleManager>().LoadAsset(assetPath, action);
        }

        public GameObject LoadAssetGameObject(string assetPath)
        {
            Debug.Log($"读取资源：{assetPath}");
            return SpringContext.GetBean<IAssetBundleManager>().LoadAssetGameObject(assetPath);
        }

        public int DebugConfig()
        {
#if UNITY_EDITOR
            return 1;
#elif DEVELOP_BUILD && ENABLE_LOG
            return 2;
#else
            return 0;
#endif
        }
    }
}