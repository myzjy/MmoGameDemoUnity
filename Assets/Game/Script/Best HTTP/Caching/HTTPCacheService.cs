#if !BESTHTTP_DISABLE_CACHING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BestHTTP.Core;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.FileSystem;
using BestHTTP.PlatformSupport.Threading;

//
// Version 1: Initial release
// Version 2: Filenames are generated from an index.
//

// ReSharper disable once CheckNamespace
namespace BestHTTP.Caching
{
    public sealed class UriComparer : IEqualityComparer<Uri>
    {
        public bool Equals(Uri x, Uri y)
        {
            return Uri.Compare(x, y, UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped, StringComparison.Ordinal) ==
                   0;
        }

        public int GetHashCode(Uri uri)
        {
            return uri.ToString().GetHashCode();
        }
    }


    public static class HttpCacheService
    {
        static HttpCacheService()
        {
            _nextNameIdx = 0x0001;
        }

        #region Properties & Fields

        /// <summary>
        /// 库文件格式版本控制支持
        /// </summary>
        private const int LibraryVersion = 3;

        public static bool IsSupported
        {
            get
            {
                if (_isSupportCheckDone)
                    return _isSupported;

                try
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                    // 显式禁用WebGL下的缓存
                    isSupported = false;
#else
                    // ReSharper disable once CommentTypo
                    //如果DirectoryExists抛出异常，我们将issupported设置为false
                    HttpManager.IOService.DirectoryExists(HttpManager.GetRootCacheFolder());
                    _isSupported = true;
#endif
                }
                catch
                {
                    _isSupported = false;

                    HttpManager.Logger.Warning("HTTPCacheService", "Cache Service Disabled!");
                }
                finally
                {
                    _isSupportCheckDone = true;
                }

                return _isSupported;
            }
        }

        private static bool _isSupported;
        private static bool _isSupportCheckDone;

        private static Dictionary<Uri, HttpCacheFileInfo> _library;

        private static readonly ReaderWriterLockSlim RwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private static readonly Dictionary<ulong, HttpCacheFileInfo> UsedIndexes =
            new Dictionary<ulong, HttpCacheFileInfo>();

        internal static string CacheFolder { get; private set; }
        private static string LibraryPath { get; set; }

        private static volatile bool _inClearThread;
        private static volatile bool _inMaintenanceThread;

        /// <summary>
        /// 当服务处于Clear或Maintenance线程中时，此属性返回true。
        /// </summary>
        public static bool IsDoingMaintenance => _inClearThread || _inMaintenanceThread;

        /// <summary>
        /// 存储下一个存储实体的索引。实体的文件名是从这个索引生成的。
        /// </summary>
        private static ulong _nextNameIdx;

        #endregion

        #region Common Functions

        internal static void CheckSetup()
        {
            if (!HttpCacheService.IsSupported)
                return;

            try
            {
                SetupCacheFolder();
                LoadLibrary();
            }

            catch
            {
                // ignored
            }
        }

        private static void SetupCacheFolder()
        {
            if (!HttpCacheService.IsSupported)
                return;

            try
            {
                if (string.IsNullOrEmpty(CacheFolder) || string.IsNullOrEmpty(LibraryPath))
                {
                    CacheFolder = System.IO.Path.Combine(HttpManager.GetRootCacheFolder(), "HTTPCache");
                    if (!HttpManager.IOService.DirectoryExists(CacheFolder))
                        HttpManager.IOService.DirectoryCreate(CacheFolder);

                    LibraryPath = System.IO.Path.Combine(HttpManager.GetRootCacheFolder(), "Library");
                }
            }
            catch
            {
                _isSupported = false;

                HttpManager.Logger.Warning("HTTPCacheService", "Cache Service Disabled!");
            }
        }

        internal static ulong GetNameIdx()
        {
            ulong result = _nextNameIdx;

            do
            {
                _nextNameIdx = ++_nextNameIdx % ulong.MaxValue;
            } while (UsedIndexes.ContainsKey(_nextNameIdx));

            return result;
        }

        public static bool HasEntity(Uri uri)
        {
            if (!IsSupported)
                return false;

            CheckSetup();

            using (new ReadLock(RwLock))
                return _library.ContainsKey(uri);
        }

        public static bool DeleteEntity(Uri uri, bool removeFromLibrary = true)
        {
            if (!IsSupported)
                return false;

            // 2019.05.10: 除库上的锁外，删除了所有锁。

            CheckSetup();

            using (new WriteLock(RwLock))
            {
                DeleteEntityImpl(uri, removeFromLibrary);

                return true;
            }
        }

        private static void DeleteEntityImpl(Uri uri, bool removeFromLibrary = true, bool useLocking = false)
        {
            bool inStats = _library.TryGetValue(uri, out var info);
            if (inStats)
                info.Delete();

            if (inStats && removeFromLibrary)
            {
                if (useLocking)
                    RwLock.EnterWriteLock();
                try
                {
                    _library.Remove(uri);
                    UsedIndexes.Remove(info.MappedNameIdx);
                }
                finally
                {
                    if (useLocking)
                        RwLock.ExitWriteLock();
                }
            }

            PluginEventHelper.EnqueuePluginEvent(new PluginEventInfo(PluginEvents.SaveCacheLibrary));
        }

        internal static bool IsCachedEntityExpiresInTheFuture(HttpRequest request)
        {
            if (!IsSupported || request.DisableCache)
                return false;

            CheckSetup();

            HttpCacheFileInfo info;

            using (new ReadLock(RwLock))
            {
                if (!_library.TryGetValue(request.CurrentUri, out info))
                    return false;
            }

            return info.WillExpireInTheFuture(
                request.State == HttpRequestStates.ConnectionTimedOut ||
                request.State == HttpRequestStates.TimedOut ||
                request.State == HttpRequestStates.Error ||
                (request.State == HttpRequestStates.Finished &&
                 request.Response != null && request.Response.StatusCode >= 500));
        }

        /// <summary>
        /// 根据规范设置缓存控制头的实用程序函数.: http://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html#sec13.3.4
        /// </summary>
        /// <param name="request"></param>
        internal static void SetHeaders(HttpRequest request)
        {
            if (!IsSupported)
                return;

            CheckSetup();

            request.RemoveHeader("If-None-Match");
            request.RemoveHeader("If-Modified-Since");

            HttpCacheFileInfo info;

            using (new ReadLock(RwLock))
            {
                if (!_library.TryGetValue(request.CurrentUri, out info))
                    return;
            }

            info.SetUpRevalidationHeaders(request);
        }

        #endregion

        #region Get Functions

        public static HttpCacheFileInfo GetEntity(Uri uri)
        {
            if (!IsSupported)
                return null;

            CheckSetup();

            HttpCacheFileInfo info;

            using (new ReadLock(RwLock))
                _library.TryGetValue(uri, out info);

            return info;
        }

        internal static HttpResponse GetFullResponse(HttpRequest request)
        {
            if (!IsSupported)
                return null;

            CheckSetup();

            using (new ReadLock(RwLock))
            {
                return !_library.TryGetValue(request.CurrentUri, out var info) ? null : info.ReadResponseTo(request);
            }
        }

        #endregion

        #region Storing

        /// <summary>
        /// 检查是否可以缓存给定的响应. http://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html#sec13.4
        /// </summary>
        /// <returns>如果可缓存(Cacheable)则返回true，否则返回false.</returns>
        internal static bool IsCacheable(Uri uri, HttpMethods method, HttpResponse response)
        {
            if (!IsSupported)
                return false;

            if (method != HttpMethods.Get)
                return false;

            if (response == null)
                return false;

            // https://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html#sec13.12 - Cache Replacement
            // 它可以将它插入缓存存储，如果它满足所有其他要求，可以使用它来响应任何未来的请求，这些请求以前会导致返回旧的响应。
            //if (response.StatusCode == 304)
            //    return false;

            // Partial response
            if (response.StatusCode == 206)
                return false;

            if (response.StatusCode < 200 || response.StatusCode >= 400)
                return false;

            //http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.9.2
            bool hasValidMaxAge = false;
            var cacheControls = response.GetHeaderValues("cache-control");
            if (cacheControls != null)
            {
                if (cacheControls.Exists(headerValue =>
                    {
                        var parser = new HeaderParser(headerValue);
                        if (parser.Values == null || parser.Values.Count <= 0) return false;
                        foreach (var value in parser.Values)
                        {
                            switch (value.Key)
                            {
                                // https://csswizardry.com/2019/03/cache-control-for-civilians/#no-store
                                case "no-store":
                                    return true;
                                case "max-age" when value.HasValue:
                                {
                                    if (double.TryParse(value.Value, out var maxAge))
                                    {
                                        // A negative max-age value is a no cache
                                        if (maxAge <= 0)
                                            return true;
                                        hasValidMaxAge = true;
                                    }

                                    break;
                                }
                            }
                        }

                        return false;
                    }))
                    return false;
            }

            var pragmas = response.GetHeaderValues("pragma");
            if (pragmas != null)
            {
                if (pragmas.Exists(headerValue =>
                    {
                        string value = headerValue.ToLower();
                        return value.Contains("no-store") || value.Contains("no-cache");
                    }))
                    return false;
            }

            // 还不支持带有字节范围的响应。
            var byteRanges = response.GetHeaderValues("content-range");
            if (byteRanges != null)
                return false;

            // 只有当至少有一个缓存头具有适当的值时才存储
            var etag = response.GetFirstHeaderValue("ETag");
            if (!string.IsNullOrEmpty(etag))
                return true;

            var expires = response.GetFirstHeaderValue("Expires").ToDateTime(DateTime.FromBinary(0));
            if (expires >= DateTime.UtcNow)
                return true;

            return response.GetFirstHeaderValue("Last-Modified") != null || hasValidMaxAge;
        }

        internal static HttpCacheFileInfo Store(Uri uri, HttpMethods method, HttpResponse response)
        {
            if (response?.Data == null || response.Data.Length == 0)
                return null;

            if (!IsSupported)
                return null;

            CheckSetup();

            HttpCacheFileInfo info;

            using (new WriteLock(RwLock))
            {
                if (!_library.TryGetValue(uri, out info))
                {
                    _library.Add(uri, info = new HttpCacheFileInfo(uri));
                    UsedIndexes.Add(info.MappedNameIdx, info);
                }

                try
                {
                    info.Store(response);
                    if (HttpManager.Logger.Level == Logger.Loglevels.All)
                        HttpManager.Logger.Verbose("HTTPCacheService",
                            $"{uri} - Saved to cache", response.BaseRequest.Context);
                }
                catch
                {
                    // 如果在我们写出响应时发生了什么事情，那么我们将删除它，因为它可能处于无效状态。
                    DeleteEntityImpl(uri);

                    throw;
                }
            }

            return info;
        }

        internal static void SetUpCachingValues(Uri uri, HttpResponse response)
        {
            if (!IsSupported)
                return;

            CheckSetup();

            using (new WriteLock(RwLock))
            {
                if (!_library.TryGetValue(uri, out var info))
                {
                    _library.Add(uri, info = new HttpCacheFileInfo(uri));
                    UsedIndexes.Add(info.MappedNameIdx, info);
                }

                try
                {
                    info.SetUpCachingValues(response);
                    if (HttpManager.Logger.Level == Logger.Loglevels.All)
                        HttpManager.Logger.Verbose("HTTPCacheService",
                            $"{uri} - SetUpCachingValues done!",
                            response.BaseRequest.Context);
                }
                catch
                {
                    // 如果在我们写出响应时发生了什么事情，那么我们将删除它，因为它可能处于无效状态。
                    DeleteEntityImpl(uri);

                    throw;
                }
            }
        }

        internal static System.IO.Stream PrepareStreamed(Uri uri, HttpResponse response)
        {
            if (!IsSupported)
                return null;

            CheckSetup();

            HttpCacheFileInfo info;

            using (new WriteLock(RwLock))
            {
                if (!_library.TryGetValue(uri, out info))
                {
                    _library.Add(uri, info = new HttpCacheFileInfo(uri));
                    UsedIndexes.Add(info.MappedNameIdx, info);
                }
            }

            try
            {
                return info.GetSaveStream(response);
            }
            catch
            {
                // 如果在我们写出响应时发生了什么事情，那么我们将删除它，因为它可能处于无效状态。
                DeleteEntityImpl(uri, true, true);

                throw;
            }
        }

        #endregion

        #region Public Maintenance Functions

        /// <summary>
        /// 删除所有缓存实体。非阻塞。
        /// <remarks>只有在当前没有处理请求时才调用它，因为缓存条目可以在服务器返回304结果时删除，因此将没有数据要从缓存中读取!</remarks>
        /// </summary>
        private static void BeginClear()
        {
            if (!IsSupported)
                return;

            if (_inClearThread)
                return;
            _inClearThread = true;

            SetupCacheFolder();

            ThreadedRunner.RunShortLiving(ClearImpl);
        }

        private static void ClearImpl()
        {
            if (!IsSupported)
                return;

            CheckSetup();

            using (new WriteLock(RwLock))
            {
                try
                {
                    // GetFiles将返回一个字符串数组，其中包含文件夹中具有完整路径的文件
                    string[] cacheEntries = HttpManager.IOService.GetFiles(CacheFolder);

                    if (cacheEntries == null) return;
                    foreach (var t in cacheEntries)
                    {
                        // 我们需要一个try-catch块，因为在目录之间。GetFiles调用和File。Delete调用维护作业或其他文件操作可以删除缓存文件夹中的任何文件。
                        // 因此，虽然任何文件都可能存在问题，但我们不想终止整个for循环
                        try
                        {
                            HttpManager.IOService.FileDelete(t);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                finally
                {
                    UsedIndexes.Clear();
                    _library.Clear();
                    _nextNameIdx = 0x0001;

                    _inClearThread = false;

                    PluginEventHelper.EnqueuePluginEvent(new PluginEventInfo(PluginEvents.SaveCacheLibrary));
                }
            }
        }

        /// <summary>
        ///删除所有过期的缓存实体。
        /// <remarks>只有在当前没有处理请求时才调用它，因为缓存条目可以在服务器返回304结果时删除，因此将没有数据要从缓存中读取!</remarks>
        /// </summary>
        public static void BeginMaintenance(HttpCacheMaintenanceParams maintenanceParam)
        {
            if (maintenanceParam == null)
                throw new ArgumentNullException(nameof(maintenanceParam));

            if (!HttpCacheService.IsSupported)
                return;

            if (_inMaintenanceThread)
                return;

            _inMaintenanceThread = true;

            SetupCacheFolder();

            ThreadedRunner.RunShortLiving(MaintenanceImpl, maintenanceParam);
        }

        private static void MaintenanceImpl(HttpCacheMaintenanceParams maintenanceParam)
        {
            CheckSetup();

            using (new WriteLock(RwLock))
            {
                try
                {
                    // 删除早于给定时间的缓存项。
                    DateTime deleteOlderAccessed = DateTime.UtcNow - maintenanceParam.DeleteOlder;
                    List<HttpCacheFileInfo> removedEntities = new List<HttpCacheFileInfo>();
                    foreach (var kvp in _library)
                        if (kvp.Value.LastAccess < deleteOlderAccessed)
                        {
                            DeleteEntityImpl(kvp.Key, false);
                            removedEntities.Add(kvp.Value);
                        }

                    foreach (var t in removedEntities)
                    {
                        _library.Remove(t.Uri);
                        UsedIndexes.Remove(t.MappedNameIdx);
                    }

                    removedEntities.Clear();

                    ulong cacheSize = GetCacheSizeImpl();

                    // 此步骤将删除所有以最老的LastAccess属性开始的条目，同时缓存大小大于给定参数中的MaxCacheSize。
                    if (cacheSize <= maintenanceParam.MaxCacheSize) return;
                    {
                        var fileInfos = new List<HttpCacheFileInfo>(_library.Count);
                        fileInfos.AddRange(_library.Select(kvp => kvp.Value));

                        fileInfos.Sort();

                        var idx = 0;
                        while (cacheSize >= maintenanceParam.MaxCacheSize && idx < fileInfos.Count)
                        {
                            try
                            {
                                var fi = fileInfos[idx];
                                var length = (ulong)fi.BodyLength;

                                DeleteEntityImpl(fi.Uri);

                                cacheSize -= length;
                            }
                            catch
                            {
                                // ignored
                            }
                            finally
                            {
                                ++idx;
                            }
                        }
                    }
                }
                finally
                {
                    _inMaintenanceThread = false;

                    PluginEventHelper.EnqueuePluginEvent(new PluginEventInfo(PluginEvents.SaveCacheLibrary));
                }
            }
        }

        public static int GetCacheEntityCount()
        {
            if (!HttpCacheService.IsSupported)
                return 0;

            CheckSetup();

            using (new ReadLock(RwLock))
            {
                return _library.Count;
            }
        }

        public static ulong GetCacheSize()
        {
            if (!IsSupported)
                return 0;

            CheckSetup();

            using (new ReadLock(RwLock))
            {
                return GetCacheSizeImpl();
            }
        }

        private static ulong GetCacheSizeImpl()
        {
            return _library.Where(kvp => kvp.Value.BodyLength > 0)
                .Aggregate<KeyValuePair<Uri, HttpCacheFileInfo>, ulong>(0,
                    (current, kvp) => current + (ulong)kvp.Value.BodyLength);
        }

        #endregion

        #region Cache Library Management

        private static void LoadLibrary()
        {
            // Already loaded?
            if (_library != null)
                return;

            if (!IsSupported)
                return;

            var version = 1;

            using (new WriteLock(RwLock))
            {
                _library = new Dictionary<Uri, HttpCacheFileInfo>(new UriComparer());
                try
                {
                    using var fs = HttpManager.IOService.CreateFileStream(LibraryPath, FileStreamModes.OpenRead);
                    using var br = new System.IO.BinaryReader(fs);
                    version = br.ReadInt32();

                    if (version > 1)
                        _nextNameIdx = br.ReadUInt64();

                    var statCount = br.ReadInt32();

                    for (var i = 0; i < statCount; ++i)
                    {
                        var uri = new Uri(br.ReadString());

                        var entity = new HttpCacheFileInfo(uri, br, version);
                        if (!entity.IsExists()) continue;
                        _library.Add(uri, entity);

                        if (version > 1)
                        {
                            UsedIndexes.Add(entity.MappedNameIdx, entity);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HttpManager.Logger.Level == Logger.Loglevels.All)
                        HttpManager.Logger.Exception("HTTPCacheService", "LoadLibrary", ex);
                }
            }

            if (version == 1)
                BeginClear();
            else
                DeleteUnusedFiles();
        }

        internal static void SaveLibrary()
        {
            if (_library == null)
                return;

            if (!IsSupported)
                return;

            using (new WriteLock(RwLock))
            {
                try
                {
                    using var fs = HttpManager.IOService.CreateFileStream(LibraryPath, FileStreamModes.Create);
                    using var bw = new System.IO.BinaryWriter(fs);
                    bw.Write(LibraryVersion);
                    bw.Write(_nextNameIdx);

                    bw.Write(_library.Count);
                    foreach (var kvp in _library)
                    {
                        bw.Write(kvp.Key.ToString());

                        kvp.Value.SaveTo(bw);
                    }
                }
                catch (Exception ex)
                {
                    if (HttpManager.Logger.Level == Logger.Loglevels.All)
                    {
                        HttpManager.Logger.Exception("HTTPCacheService", "SaveLibrary", ex);
                    }
                }
            }
        }

        internal static void SetBodyLength(Uri uri, int bodyLength)
        {
            if (!IsSupported)
                return;

            CheckSetup();

            using (new WriteLock(RwLock))
            {
                if (_library.TryGetValue(uri, out var fileInfo))
                    fileInfo.BodyLength = bodyLength;
                else
                {
                    _library.Add(uri, fileInfo = new HttpCacheFileInfo(uri, DateTime.UtcNow, bodyLength));
                    UsedIndexes.Add(fileInfo.MappedNameIdx, fileInfo);
                }
            }
        }

        /// <summary>
        ///从缓存文件夹中删除不在库中的所有文件。
        /// </summary>
        private static void DeleteUnusedFiles()
        {
            if (!IsSupported)
                return;

            CheckSetup();

            // GetFiles将返回一个字符串数组，其中包含文件夹中具有完整路径的文件
            string[] cacheEntries = HttpManager.IOService.GetFiles(CacheFolder);

            foreach (var cacheItem in cacheEntries)
            {
                //我们需要一个try-catch块，因为在目录之间。GetFiles调用和File。Delete调用维护作业或其他文件操作可以删除缓存文件夹中的任何文件。
                //因此，虽然任何文件都可能存在问题，但我们不想终止整个for循环
                try
                {
                    var filename = System.IO.Path.GetFileName(cacheItem);
                    bool deleteFile;
                    if (ulong.TryParse(filename, System.Globalization.NumberStyles.AllowHexSpecifier, null,
                            out var idx))
                    {
                        using (new ReadLock(RwLock))
                            deleteFile = !UsedIndexes.ContainsKey(idx);
                    }
                    else
                        deleteFile = true;

                    if (deleteFile)
                        HttpManager.IOService.FileDelete(cacheItem);
                }
                catch
                {
                    // ignored
                }
            }
        }

        #endregion
    }
}

#endif