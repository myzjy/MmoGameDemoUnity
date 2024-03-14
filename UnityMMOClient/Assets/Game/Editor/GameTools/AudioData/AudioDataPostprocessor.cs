using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using ZJYFrameWork.AssetBundles.AssetBundleToolsConfig;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.EditorAssetBundle.Editors;
using ZJYFrameWork.Hotfix.Common.AudioDataObj;
using Path = System.IO.Path;

#if UNITY_EDITOR

namespace ZJYFrameWork.Editors.GameTools.AudioDataPost
{
    public class AudioDataPostprocessor : AssetPostprocessor
    {
        // [FormerlySerializedAs("_bgmDirectoryPath")] [SerializeField] private string bgmDirectoryPath = "Assets/Game/AssetBundles/AudioClip/BGM";
        static readonly string AudioDataDirectoryPath = "Assets/Game/AssetBundles/AudioClip/Sound";
        static readonly string RawDataDirecotyName = "RawData";

        /// <summary>
        /// 以下的se会并入Resources，为了不AssetBundle化，不Create AudioData
        /// </summary>
        private static readonly string[] IgnoreCreateAudioData = new string[]
            { };

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            int assetLength = importedAssets.Length;
            for (int i = 0; i < assetLength; i++)
            {
                string assetPath = importedAssets[i];
                if (assetPath.Contains(AudioDataDirectoryPath) && File.Exists(assetPath))
                {
                    AudioDataPostProcessor(assetPath);
                }
            }
        }

        private static void AudioDataPostProcessor(string assetPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            int index = Array.IndexOf(IgnoreCreateAudioData, fileName);
            if (index >= 0)
            {
                return;
            }

            var importer = AssetImporter.GetAtPath(assetPath);
            /*
             * RawData的情况下，加上文件夹名，加上AssetBundleName
             */
            if (assetPath.Contains(RawDataDirecotyName))
            {
                /* *
                 * 检查音频数据是否存在
                 */
                string audioDataPath = Path.Combine(AudioDataDirectoryPath, fileName.ToUpper() + ".asset");
                AudioData audioData = AssetDatabase.LoadAssetAtPath(audioDataPath, typeof(AudioData)) as AudioData;
                if (audioData == null)
                {
                    audioData = ScriptableObject.CreateInstance<AudioData>();
                    audioData.name = fileName.ToUpper();
                    audioData.clip = AssetDatabase.LoadAssetAtPath(assetPath, typeof(AudioClip)) as AudioClip;
                    audioData.audioAsset = fileName.ToUpper();
                    AssetDatabase.CreateAsset(audioData, audioDataPath);
                }

                importer.assetBundleName = "";
            }
            else
            {
                importer.assetBundleName = fileName.ToLower().Replace(".", "_") + AssetBundleConfig.AssetBundleSuffix;

                var asset = AssetDatabase.LoadAssetAtPath<AudioData>(assetPath);
                asset.audioAsset = fileName.ToUpper();
                EditorUtility.SetDirty(asset);
            }
        }
    }
}
#endif