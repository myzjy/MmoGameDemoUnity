#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ZJYFrameWork.UISerializable.UIViewEditor
{
    [CustomEditor(typeof(ViewSignSerializableUI), true)]
    public class UIViewSignSerializableUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var gizmos = target as ViewSignSerializableUI;
            //修改
            // EditorGUI.BeginChangeCheck();
            //返回活动的游戏对象。(检查图中所示)
            var obj = Selection.activeGameObject;
            var cms = obj.GetComponents(typeof(Component));

            cms = cms.Where(a => gizmos is { } && a.GetType() != gizmos.GetType()).ToArray();
            // List<string> names = new List<string>();
            string[] names = new string[cms.Length + 1];
            for (int i = 0; i < cms.Length; i++)
            {
                names[i] = cms[i].GetType().Name;
            }

            names[cms.Length] = "GameObject";
            gizmos.MaskIndex = EditorGUILayout.MaskField("选择组件", gizmos.MaskIndex, names);
            for (var i = 0; i < cms.Length; i++)
            {
                if (0 == (1 << i & gizmos.MaskIndex))
                {
                    if (gizmos.Contains(cms[i]))
                    {
                        gizmos.Delete(cms[i]);
                    }
                }
                else
                {
                    if (!gizmos.Contains(cms[i]))
                    {
                        var KeyData = new UIKeyObjectData()
                        {
                            UI_Serializable_Key = ViewSignSerializableUI.GetPath(cms[i]),
                            UI_Serializable_Obj = cms[i],
                            Path = ViewSignSerializableUI.GetObjPath(cms[i])
                        };
                        gizmos.KodComs.Add(KeyData);
                    }
                }
            }

            foreach (var item in gizmos.KodComs)
            {
                using (var hs = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("key:", GUILayout.Width(30));
                    string newKey = EditorGUILayout.TextField(item.UI_Serializable_Key);
                    if (newKey != item.UI_Serializable_Key)
                    {
                        item.UI_Serializable_Key = newKey;
                    }

                    EditorGUILayout.LabelField("selectObj:", GUILayout.Width(60));
                    EditorGUILayout.ObjectField(item.UI_Serializable_Obj, typeof(Component), false);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("path:", GUILayout.Width(30));
                    EditorGUILayout.SelectableLabel(item.Path, GUILayout.Height(20));
                }
            }

            if (0 != (1 << (names.Length - 1) & gizmos.MaskIndex))
            {
                if (!gizmos.Contains(obj))
                {
                    var KeyData = new UIKeyObjectData()
                    {
                        UI_Serializable_Key = ViewSignSerializableUI.GetPath(obj),
                        UI_Serializable_Obj = obj,
                        Path = ViewSignSerializableUI.GetObjPath(obj)
                    };
                    gizmos.KodComs.Add(KeyData);
                }
            }
            else
            {
                if (gizmos.Contains(obj))
                {
                    gizmos.Delete(obj);
                }
            }

            // if (isChange)
            // {
            //     gizmos.ViewRootFlush();
            // }

        }
    }
}
#endif