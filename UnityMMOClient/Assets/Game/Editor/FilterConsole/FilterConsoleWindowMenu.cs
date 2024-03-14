using AssetBundleEditorTools.AssetBundleSet;
using UnityEditor;

namespace FilterConsole
{
    public class FilterConsoleWindowMenu
    {
        [MenuItem(AssetBundleMenuItems.FilterConsoleWindowMenu, false, int.MaxValue)]
        public static void ShowWindow()
        {
            ShowWindows();
        }
        [MenuItem(AssetBundleMenuItems.FilterConsoleEditorMenu, false, int.MaxValue)]
        public static void ShowWindows()
        {
            FilterConsoleWindow.ShowWindow();
        }
    }
}