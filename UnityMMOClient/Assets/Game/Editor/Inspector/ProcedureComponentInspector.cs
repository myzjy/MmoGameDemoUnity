using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork.Procedure;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Editors.Inspector
{
    [CustomEditor(typeof(ProcedureComponent))]
    public class ProcedureComponentInspector : GameFrameworkInspector
    {
        //
        private SerializedProperty availableProcedureTypeNames = null;

        //当前选择
        private SerializedProperty entranceProcedureTypeName = null;

        /// <summary>
        /// 状态机
        /// </summary>
        private string[] m_ProcedureTypeNames = null;

        private List<string> m_CurrentAvailableProcedureTypeNames = null;
        private int m_EntranceProcedureIndex = -1;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            ProcedureComponent t = (ProcedureComponent)target;
            if (string.IsNullOrEmpty(entranceProcedureTypeName.stringValue))
            {
                EditorGUILayout.HelpBox("Entrance procedure is invalid.", MessageType.Error);
                Debug.Log($"entranceProcedureTypeName.stringValue:{entranceProcedureTypeName.stringValue}");
            }
            else if (EditorApplication.isPlaying)
            {
                var currentProcedure = SpringContext.GetBean<IProcedureFsmManager>().ProcedureFsm.CurrentState;
                EditorGUILayout.LabelField("Current Procedure",
                    currentProcedure == null ? "None" : currentProcedure.GetType().ToString());
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                GUILayout.Label("Available Procedures", EditorStyles.boldLabel);
                Debug.Log($"m_ProcedureTypeNames:{m_ProcedureTypeNames.Length}");
                if (m_ProcedureTypeNames.Length > 0)
                {
                    EditorGUILayout.BeginVertical("box");
                    {
                        foreach (var procedureTypeName in m_ProcedureTypeNames)
                        {
                            var selected = m_CurrentAvailableProcedureTypeNames.Contains(procedureTypeName);
                            if (selected == EditorGUILayout.ToggleLeft(procedureTypeName, selected)) continue;
                            if (!selected)
                            {
                                m_CurrentAvailableProcedureTypeNames.Add(procedureTypeName);
                                WriteAvailableProcedureTypeNames();
                            }
                            else if (procedureTypeName != entranceProcedureTypeName.stringValue)
                            {
                                m_CurrentAvailableProcedureTypeNames.Remove(procedureTypeName);
                                WriteAvailableProcedureTypeNames();
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.HelpBox("There is no available procedure.", MessageType.Warning);
                }

                if (m_CurrentAvailableProcedureTypeNames.Count > 0)
                {
                    EditorGUILayout.Separator();

                    int selectedIndex = EditorGUILayout.Popup("Entrance Procedure", m_EntranceProcedureIndex,
                        m_CurrentAvailableProcedureTypeNames.ToArray());
                    if (selectedIndex != m_EntranceProcedureIndex)
                    {
                        m_EntranceProcedureIndex = selectedIndex;
                        entranceProcedureTypeName.stringValue = m_CurrentAvailableProcedureTypeNames[selectedIndex];
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Select available procedures first.", MessageType.Info);
                }
            }
            //结束Group绘制
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
            Repaint();
        }

        private void OnEnable()
        {
            availableProcedureTypeNames = serializedObject.FindProperty("availableProcedureTypeNames");
            entranceProcedureTypeName = serializedObject.FindProperty("entranceProcedureTypeName");

            RefreshTypeNames();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_ProcedureTypeNames = AssemblyUtils.GetAllSubClassNames(typeof(FsmState<IProcedureFsmManager>));
            Debug.Log($"[RefreshTypeNames] m_ProcedureTypeNames :{m_ProcedureTypeNames} m_ProcedureTypeNames.Length:{m_ProcedureTypeNames.Length}");
            ReadAvailableProcedureTypeNames();
            var oldCount = m_CurrentAvailableProcedureTypeNames.Count;
            m_CurrentAvailableProcedureTypeNames = m_CurrentAvailableProcedureTypeNames
                .Where(x => m_ProcedureTypeNames.Contains(x)).ToList();
            if (m_CurrentAvailableProcedureTypeNames.Count != oldCount)
            {
                WriteAvailableProcedureTypeNames();
            }
            else if (!string.IsNullOrEmpty(entranceProcedureTypeName.stringValue))
            {
                m_EntranceProcedureIndex =
                    m_CurrentAvailableProcedureTypeNames.IndexOf(entranceProcedureTypeName.stringValue);
                if (m_EntranceProcedureIndex < 0)
                {
                    entranceProcedureTypeName.stringValue = null;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ReadAvailableProcedureTypeNames()
        {
            m_CurrentAvailableProcedureTypeNames = new List<string>();
            var count = availableProcedureTypeNames.arraySize;
            for (var i = 0; i < count; i++)
            {
                m_CurrentAvailableProcedureTypeNames.Add(availableProcedureTypeNames.GetArrayElementAtIndex(i)
                    .stringValue);
            }
        }

        private void WriteAvailableProcedureTypeNames()
        {
            availableProcedureTypeNames.ClearArray();
            if (m_CurrentAvailableProcedureTypeNames == null)
            {
                return;
            }

            m_CurrentAvailableProcedureTypeNames.Sort();
            var count = m_CurrentAvailableProcedureTypeNames.Count;
            for (var i = 0; i < count; i++)
            {
                availableProcedureTypeNames.InsertArrayElementAtIndex(i);
                availableProcedureTypeNames.GetArrayElementAtIndex(i).stringValue =
                    m_CurrentAvailableProcedureTypeNames[i];
            }

            if (string.IsNullOrEmpty(entranceProcedureTypeName.stringValue)) return;
            m_EntranceProcedureIndex =
                m_CurrentAvailableProcedureTypeNames.IndexOf(entranceProcedureTypeName.stringValue);
            if (m_EntranceProcedureIndex < 0)
            {
                entranceProcedureTypeName.stringValue = null;
            }
        }
    }
}