using System.Collections.Generic;
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
            HttpManager.Logger.Information("HostManager", "RemoveAllIdleConnections");
            foreach (var host_kvp in hosts)
            foreach (var variant_kvp in host_kvp.Value.HostConnectionVariant)
                variant_kvp.Value.RemoveAllIdleConnections();
        }

        public static void TryToSendQueuedRequests()
        {
            foreach (var kvp in hosts)
                kvp.Value.TryToSendQueuedRequests();
        }

        public static void Shutdown()
        {
            HttpManager.Logger.Information("HostManager", "Shutdown initiated!");
            foreach (var kvp in hosts)
                kvp.Value.Shutdown();
        }

        public static void Clear()
        {
            HttpManager.Logger.Information("HostManager", "Clearing hosts!");
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
                    HttpManager.Logger.Warning("HostManager", "Save and load Disabled!");
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

                HttpManager.Logger.Information("HostManager", hosts.Count + " hosts saved!");
            }
            catch
            {
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
                using (var fs = HttpManager.IOService.CreateFileStream(LibraryPath, FileStreamModes.OpenRead))
                using (var br = new System.IO.BinaryReader(fs))
                {
                    int version = br.ReadInt32();

                    int hostCount = br.ReadInt32();

                    for (int i = 0; i < hostCount; ++i)
                    {
                        GetHost(br.ReadString())
                            .LoadFrom(version, br);
                    }

                    HttpManager.Logger.Information("HostManager", hostCount.ToString() + " HostDefinitions loaded!");
                }
            }
            catch
            {
                try
                {
                    HttpManager.IOService.FileDelete(LibraryPath);
                }
                catch
                {
                }
            }
        }
    }
}