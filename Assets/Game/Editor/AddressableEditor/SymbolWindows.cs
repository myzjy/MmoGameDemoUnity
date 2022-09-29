﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class SymbolWindows : EditorWindow
{
    private const string ITEMTOOLS = "Tool/Symbol";
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

    private readonly List<SymbolData> _mSymbolDataList = new List<SymbolData>()
    {
        new SymbolData("OUTPUT_API_JSONS", "向temporary缓存输出API响应的JSON"),
        new SymbolData("OUTPUT_VERBOSE_LOGS", "记录输出有效化"),
        new SymbolData("USE_DEBUG_TOOLS", "用户调试"),
        new SymbolData("LOGGER_ON","LOG"),
        new SymbolData("DEVELOP_BUILD","开发模式"),
        new SymbolData("DISABLE_SERVERSENT_EVENTS",""),
        new SymbolData("BESTHTTP_DISABLE_SIGNALR",""),
        new SymbolData("BESTHTTP_DISABLE_SOCKETIO",""),
        new SymbolData("BESTHTTP_DISABLE_UNITY_FORM",""),

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
            var strs = _mSymbolDataList.Where(a => a.IsEnable).Select(it => it.name).ToArray();

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildTargetGroup.Android,
                string.Join(";", strs)
            );
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildTargetGroup.iOS,
                string.Join(";", strs)
            );
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildTargetGroup.Standalone,
                string.Join(";", strs)
            );
            Close();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
#endif