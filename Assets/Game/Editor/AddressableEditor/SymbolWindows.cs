using System;
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

    private List<SymbolData> m_symbolDataList = new List<SymbolData>()
    {
        new SymbolData("Inst_resouce", "创建物体设定是否采用"),
        new SymbolData("Inst_resouce_cb", "创建物体设定是否采用这个"),
        new SymbolData("LoadAsset_re", "读资源设定是否采用这个"),
        new SymbolData("IOBSERVABLES", ""),
        new SymbolData("OUTPUT_API_JSONS", "向temporary缓存输出API响应的JSON"),
        new SymbolData("OUTPUT_VERBOSE_LOGS", "记录输出有效化"),
        new SymbolData("USE_BSET_HTTP", "用户基础Https"),
        new SymbolData("USE_DEBUG_TOOLS", "用户调试"),
        new SymbolData("HOTFIX_ENABLE","xlua"),
        new SymbolData("LOGGER_ON","LOG"),

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
        foreach (var n in m_symbolDataList)
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
        foreach (var item in m_symbolDataList)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            item.IsEnable = EditorGUILayout.Toggle(item.IsEnable, GUILayout.Width(16));
            EditorGUILayout.LabelField(item.name, GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));
            EditorGUILayout.LabelField(item.des, GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Save"))
        {
            var strs = m_symbolDataList.Where(a => a.IsEnable).Select(it => it.name).ToArray();

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