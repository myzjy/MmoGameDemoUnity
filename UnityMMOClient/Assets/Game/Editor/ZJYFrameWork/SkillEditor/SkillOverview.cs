using Sirenix.OdinInspector;
using Sirenix.Utilities;
using ZJYFrameWork.BattleFramework.BattleScriptObject.SkillsDataFramework;

namespace ZJYFrameWork.SkillEditor
{
    public class SkillOverview: GlobalConfig<SkillOverview> 
    {
        [ReadOnly]
        [ListDrawerSettings(Expanded = true)]
        public SkillData[] AllCharacters;


    }
}