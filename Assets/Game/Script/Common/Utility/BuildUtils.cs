﻿namespace Common.Utility
{
    public class BuildUtils
    {
        /// <summary>
        /// 清单包名称
        /// </summary>
        public const string ManifestBundleName = "AssetBundles";
        public const string ChannelNameFileName = "channel_name.bytes";
        public const string AppVersionFileName = "app_version.bytes";
        public const string ResVersionFileName = "res_version.bytes";
        public const string NoticeVersionFileName = "notice_version.bytes";
        public const string AssetBundlesSizeFileName = "assetbundls_size.bytes";
        public const string UpdateNoticeFileName = "update_notice.bytes";

        public static bool CheckIsNewVersion(string sourceVersion, string targetVersion)
        {
            string[] sVerList = sourceVersion.Split('.');
            string[] tVerList = targetVersion.Split('.');

            if (sVerList.Length < 3 || tVerList.Length < 3) return false;
            try
            {
                int sV0 = int.Parse(sVerList[0]);
                int sV1 = int.Parse(sVerList[1]);
                int sV2 = int.Parse(sVerList[2]);
                int tV0 = int.Parse(tVerList[0]);
                int tV1 = int.Parse(tVerList[1]);
                int tV2 = int.Parse(tVerList[2]);

                if (tV0 > sV0)
                {
                    return true;
                }
                else if (tV0 < sV0)
                {
                    return false;
                }

                if (tV1 > sV1)
                {
                    return true;
                }
                else if (tV1 < sV1)
                {
                    return false;
                }

                return tV2 > sV2;
            }
            catch (System.Exception ex)
            {
                ToolsDebug.LogError(
                    $"parse version error. clientversion: {sourceVersion} serverversion: {targetVersion}\n {ex.Message}\n{ex.StackTrace}");
                return false;
            }

            return false;
        }
    }
}