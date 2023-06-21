using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZJYFrameWork.UISerializable.UIViewEditor
{
    [CustomEditor(typeof(UISerializableKeyObject), true)]
    public class UISerializableKeyObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
          //  return;
            UISerializableKeyObject keyGizmos = target as UISerializableKeyObject;
            // EditorGUI.BeginChangeCheck();

            //选择中
            var selectObj = Selection.activeGameObject;
            if (keyGizmos != null)
            {
                keyGizmos.FlushData();
                //获取选中物体里面的所有组件
                var coms = selectObj.GetComponents(typeof(Component));
                string[] names = new string[coms.Length];
                for (int i = 0; i < coms.Length; i++)
                {
                    names[i] = coms[i].GetType().FullName;
                }

                //空格
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("需要使用的UI组件:");
                foreach (var item in keyGizmos.dataList)
                {
                    // Debug.LogError(item.UI_Serializable_Key);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("key:", GUILayout.Width(30));
                        EditorGUILayout.SelectableLabel(item.UI_Serializable_Key, GUILayout.Height(20));


                        EditorGUILayout.LabelField("selectObj:", GUILayout.Width(60));
                        EditorGUILayout.ObjectField(item.UI_Serializable_Obj, typeof(Component), false);
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("path:", GUILayout.Width(30));
                        EditorGUILayout.SelectableLabel(item.Path, GUILayout.Height(20));
                    }
                }

            }
            AssetDatabase.Refresh();

        }
    }
}