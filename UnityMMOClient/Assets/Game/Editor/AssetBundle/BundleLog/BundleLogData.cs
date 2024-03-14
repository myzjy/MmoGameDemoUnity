#if UNITY_EDITOR && DEVELOP_BUILD

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tools.BundleLog
{
	[Serializable]
	public class BundleData
	{
		public string Name;
		public string[] Dependencies = new string[0];
		public string Trace;
	}

	public static class BundleLogData
	{
		public static IEnumerable<BundleData> Logs => Data.instance.Logs;

		public delegate void UpdateLog(IEnumerable<BundleData> datas);

		public static event UpdateLog UpdateLogEvent;

		// 启动中的方向盘不行，每次都要检查
		// GCなくて0.02msくらいなので毎回呼び出ししても問題なさそう
		// public static bool IsAssetBundleMode => EditorPrefs.GetBool("ZJYFrameWork/Tools/AssetBundle Mode", false);

		public static bool IsValid => Resources.FindObjectsOfTypeAll<BundleLogWindow>().Length > 0;

		public static void Add(string path)
		{
			var assetName = AssetImporter.GetAtPath(path).assetBundleName;
			var dependencies = AssetDatabase.GetDependencies(path)
				.Select(AssetImporter.GetAtPath)
				.Select(importer => importer.assetBundleName)
				.Where(n => !string.IsNullOrEmpty(n) && n != assetName)
				.Distinct()
				.ToArray();

			Add(assetName, dependencies);
		}

		public static void Add(string name, string[] dependencies)
		{
			Data.instance.Add(new BundleData {
				Name = name,
				Dependencies = dependencies,
				Trace = StackTraceUtility.ExtractStackTrace(),
			});
			UpdateLogEvent?.Invoke(Data.instance.Logs);
		}

		public static void Clear()
		{
			Data.instance.Clear();
			UpdateLogEvent?.Invoke(Data.instance.Logs);
		}
	}

	public class Data : ScriptableSingleton<Data>
	{
		public IEnumerable<BundleData> Logs => instance._logs;

		[SerializeField]
		private List<BundleData> _logs = new List<BundleData>(LogMax + 1);

		public const int LogMax = 10000;

		internal void Add(BundleData data)
		{
			_logs.RemoveAll(l => l.Equals(data));
			_logs.Insert(0, data);
			while (_logs.Count > LogMax)
				_logs.RemoveAt(LogMax);
		}

		internal void Clear()
		{
			_logs = new List<BundleData>(LogMax + 1);
		}
	}
}

#endif