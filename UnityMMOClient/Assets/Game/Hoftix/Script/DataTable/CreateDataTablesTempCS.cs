using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZJYFrameWorkEditor.DataScriptableObject.DataTableScript;

namespace ZJYFrameWork.BattleFramework.BattleScriptObject.SkillsDataFramework
{
    [System.Serializable]
    public class CreateDataTablesTempCS : SerializedScriptableObject
    {
        [DictionaryDrawerSettings]
        public List<CreateDataTableListDataTemp> CreateDataTableListData=new List<CreateDataTableListDataTemp>();
    }
}