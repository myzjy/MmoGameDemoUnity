using System;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.Editors.Inspector
{
    [CustomEditor(typeof(NetworkManager))]
    public class NetworkManagerInspector : GameFrameworkInspector
    {
        private SerializedProperty UserAuth = null;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            NetworkManager t = (NetworkManager)target;
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            EditorGUILayout.LabelField("Token:",
                string.IsNullOrEmpty(t.UserAuth.AuthToken) ? "None" : t.UserAuth.AuthToken);
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("Server:",
                    HostString(SpringContext.GetBean<ISettingManager>().GetSelectHostType()));
                EditorGUILayout.LabelField("ServerIP:",
                    SpringContext.GetBean<ISettingManager>().GetHttpsBase());
                EditorGUILayout.LabelField("WebServerIP:",
                    SpringContext.GetBean<ISettingManager>().GetWebSocketBase());
            }
            else
            {
                EditorGUILayout.LabelField("Server:", "None");
                EditorGUILayout.LabelField("ServerIP:", "None");
                EditorGUILayout.LabelField("WebServerIP:", "None");
            }

            EditorGUILayout.PropertyField(UserAuth, true);         
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();

            
        }

        private string HostString(HostType nowHostType)
        {
            return nowHostType switch
            {
                HostType.Develop => "开发服",
                HostType.Test => "测试服",
                HostType.Online => "正式服",
                HostType.None => "",
                _ => throw new ArgumentOutOfRangeException(nameof(nowHostType), nowHostType, null)
            };
        }

        private void OnEnable()
        {
            UserAuth = serializedObject.FindProperty("UserAuth");
        }
    }
}