using System;
using UnityEngine;

namespace ZJYFrameWork.Net
{
    public class UserAgent
    {
        private static UserAgent instance;

        public static string Value
        {
            get
            {
                instance = instance ?? new UserAgent();
                return instance.ToString();
            }
        }

        private string cached;

        private UserAgent()
        {
            string appName = Application.identifier;
            string version = Application.version;
            string operatingSystem = SystemInfo.operatingSystem;
            string device = SystemInfo.deviceModel;

            cached = $"{appName}/{version} ({operatingSystem}; {device})";

#if UNITY_EDITOR
            var platform = "Unknown";
            var buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
            platform = buildTarget switch
            {
                UnityEditor.BuildTarget.iOS => "iPhone",
                UnityEditor.BuildTarget.Android => "Android",
                _ => platform
            };
            cached += $"<Emulating:{platform}>";
#endif
        }

        public override string ToString()
        {
            return cached;
        }
    }
}