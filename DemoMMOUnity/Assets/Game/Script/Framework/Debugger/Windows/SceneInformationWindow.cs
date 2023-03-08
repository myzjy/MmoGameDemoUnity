﻿using UnityEngine;
using UnityEngine.SceneManagement;
using ZJYFrameWork.Debugger.Widows.Model;

namespace ZJYFrameWork.Debugger.Windows
{
    public sealed class SceneInformationWindow : ScrollableDebuggerWindowBase
    {
        protected override void OnDrawScrollableWindow()
        {
            GUILayout.Label("<b>Scene Information</b>");
            GUILayout.BeginVertical("box");
            {
                DrawItem("Scene Count", SceneManager.sceneCount.ToString());
                DrawItem("Scene Count In Build Settings", SceneManager.sceneCountInBuildSettings.ToString());

                UnityEngine.SceneManagement.Scene activeScene = SceneManager.GetActiveScene();
                DrawItem("Active Scene Name", activeScene.name);
                DrawItem("Active Scene Path", activeScene.path);
                DrawItem("Active Scene Build Index", activeScene.buildIndex.ToString());
                DrawItem("Active Scene Is Dirty", activeScene.isDirty.ToString());
                DrawItem("Active Scene Is Loaded", activeScene.isLoaded.ToString());
                DrawItem("Active Scene Is Valid", activeScene.IsValid().ToString());
                DrawItem("Active Scene Root Count", activeScene.rootCount.ToString());
            }
            GUILayout.EndVertical();
        }
    }
}