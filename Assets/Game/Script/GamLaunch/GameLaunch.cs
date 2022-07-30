using System;
using System.Collections;
using System.Collections.Generic;
using Common.GameChannel;
using Common.Utility;
using Framework.AssetBundles.Config;
using Framework.AssetBundles.Utilty;
using Script.Framework.AssetBundle;
using Script.Framework.UI.Tip;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork.UISerializable.Framwork.UIRootCS;

public class GameLaunch : MonoBehaviour
{
    AssetBundleUpdater updater;
    const string noticeTipPrefabPath = "UI/Prefab/UINoticeTip.prefab";

    private const string launchPrefabPath = "uilaunch"; //"loginpanel";//"UI/Prefab/view/LauncPanel.prefab";

    // const string launchPrefabPath = "uilaunch";//"UI/Prefab/view/LauncPanel.prefab";
    GameObject launchPrefab;

    GameObject noticeTipPrefab;
    [SerializeField] private UIRoot root;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        //初始化版本号
        /*
         * 此处的时间是向服务器请求的，不能用本机时间
         */
        var startTime = DateTime.Now;
        yield return InitAppVersion();
        ToolsDebug.Log($"InitAppVersion use {(DateTime.Now - startTime).Milliseconds}ms");
        // 启动资源管理模块
        startTime = DateTime.Now;
        yield return AssetBundleManager.Instance.Initialize();
        ToolsDebug.Log($"AssetBundleManager Initialize use {(DateTime.Now - startTime).Milliseconds}ms");
      
        startTime = DateTime.Now;
        ToolsDebug.Log($"XLuaManager StartHotfix use {(DateTime.Now - startTime).Milliseconds}ms");

        // var data = AssetDatabase.GetAssetPathsFromAssetBundle(launchPrefabPath + AssetBundleConfig.AssetBundleSuffix);
        yield return InitLaunchPrefab();
        yield return InitNoticeTipPrefab();

        if (updater != null)
        {
            updater.StartCheckUpdate();
        }

        // root.StartRoot();
    }

    private IEnumerator InitAppVersion()
    {
        var appVersionRequest = AssetBundleManager.Instance.RequestAssetFileAsync(BuildUtils.AppVersionFileName);
        // ReSharper disable once AccessToDisposedClosure
        yield return new WaitUntil(() => appVersionRequest.isDone);
        var streamingAppVersion = appVersionRequest.text;
        appVersionRequest.Dispose();

        var appVersionPath = AssetBundleUtility.GetPersistentDataPath(BuildUtils.AppVersionFileName);
        var persistentAppVersion = GameUtility.SafeReadAllText(appVersionPath);
        ToolsDebug.Log($"streamingAppVersion = {streamingAppVersion}, persistentAppVersion = {persistentAppVersion}");

        // 如果persistent目录版本比streamingAssets目录app版本低，说明是大版本覆盖安装，清理过时的缓存
        if (!string.IsNullOrEmpty(persistentAppVersion) &&
            BuildUtils.CheckIsNewVersion(persistentAppVersion, streamingAppVersion))
        {
            var path = AssetBundleUtility.GetPersistentDataPath();
            GameUtility.SafeDeleteDir(path);
        }

        GameUtility.SafeWriteAllText(appVersionPath, streamingAppVersion);
        // ChannelManager.instance.appVersion = streamingAppVersion;
    }

    //提示框
    IEnumerator InitNoticeTipPrefab()
    {
        var start = DateTime.Now;
        var loader = AssetBundleManager.Instance.LoadAssetAsync(noticeTipPrefabPath, typeof(GameObject));
        yield return loader;
        noticeTipPrefab = loader.asset as GameObject;
        ToolsDebug.Log($"Load noticeTipPrefab use {(DateTime.Now - start).Milliseconds}ms");
        loader.Dispose();
        if (noticeTipPrefab == null)
        {
            ToolsDebug.LogError("LoadAssetAsync noticeTipPrefab err : " + noticeTipPrefabPath);
            yield break;
        }

        var go = InstantiateGameObject(noticeTipPrefab);
        UINoticeTip.Instance.UIGameObject = go;

        yield break;
    }

    IEnumerator InitChannel()
    {
#if UNITY_EDITOR
        if (AssetBundleConfig.IsEditorMode)
        {
            yield break;
        }
#endif
        var channelNameRequest = AssetBundleManager.Instance.RequestAssetFileAsync(BuildUtils.ChannelNameFileName);
        yield return channelNameRequest;
        var channelName = channelNameRequest.text;
        channelNameRequest.Dispose();
        ChannelManager.Instance.Init(channelName);
        ToolsDebug.Log($"channelName = {channelName}");
    }

    IEnumerator InitLaunchPrefab()
    {
        var start = DateTime.Now;
        var loader = AssetBundleManager.Instance.LoadAssetAsync(launchPrefabPath, typeof(GameObject));
        yield return loader;
        launchPrefab = loader.asset as GameObject;
        ToolsDebug.Log($"Load launchPrefab use {(DateTime.Now - start).Milliseconds}ms");
        loader.Dispose();
        if (launchPrefab == null)
        {
            ToolsDebug.LogError("LoadAssetAsync launchPrefab err : " + launchPrefabPath);
            yield break;
        }

        var go = InstantiateGameObject(launchPrefab);
        updater = go.AddComponent<AssetBundleUpdater>();
        yield break;
    }

    GameObject InstantiateGameObject(GameObject prefab)
    {
        var luanchLayer = GameObject.Find("UIRoot/LuanchLayer");
        var start = DateTime.Now;
        GameObject go = GameObject.Instantiate(prefab);
        ToolsDebug.Log($"Instantiate use {(DateTime.Now - start).Milliseconds}ms");
        go.transform.parent = luanchLayer.transform;
        var rectTransform = go.GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localPosition = Vector3.zero;

        return go;
    }
}