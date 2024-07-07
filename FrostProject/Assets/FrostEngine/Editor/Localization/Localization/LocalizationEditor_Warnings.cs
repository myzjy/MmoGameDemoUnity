using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrostEngine.Localization
{
	public partial class LocalizationEditor
	{
		void OnGUI_Warning_SourceInScene()
		{
		}

		private bool bSourceInsidePluginsFolder = true;
		public void OnGUI_Warning_SourceInsidePluginsFolder()
		{
			if (!bSourceInsidePluginsFolder || mLanguageSource.UserAgreesToHaveItInsideThePluginsFolder)
				return;
			
			if (!mLanguageSource.IsGlobalSource())
			{
				bSourceInsidePluginsFolder = false;
				return;
			}

			string pluginPath = UpgradeManager.GetI2LocalizationPath();
			string assetPath = AssetDatabase.GetAssetPath(target);

			if (!assetPath.StartsWith(pluginPath, StringComparison.OrdinalIgnoreCase))
			{
				bSourceInsidePluginsFolder = false;
				return;
			}
			
			string Text = @"Its advised to move this Global Source to a folder outside the I2 Localization.
For example (Assets/I2/Resources) instead of (Assets/I2/Localization/Resources)

That way upgrading the plugin its as easy as deleting the I2/Localization and I2/Common folders and reinstalling. 

Do you want the plugin to automatically move the LanguageSource to a folder outside the plugin?";
			EditorGUILayout.HelpBox(Text, MessageType.Warning);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Keep as is"))
			{
				SerializedProperty Agree = serializedObject.FindProperty("UserAgreesToHaveItInsideThePluginsFolder");
				Agree.boolValue = true;
				bSourceInsidePluginsFolder = true;
			}

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Ask me later"))
			{
				bSourceInsidePluginsFolder = false;
			}

			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Move to the Recommended Folder"))
				EditorApplication.delayCall += MoveGlobalSource;
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);		
		}

        public bool OnGUI_Warning_SourceNotUpToDate()
        {
            if (mProp_GoogleLiveSyncIsUptoDate.boolValue)
            {
                return false;
            }

            string Text = "Spreadsheet is not up-to-date and Google Live Synchronization is enabled\n\nWhen playing in the device the Spreadsheet will be downloaded and override the translations built from the editor.\n\nTo fix this, Import or Export REPLACE to Google";
            EditorGUILayout.HelpBox(Text, MessageType.Warning);
            return true;
        }

		private static void MoveGlobalSource()
		{
			EditorApplication.delayCall -= MoveGlobalSource;

			string pluginPath = UpgradeManager.GetI2LocalizationPath();
			string assetPath = AssetDatabase.GetAssetPath(mLanguageSource.ownerObject);

			string I2Path = pluginPath.Substring(0, pluginPath.Length-"/Localization".Length);
			string newPath = I2Path + "/Resources/" + mLanguageSource.ownerObject.name + ".prefab";

			string fullresFolder = Application.dataPath + I2Path.Replace("Assets","") + "/Resources";
			bool folderExists = Directory.Exists (fullresFolder);
			
			if (!folderExists)
				AssetDatabase.CreateFolder(I2Path, "Resources");
			AssetDatabase.MoveAsset(assetPath, newPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			var prefab = AssetDatabase.LoadAssetAtPath(newPath, typeof(GameObject)) as GameObject;
			Selection.activeGameObject = prefab;

			Debug.Log("LanguageSource moved to:" + newPath);
			ShowInfo("Please, ignore some console warnings/errors produced by this operation, everything worked fine. In a new release those warnings will be cleared");
		}

		public static void DelayedDestroySource()
		{

		}
	}
}