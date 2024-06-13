using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork.BattleFramework.BattleScriptObject.SkillsDataFramework;
using ZJYFrameWorkEditor.DataScriptableObject.DataTableScript;

namespace ZJYFrameWork.SkillEditor
{
    public class SkillEditorWindows : OdinMenuEditorWindow
    {
        [MenuItem("Tools/FrameworkTools/SkillWindows")]
        public static void OpenWindows()
        {
            GetWindow<SkillEditorWindows>().Show();
        }

        private SkillData[] SkillDatas;

        public void UpdateCharacterOverview()
        {
            SkillDatas= AssetDatabase.FindAssets("t:SkillData")
                .Select(guid => AssetDatabase.LoadAssetAtPath<SkillData>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();   
        }

        protected override void DrawEditors()
        {
            base.DrawEditors();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Selection.SupportsMultiSelect = false;
            // tree.Add("Settings", GeneralDrawerConfig.Instance);
            // tree.Add("skill", tree.DefaultMenuStyle);
            UpdateCharacterOverview();
            // tree.Add("Characters", new SkillOverview(SkillDatas));

            tree.AddAllAssetsAtPath("Odin St", "Assets/Game/AssetBundles/ScriptableObject/SkillMsgData",
                typeof(SkillData), true, true);
            return tree;
        }
        private void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }
        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Skill")))
                {
                    ScriptableObjectCreator.ShowDialog<SkillData>("Assets/Game/AssetBundles/ScriptableObject/SkillMsgData", obj =>
                    {
                        obj.skillName = obj.name;
                        base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    });
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create CSTemp")))
                {
                    ScriptableObjectCreator.ShowDialog<CreateDataTablesTempCS>("Assets/Game/AssetBundles/ScriptableObject/Temps", obj =>
                    {
                        base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    });
                }
                
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}