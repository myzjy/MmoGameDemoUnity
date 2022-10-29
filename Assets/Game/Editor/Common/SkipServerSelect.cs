using System;
using System.Collections.Generic;
using UnityEditor;
using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.Editors.Common
{
#if UNITY_EDITOR

    public class SkipServerSelect
    {
        public const string MENU_BASE_PATH = "Tools/Skip Server Select/";

        private static List<HostType> selectableTypes;

        static SkipServerSelect()
        {
            HostType[] types =
                (HostType[])Enum.GetValues(typeof(HostType));
            selectableTypes = new List<HostType>(types);

            HostType type =
                (HostType)EditorPrefs.GetInt(MENU_BASE_PATH, (int)HostType.None);
            selectableTypes.ForEach(t => Menu.SetChecked(MENU_BASE_PATH + t.ToString(), t == type));
        }
        private static void SetPrefs(HostType type)
        {
            EditorPrefs.SetInt(MENU_BASE_PATH, (int) type);
            SymbolWindows.OnSaveData();
            selectableTypes.ForEach(t => Menu.SetChecked(MENU_BASE_PATH + t.ToString(), t == type));
        }

        [MenuItem(MENU_BASE_PATH + "None")]
        private static void MenuAction_None()
        {
            SetPrefs(HostType.None);
        }

        [MenuItem(MENU_BASE_PATH + "Test")]
        private static void MenuAction_Test()
        {
            SetPrefs(HostType.Test);
        }

        [MenuItem(MENU_BASE_PATH + "Online")]
        private static void MenuAction_Online()
        {
            SetPrefs(HostType.Online);
        }

        private static bool SetChecked(HostType type)
        {
            Menu.SetChecked(MENU_BASE_PATH + type.ToString(),
                type == (HostType) EditorPrefs.GetInt(MENU_BASE_PATH,
                    (int) HostType.None));
            return true;
        }
    }
#endif
}