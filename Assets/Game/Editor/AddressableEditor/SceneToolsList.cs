#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneList
{
    [MenuItem("Tool/Start", false, 1)]
    private static void OpenStartScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/LaunchScene.unity");
    }
}
#endif