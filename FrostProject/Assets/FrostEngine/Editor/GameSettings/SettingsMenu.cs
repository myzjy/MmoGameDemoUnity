using UnityEditor;

namespace FrostEngine.Editor.GameSettings
{
    public static class SettingsMenu
    {
        [MenuItem("FrostEngine/Settings/FrostEngineSettings", priority = -1)]
        public static void OpenSettings() => SettingsService.OpenProjectSettings("FrostEngine/FrostEngineSettings");
    }
}