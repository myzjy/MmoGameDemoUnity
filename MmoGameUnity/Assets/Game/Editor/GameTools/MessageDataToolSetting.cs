using UnityEngine;
using ZJYFrameWork.I18n;

namespace ZJYFrameWork.Editors.GameTools
{
    [CreateAssetMenu(menuName = "ScriptableObject/MessageData/MessageDataToolSetting")]
    [System.Serializable]
    public class MessageDataToolSetting: ScriptableObject
    {
        [Tooltip("电子表格的URL")]
        public string spreadSheetURL = "REMOVED_URL";

        [Tooltip("以csv形式下载的URL")]
        public string spreadSheetCSVDownloadURL = "REMOVED_URL";

        [Tooltip("EnumMessage.cs的输出路径")]
        public string enumFilePath = "/Game/Script/Common/";

        [Tooltip("ScriptableObject的输出路径")]
        public string outputDataPath = "Assets/Game/AssetBundles/ScriptableObject/MessageData/";

        [Tooltip("editor上设置ReplaceText的语言data")]
        public MessageData editorLanguageData;

        #region 活动用

        [Tooltip("活动用电子表格的URL")]
        public string eventSpreadSheetURL = "REMOVED_URL";

        [Tooltip("活动用MessageClass的输出路径")]
        public string eventMessageClassFilePath = "/Game/Script/Event/Message/";

        [Tooltip("活动用ScriptableObject的输出路径")]
        public string eventOutputDataPath = "Assets/Game/AssetBundles/ScriptableObject/MessageData/Event/";

        #endregion


        #region 宣传用

        [Tooltip("活动用电子表格的URL")]
        public string campaignSpreadSheetURL = "REMOVED_URL";

        [Tooltip("活动用MessageClass的输出路径")]
        public string campaignMessageClassFilePath = "/Game/Script/Campaign/Message/";

        [Tooltip("活动用ScriptableObject的输出路径")]
        public string campaignOutputDataPath = "Assets/Game/AssetBundles/ScriptableObject/MessageData/Campaign/";

        #endregion
    }
}