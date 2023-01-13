using UnityEditor;
using UnityEngine;
using ZJYFrameWork.Common;

namespace ZJYFrameWork.Editors.GameTools
{
    public class MessageDataToolWindows : EditorWindow
    {
        private const string WINDOW_NAME = "MessageDataTool";

        private const float ButtonHeight = 24f;

        private const float ButtonMaxWidth = 500f;
        private static MessageDataToolWindows window;

#pragma warning disable CS0414
        private bool _isSubMessageTab = false;
#pragma warning restore CS0414
        private Message checkKey = Message.MAX_NUM;
        private string checkKeyStr = string.Empty;

        private bool isUseStringSreachMode = false;

        private string[] notUseKeys = null;
        private Vector2 scrollPosAll = Vector2.zero;

        private Vector2 scrollPosNotUse = Vector2.zero;

        private void OnGUI()
        {
            scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);
            {
                EditorGUILayout.Separator();
                DrawCommonMessage();
            }
            EditorGUILayout.EndScrollView();
        }

        [MenuItem("Tools/Localize/MessageDataTool")]
        public static void ShowWindow()
        {
            MessageDataToolWindows.window = EditorWindow.GetWindow<MessageDataToolWindows>(false, WINDOW_NAME);
            MessageDataToolWindows.window.Show();
        }

        private void DrawCommonMessage()
        {
            {
                EditorGUILayout.Separator();

                GUILayout.Label("电子表格");

                // if (GUILayout.Button("在浏览器/文件夹中打开", GUILayout.Height(ButtonHeight), GUILayout.MaxWidth(ButtonMaxWidth)))
                // {
                //     MessageDataTool.OpenSpreadSheet();
                // }

                EditorGUILayout.Separator();

                GUILayout.Label("CSV数据");

                using (new EditorGUILayout.HorizontalScope())
                {
                    // if (GUILayout.Button("下载地址", GUILayout.Height(ButtonHeight),
                    //         GUILayout.MaxWidth(ButtonMaxWidth * 0.5f)))
                    // {
                    //     MessageDataTool.Download();
                    // }

                    if (GUILayout.Button("导入", GUILayout.Height(ButtonHeight),
                            GUILayout.MaxWidth(ButtonMaxWidth * 0.5f)))
                    {
                        MessageDataTool.Import();
                    }
                }

                EditorGUILayout.Separator();

                GUILayout.Label("搜索使用文件");


                isUseStringSreachMode = EditorGUILayout.ToggleLeft("通过String输入搜索：", isUseStringSreachMode);

                if (isUseStringSreachMode)
                {
                    checkKeyStr = EditorGUILayout.TextField(checkKeyStr, GUILayout.Height(ButtonHeight),
                        GUILayout.MaxWidth(ButtonMaxWidth));
                    try
                    {
                        checkKey = (Message)System.Enum.Parse(typeof(Message), checkKeyStr);
                    }
                    catch
                    {
                        checkKey = Message.MAX_NUM;
                    }
                }
                else
                {
                    checkKey = (Message)EditorGUILayout.EnumPopup(checkKey, GUILayout.Height(ButtonHeight),
                        GUILayout.MaxWidth(ButtonMaxWidth));
                }


                EditorGUILayout.Separator();

                if (notUseKeys != null)
                {
                    scrollPosNotUse = EditorGUILayout.BeginScrollView(scrollPosNotUse);

                    int length = notUseKeys.Length;
                    for (int i = 0; i < length; ++i)
                    {
                        EditorGUILayout.LabelField(notUseKeys[i]);
                    }

                    EditorGUILayout.EndScrollView();
                }
            }
        }
    }
}