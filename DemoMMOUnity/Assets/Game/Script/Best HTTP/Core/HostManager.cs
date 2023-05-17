using System.Collections.Generic;
using System.Linq;
using BestHTTP.PlatformSupport.FileSystem;

namespace BestHTTP.Core
{
    public static class HostManager
    {
        private const int Version = 1;
        private static string LibraryPath = string.Empty;
        private static bool IsSaveAndLoadSupported = false;
        private static bool IsLoaded = false;

        private static Dictionary<string, HostDefinition> hosts = new Dictionary<string, HostDefinition>();

        public static HostDefinition GetHost(string hostStr)
        {
            HostDefinition host;
            if (!hosts.TryGetValue(hostStr, out host))
                hosts.Add(hostStr, host = new HostDefinition(hostStr));

            return host;
        }

        public static void RemoveAllIdleConnections()
        {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            Debug.Log($"[HostManager] [method:RemoveAllIdleConnections] [msg] RemoveAllIdleConnections");
#endif
            foreach (var variantKvp in hosts.SelectMany(hostKvp => hostKvp.Value.HostConnectionVariant))
            {
                variantKvp.Value.RemoveAllIdleConnections();
            }
        }

        public static void TryToSendQueuedRequests()
        {
            foreach (var kvp in hosts)
                kvp.Value.TryToSendQueuedRequests();
        }

        public static void Shutdown()
        {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            Debug.Log($"[HostManager] [method:Shutdown] [msg] Shutdown initiated!");
#endif
            foreach (var kvp in hosts)
            {
                kvp.Value.Shutdown();
            }
        }

        public static void Clear()
        {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
            Debug.Log($"[HostManager] [method:Clear] [msg] Clearing hosts!");
#endif
            hosts.Clear();
        }

        private static void SetupFolder()
        {
            if (string.IsNullOrEmpty(LibraryPath))
            {
                try
                {
                    LibraryPath = System.IO.Path.Combine(HttpManager.GetRootCacheFolder(), "Hosts");
                    HttpManager.IOService.FileExists(LibraryPath);
                    IsSaveAndLoadSupported = true;
                }
                catch
                {
                    IsSaveAndLoadSupported = false;
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                    Debug.Log($"[HostManager] [method:SetupFolder] [msg] Save and load Disabled!");
#endif
                }
            }
        }

        public static void Save()
        {
            if (!IsSaveAndLoadSupported || string.IsNullOrEmpty(LibraryPath))
                return;

            try
            {
                using (var fs = HttpManager.IOService.CreateFileStream(LibraryPath, FileStreamModes.Create))
                using (var bw = new System.IO.BinaryWriter(fs))
                {
                    bw.Write(Version);

                    bw.Write(hosts.Count);
                    foreach (var kvp in hosts)
                    {
                        bw.Write(kvp.Key.ToString());

                        kvp.Value.SaveTo(bw);
                    }
                }
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                Debug.Log($"[HostManager] [method:Save] [msg] {hosts.Count} hosts saved!");
#endif
            }
            catch
            {
                // ignored
            }
        }

        public static void Load()
        {
            if (IsLoaded)
                return;
            IsLoaded = true;

            SetupFolder();

            if (!IsSaveAndLoadSupported || string.IsNullOrEmpty(LibraryPath) ||
                !HttpManager.IOService.FileExists(LibraryPath))
                return;

            try
            {
                using var fs = HttpManager.IOService.CreateFileStream(LibraryPath, FileStreamModes.OpenRead);
                using var br = new System.IO.BinaryReader(fs);
                int version = br.ReadInt32();

                int hostCount = br.ReadInt32();

                for (int i = 0; i < hostCount; ++i)
                {
                    GetHost(br.ReadString()).LoadFrom(version, br);
                }
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                Debug.Log($"[HostManager] [method:Load] [msg] {hostCount.ToString()} HostDefinitions loaded!");
#endif
            }
            catch
            {
                try
                {
                    HttpManager.IOService.FileDelete(LibraryPath);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}