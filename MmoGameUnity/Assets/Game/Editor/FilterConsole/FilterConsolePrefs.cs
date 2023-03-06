using System;
using UnityEditor;

namespace FilterConsole
{
   	/// <summary>
	/// Prefs関連の隔離先
	/// </summary>
	public partial class FilterConsoleWindow
	{

		// EditorPrefs用のkey群
		public enum BoolPrefsKeyName
		{
			DebugHeader = 0,
			DisplayLog,
			DisplayWarning,
			DisplayError,
			DisplayDate,
			AutoScroll,
			IncludingStacktrace,
			FoldDetail,
		}

		public enum FloatPrefsKeyName
		{
			ResizerRectY = 0,
		}

		/// <summary>
		/// キーとdefault値の定義
		/// </summary>
		public class PrefsKey
		{
			private const string PrefsPrefix = "ColoplEditor.FilterConsole.";

			#region bool

			public static Key<bool>[] BoolKeys = {
				new Key<bool>(BoolPrefsKeyName.DebugHeader.ToString(), "デバッグボタン", false),

				new Key<bool>(BoolPrefsKeyName.DisplayLog.ToString(), "Log表示", true),
				new Key<bool>(BoolPrefsKeyName.DisplayWarning.ToString(), "Warning表示", true),
				new Key<bool>(BoolPrefsKeyName.DisplayError.ToString(), "Error表示", true),

				new Key<bool>(BoolPrefsKeyName.DisplayDate.ToString(), "時刻表示", false),
				new Key<bool>(BoolPrefsKeyName.AutoScroll.ToString(), "自動スクロール", true),
				new Key<bool>(BoolPrefsKeyName.IncludingStacktrace.ToString(), "StackTraceもフィルタのチェックに含む", false),

				new Key<bool>(BoolPrefsKeyName.FoldDetail.ToString(), "ウィンドウ下部の短縮表示", true),
			};

			public static string DebugHeader { get { return BoolKeys[(int) BoolPrefsKeyName.DebugHeader].KeyName; } }

			public static string DisplayLog { get { return BoolKeys[(int) BoolPrefsKeyName.DisplayLog].KeyName; } }
			public static string DisplayWarning { get { return BoolKeys[(int) BoolPrefsKeyName.DisplayWarning].KeyName; } }
			public static string DisplayError { get { return BoolKeys[(int) BoolPrefsKeyName.DisplayError].KeyName; } }

			public static string DisplayDate { get { return BoolKeys[(int) BoolPrefsKeyName.DisplayDate].KeyName; } }
			public static string AutoScroll { get { return BoolKeys[(int) BoolPrefsKeyName.AutoScroll].KeyName; } }

			public static string IncludingStacktrace { get { return BoolKeys[(int) BoolPrefsKeyName.IncludingStacktrace].KeyName; } }

			public static string FoldDetail { get { return BoolKeys[(int) BoolPrefsKeyName.FoldDetail].KeyName; } }

			#endregion

			#region float

			public static Key<float>[] FloatKeys = {
				new Key<float>(FloatPrefsKeyName.ResizerRectY.ToString(), "分割バーのY座標", 400f),
			};

			public static string ResizerRectY { get { return FloatKeys[(int) FloatPrefsKeyName.ResizerRectY].KeyName; } }

			#endregion

			public class Key<T> where T : IComparable
			{
				public string KeyName;
				public string MenuName;
				public T DefaultValue;

				public Key(string keyName, string menu, T defaultValue)
				{
					KeyName = PrefsPrefix + keyName;
					MenuName = menu;
					DefaultValue = defaultValue;
				}
			}
		}

		/// <summary>
		/// 値へのアクセス役
		/// </summary>
		public static class Prefs
		{
			static Prefs()
			{
				InitValue();
			}

			public static void InitValue()
			{
				foreach (var key in PrefsKey.BoolKeys)
				{
					if (!HasKey(key.KeyName))
					{
						SetBool(key.KeyName, key.DefaultValue);
					}
				}
				foreach (var key in PrefsKey.FloatKeys)
				{
					if (!HasKey(key.KeyName))
					{
						SetFloat(key.KeyName, key.DefaultValue);
					}
				}
			}

			public static void Delete()
			{
				foreach (var key in PrefsKey.BoolKeys)
				{
					EditorPrefs.DeleteKey(key.KeyName);
				}
				foreach (var key in PrefsKey.FloatKeys)
				{
					EditorPrefs.DeleteKey(key.KeyName);
				}

				InitValue();
			}

			public static bool HasKey(string key)
			{
				return EditorPrefs.HasKey(key);
			}

			#region Bool

			public static bool GetBool(string key)
			{
				return EditorPrefs.GetBool(key);
			}

			public static bool GetBool(string key, bool defaultValue)
			{
				return EditorPrefs.GetBool(key, defaultValue);
			}

			public static void SetBool(string key, bool value)
			{
				EditorPrefs.SetBool(key, value);
			}

			#endregion

			#region Float

			public static float GetFloat(string key)
			{
				return EditorPrefs.GetFloat(key);
			}

			public static float GetFloat(string key, float defaultValue)
			{
				return EditorPrefs.GetFloat(key, defaultValue);
			}

			public static void SetFloat(string key, float value)
			{
				EditorPrefs.SetFloat(key, value);
			}

			#endregion

		}

	}
}