using Sirenix.OdinInspector;
using UnityEngine;

namespace ZJYFrameWork.BattleFramework.BattleScriptObject.SkillsDataFramework
{
    [System.Serializable]
    public class SkillData:SerializedScriptableObject
    {
        [HorizontalGroup("Split", 55, LabelWidth = 70)]
        [HideLabel, PreviewField(55, ObjectFieldAlignment.Left)]
        public Texture Icon;
        
        [VerticalGroup("Split/Meta")]
        public string skillName;

        [VerticalGroup("Split/Meta")]
        public string luaScript;

    }
}