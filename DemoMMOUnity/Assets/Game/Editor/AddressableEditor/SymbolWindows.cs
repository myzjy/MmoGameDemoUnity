using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork.WebRequest;

#if UNITY_EDITOR

public class SymbolWindows : EditorWindow
{
    private const string ITEMTOOLS = "Tools/Symbol";
    private const string SymbolString_WINDOWS = "Symbol";

    private class SymbolData
    {
        public string name { get; private set; }
        public string des { get; private set; }
        public bool IsEnable { get; set; }

        public SymbolData(string name, string des)
        {
            this.name = name.ToUpper();
            this.des = des;
        }
    }

    private static  List<SymbolData> _mSymbolDataList = new List<SymbolData>()
    {
        new SymbolData("LOGGER_ON", "LOG"),
        new SymbolData("DEVELOP_BUILD", "开发模式"),
        new SymbolData("DISABLE_SERVERSENT_EVENTS", ""),
        new SymbolData("BESTHTTP_DISABLE_SIGNALR", ""),
        new SymbolData("BESTHTTP_DISABLE_SOCKETIO", ""),
        new SymbolData("BESTHTTP_DISABLE_UNITY_FORM", ""),
        new SymbolData("BESTHTTP_DISABLE_SIGNALR_CORE", ""),
        new SymbolData("ENABLE_DEBUG_LOG", ""),
        new SymbolData("ENABLE_LOG", ""),
        new SymbolData("ENABLE_DEBUG_AND_ABOVE_LOG", ""),
        new SymbolData("ASSET_BUNDLE_DEVELOP_EDITOR", "AssetBundles编辑模式"),
        new SymbolData("HOTFIX_ENABLE", "xlua"),
        new SymbolData("ENABLE_LOG_NETWORK","网络相关 log")

    };


    [MenuItem(ITEMTOOLS)]
    private static void Open()
    {
        var window = GetWindow<SymbolWindows>(true, SymbolString_WINDOWS);
        window.Init();
    }

    private void Init()
    {
        var defineSymbols = PlayerSettings
            .GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)
            .Split(';');
        foreach (var n in _mSymbolDataList)
        {
            n.IsEnable = defineSymbols.Any(c => c == n.name);
        }
    }

    private Vector2 verticalVec;

    public void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        verticalVec = EditorGUILayout.BeginScrollView(
            verticalVec, GUILayout.Height(position.height));
        foreach (var item in _mSymbolDataList)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            item.IsEnable = EditorGUILayout.Toggle(item.IsEnable, GUILayout.Width(16));
            EditorGUILayout.LabelField(item.name, GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));
            EditorGUILayout.LabelField(item.des, GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Save"))
        {
            OnSave();
            // Close();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    // private static List<SymbolData> symbolDataList = new List<SymbolData>();
    public static void OnSave()
    {
        var list = _mSymbolDataList.Where(a => a.IsEnable).ToList();
        var lists = list.Select(a => a.name).ToList();
        var str = _mSymbolDataList.Where(a => a.IsEnable).Select(it => it.name).ToList();
        HostType type =
            (HostType)EditorPrefs.GetInt("Tools/Skip Server Select/",
                (int)HostType.None);
        str.Add($"API_{type.ToString().ToUpper()}");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            BuildTargetGroup.Android,
            string.Join(";", str)
        );
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            BuildTargetGroup.Standalone,
            string.Join(";", str)
        );
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            BuildTargetGroup.iOS,
            string.Join(";", str)
        );
    }
    public static void OnSaveData()
    {
        var defineSymbols = PlayerSettings
            .GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');

        foreach (var itemString in _mSymbolDataList)
        {
            itemString.IsEnable = defineSymbols.Any(c => c == itemString.name);
        }

        var str = _mSymbolDataList.Where(a => a.IsEnable).Select(it => it.name).ToList();
        HostType type =
            (HostType)EditorPrefs.GetInt("Tools/Skip Server Select/",
                (int)HostType.None);
        str.Add($"API_{type.ToString().ToUpper()}");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            BuildTargetGroup.Android,
            string.Join(";", str)
        );
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            BuildTargetGroup.Standalone,
            string.Join(";", str)
        );
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            BuildTargetGroup.iOS,
            string.Join(";", str)
        );
    }
}
#endif