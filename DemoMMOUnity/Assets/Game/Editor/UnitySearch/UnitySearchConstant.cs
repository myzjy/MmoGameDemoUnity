using UnityEditor;
using UnityEngine;

namespace UnitySearch
{
	internal static class UnitySearchConstant
	{
		internal const float WIDTH = 700f;
		internal const float HEIGHT = 400f;
		internal const float INPUT_HEIGHT = 50F;
		internal const float ROW_HEIGHT = 40F;
		internal const float RESULT_HEIGHT = HEIGHT - INPUT_HEIGHT;
		
		internal static readonly GUIStyle SearchStyle;
		internal static readonly GUIStyle ClearButtonStyle;
		
		internal static readonly GUIStyle RowLabelStyle;
		internal static readonly GUIStyle RowSubLabelStyle;

		private static readonly GUIStyle _backgroundEven;
		private static readonly GUIStyle _backgroundOdd;
		
		internal static readonly Texture2D SearchTexture;
		internal static readonly Texture2D SettingTexture;
		internal static readonly Texture2D WindowTexture;
		internal static readonly Texture2D HierarchyTexture;
		
		static UnitySearchConstant()
		{
			SearchStyle = new GUIStyle(EditorStyles.largeLabel)
			{
				padding = {left = 40},
				fontSize = 30,
				normal = {textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black },
				alignment = TextAnchor.MiddleLeft,
			};
			ClearButtonStyle = new GUIStyle(EditorStyles.largeLabel)
			{
				fontSize = 25,
				normal = {textColor = Color.gray},
				alignment = TextAnchor.MiddleCenter,
			};
			RowLabelStyle = new GUIStyle(EditorStyles.label)
			{
				padding = {left = 45, top = 3},
				fontSize = 17,
				alignment = TextAnchor.UpperLeft,
			};
			RowSubLabelStyle = new GUIStyle(EditorStyles.label)
			{
				fontStyle = FontStyle.Italic,
				padding = {left = 45, bottom = 3},
				normal = {textColor = Color.gray},
				fontSize = 11,
				alignment = TextAnchor.LowerLeft,
			};
			
			_backgroundEven = new GUIStyle("OL EntryBackEven");
			_backgroundOdd = new GUIStyle("OL EntryBackOdd");

			SearchTexture = EditorGUIUtility.Load("d_ViewToolZoom On") as Texture2D;
			SettingTexture = EditorGUIUtility.Load("EditorSettings Icon") as Texture2D;
			WindowTexture = EditorGUIUtility.Load("d_UnityEditor.ConsoleWindow") as Texture2D;
			HierarchyTexture = EditorGUIUtility.Load("UnityEditor.HierarchyWindow") as Texture2D;
		}

		internal static void DrawBackground(Rect rect, bool isOdd, bool isOn)
		{
			(isOdd ? _backgroundOdd : _backgroundEven).Draw(rect, false, false, isOn, false);
		}
	}
}