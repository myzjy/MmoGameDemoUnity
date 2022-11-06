using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork.I18n;

namespace ZJYFrameWork.Editors.GameTools
{
    public class MessageDataTool
    {
        private const string MESSAGE_DATA_PREFIX = "MessageData_";

        // 替换为换行代码的标签
        private const string NEWLINE_REPLACE_TAG_BR = "<br>";

        const string NEWLINE_REPLACE_TAG_N = "\\n";

        // 这个工具的路径
        public const string TOOL_PATH = "Assets/Game/AssetBundles/ScriptableObject/MessageData/";

        //设定文件的路径
        public const string SETTING_FILE_NAME = "MessageDataToolSetting.asset";
        private static MessageDataToolSetting setting = null;

        /// <summary>
        /// 设置文件的加载
        /// </summary>
        static void LoadSetting()
        {
            if (setting != null)
            {
                return;
            }
            Debug.Log(TOOL_PATH + SETTING_FILE_NAME);

            setting = AssetDatabase.LoadAssetAtPath<MessageDataToolSetting>(TOOL_PATH + SETTING_FILE_NAME);
        }

        /// <summary>
        /// csvデータのインポート
        /// </summary>
        public static void Import()
        {
            LoadSetting();
            string path = EditorUtility.OpenFilePanel("请选择CSV文件", "./", "csv");
            if (path.Length == 0)
            {
                return;
            }


            using (var sr = new System.IO.StreamReader(path))
            {
                Debug.Log("[MessageDataTool]---MessageData Import---");

                string enumText =
                    "namespace ZJYFrameWork {\n" +
                    "public enum Message {\n";

                int languageCount = 0;
                List<string>[] messageLists = null;

                // 1行目から言語数を取得
                if (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var values = line.Split(',');
                    languageCount = values.Length - 2;

                    messageLists = new List<string>[languageCount];
                    for (int i = 0; i < languageCount; ++i)
                    {
                        messageLists[i] = new List<string>();
                    }
                }

                // データがなくなるまでループ
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var values = line.Split(',');

                    string key = values[1];

                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }

                    if (values.Length > 2)
                    {
                        string comment = values[2];
                        if (!string.IsNullOrEmpty(comment))
                        {
                            enumText += string.Format("/// <summary>\n/// {0}\n/// </summary>\n", comment);
                            //enumText += string.Format("/// {0}\n", comment);
                        }
                    }

                    enumText += key + ",\n";

                    for (int i = 0; i < languageCount; ++i)
                    {
                        string value = values[2 + i];
                        value = ReplaceTag(value);
                        messageLists[i].Add(value);
                    }
                }

                enumText += "MAX_NUM\n}\n}";

                // EnumMessage.cs作成
                StreamWriter sw;
                FileInfo fi;
                Debug.Log(setting);
                string messagePath = $"{Application.dataPath}{setting.enumFilePath}EnumMessage.cs";
                Debug.Log(messagePath);
                fi = new FileInfo(messagePath);
                sw = fi.CreateText();
                sw.WriteLine(enumText);
                sw.Flush();
                sw.Close();

                // ScriptableObject作成
                for (int i = 0; i < languageCount; ++i)
                {
                    string scriptablePath = setting.outputDataPath + MESSAGE_DATA_PREFIX + messageLists[i][0] +
                                            ".asset";

                    var data = AssetDatabase.LoadAssetAtPath<MessageData>(scriptablePath);
                    if (data != null)
                    {
                        // 有的话更新
                        data.hideFlags = HideFlags.NotEditable;
                        data.messages = messageLists[i].ToArray();

                        EditorUtility.SetDirty(data);
                        AssetDatabase.SaveAssets();
                    }
                    else
                    {
                        // 没有的话就做
                        data = ScriptableObject.CreateInstance<MessageData>();

                        data.hideFlags = HideFlags.NotEditable;
                        data.messages = messageLists[i].ToArray();

                        AssetDatabase.CreateAsset((ScriptableObject)data, scriptablePath);

                        Debug.Log(
                            "[MessageDataTool] NewFile " + MESSAGE_DATA_PREFIX + messageLists[i][0] + ".asset");
                    }
                }

                Debug.Log("[MessageDataTool]---MessageData Updated!---");
                AssetDatabase.Refresh();
            }
        }

        public static string ReplaceTag(string value)
        {
            value = value.Replace(NEWLINE_REPLACE_TAG_BR, "\n");
            value = value.Replace(NEWLINE_REPLACE_TAG_N, "\n");
            value = value.Replace("⚠️", "⚠");
            return value;
        }

        #region MyRegion

        // /// <summary>
        // /// イベント用csvデータのインポート
        // /// </summary>
        // public static void SubMessageImport(MessageManager.SubMessageType subMessageType)
        // {
        // 	LoadSetting();
        // 	string path = EditorUtility.OpenFilePanel("CSVファイルを選択して下さい", "C:\\", "csv");
        // 	if (path.Length == 0)
        // 	{
        // 		return;
        // 	}
        // 	int id = 0;
        //
        // 	string prefix = string.Format("{0}_Message_DTL - ", subMessageType.ToString());
        //
        // 	string tmp = System.IO.Path.GetFileNameWithoutExtension(path);
        //
        // 	if (!int.TryParse(tmp.Substring(tmp.IndexOf(prefix) + prefix.Length), out id))
        // 	{
        // 		Debug.LogErrorFormat("[MessageDataTool] {0}用のMessageDataではありません", subMessageType.ToString());
        // 		return;
        // 	}
        //
        // 	try
        // 	{
        // 		using (var sr = new System.IO.StreamReader(path))
        // 		{
        // 			Debug.LogFormat("[MessageDataTool]---{0}_{1} MessageData Import---", subMessageType.ToString(), id.ToString());
        //
        // 			string classNameStr = string.Empty;
        // 			string enumText = string.Empty;
        //
        // 			if (id % 10000 == 0)
        // 			{
        // 				// switch (subMessageType)
        // 				// {
        // 				// 	case MessageManager.SubMessageType.Event:
        // 				// 		classNameStr = string.Format("{0}{1}Common", ((Dpuzzle.Net.GameEventType)(id / 10000)).ToString(), subMessageType.ToString());
        // 				// 		enumText = "namespace Dpuzzle {\n" +
        // 				// 		"public class Message_" + classNameStr + " {\n";
        // 				// 		break;
        // 				// 	case MessageManager.SubMessageType.Campaign:
        // 						classNameStr = string.Format("{0}{1}Common", ((Dpuzzle.Net.CampaignType)(id / 10000)).ToString(), subMessageType.ToString());
        // 						enumText = "namespace Dpuzzle {\n" +
        // 						"public class Message_Campaign_" + classNameStr + " {\n";
        // 					// 	break;
        // 					// default:
        // 					// 	break;
        // 				// }
        // 			}
        // 			else
        // 			{
        // 				// switch (subMessageType)
        // 				// {
        // 				// 	case MessageManager.SubMessageType.Event:
        // 				// 		classNameStr = id.ToString();
        // 				// 		enumText = "namespace Dpuzzle {\n" +
        // 				// 		"public class Message_" + classNameStr + " {\n";
        // 				// 		break;
        // 					// case MessageManager.SubMessageType.Campaign:
        // 						classNameStr = string.Format("Campaign_{0}", id);
        // 						enumText = "namespace Dpuzzle {\n" +
        // 						"public class Message_Campaign_" + classNameStr + " {\n";
        // 				// 		break;
        // 				// 	default:
        // 				// 		break;
        // 				// }
        // 			}
        // 			Debug.Log($"FileName:{string.Format("Message_{0}.cs", classNameStr)}");
        //
        // 			List<string> keyList = new List<string>();
        // 			List<string> messageList = new List<string>();
        //
        // 			// 1行目は無視
        // 			if (!sr.EndOfStream)
        // 			{
        // 				sr.ReadLine();
        // 			}
        //
        // 			// データがなくなるまでループ
        // 			while (!sr.EndOfStream)
        // 			{
        // 				var line = sr.ReadLine();
        // 				var values = line.Split(',');
        // 				if (values.Length != 4)
        // 				{
        // 					Debug.LogError($"MessageError:{line}");
        // 					break;
        // 				}
        // 				string key = values[1];
        // 				keyList.Add(key);
        //
        // 				if (string.IsNullOrEmpty(key))
        // 				{
        // 					continue;
        // 				}
        //
        // 				string value = values[2];
        // 				string comment = value;
        // 				if (!string.IsNullOrEmpty(comment))
        // 				{
        // 					enumText += string.Format("/// {0}\n", comment);
        // 				}
        //
        // 				enumText += string.Format("public const string {1} = \"{0}_{1}\";\n", id, key);
        //
        // 				value = ReplaceTag(value);
        // 				messageList.Add(value);
        // 			}
        // 			enumText += "}\n}";
        //
        // 			// Message_{eventId}.cs作成
        // 			StreamWriter sw;
        // 			FileInfo fi;
        // 			fi = new FileInfo(Application.dataPath + setting.eventMessageClassFilePath + string.Format("Message_{0}.cs", classNameStr));
        // 			sw = fi.CreateText();
        // 			sw.WriteLine(enumText);
        // 			sw.Flush();
        // 			sw.Close();
        //
        // 			// ScriptableObject作成
        // 			string fileName = string.Empty;
        // 			string scriptablePath = string.Empty;
        //
        // 			// switch (subMessageType)
        // 			// {
        // 				// case MessageManager.SubMessageType.Event:
        // 				// 	fileName = EVENT_MESSAGE_DATA_PREFIX + id.ToString() + ".asset";
        // 				// 	scriptablePath = setting.eventOutputDataPath + fileName;
        // 				// 	break;
        // 				// case MessageManager.SubMessageType.Campaign:
        // 					fileName = CAMPAIGN_MESSAGE_DATA_PREFIX + id.ToString() + ".asset";
        // 					scriptablePath = setting.campaignOutputDataPath + fileName;
        // 				// 	break;
        // 				// default:
        // 				// 	break;
        // 			// }
        //
        //
        // 			var data = AssetDatabase.LoadAssetAtPath<SubMessageData>(scriptablePath);
        // 			if (data != null)
        // 			{
        // 				// あったら更新
        // 				data.hideFlags = HideFlags.NotEditable;
        // 				data.keys = keyList.ToArray();
        // 				data.messages = messageList.ToArray();
        //
        // 				EditorUtility.SetDirty(data);
        // 				AssetDatabase.SaveAssets();
        // 			}
        // 			else
        // 			{
        // 				// なければ作る
        // 				data = ScriptableObject.CreateInstance<SubMessageData>();
        //
        // 				data.hideFlags = HideFlags.NotEditable;
        // 				data.keys = keyList.ToArray();
        // 				data.messages = messageList.ToArray();
        //
        // 				AssetDatabase.CreateAsset((ScriptableObject)data, scriptablePath);
        //
        // 				// AssetBundleNameを設定
        // 				var importer = AssetImporter.GetAtPath(scriptablePath);
        // 				importer.assetBundleName = fileName.Replace(".asset", AssetBundleLoader.ASSETBUNDLE_EXT);
        // 				importer.SaveAndReimport();
        //
        // 				Debug.Log("[MessageDataTool] NewFile " + fileName);
        // 			}
        //
        // 			Debug.LogFormat("[MessageDataTool]---{0}MessageData Updated!---", subMessageType.ToString());
        // 			AssetDatabase.Refresh();
        // 		}
        // 	}
        // 	catch (System.Exception e)
        // 	{
        // 		Debug.LogError(e.StackTrace);
        // 	}
        // }

        #endregion
    }
}