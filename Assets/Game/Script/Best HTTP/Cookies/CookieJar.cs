#if !BESTHTTP_DISABLE_COOKIES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BestHTTP.Core;
using BestHTTP.PlatformSupport.FileSystem;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Cookies
{
    /// <summary>
    /// The Cookie Jar implementation based on RFC 6265(http://tools.ietf.org/html/rfc6265).
    /// </summary>
    public static class CookieJar
    {
        // cookie商店的版本。为了保持兼容性，可以在未来的版本中使用它。
        private const int Version = 1;

        /// <summary>
        /// 插件会删除之前访问过这个阈值的cookie。默认值为7天.
        /// </summary>
        private static readonly TimeSpan AccessThreshold = TimeSpan.FromDays(7);

        /// <summary>
        /// 如果支持File api则返回true。
        /// </summary>
        private static bool IsSavingSupported
        {
            get
            {
#if !BESTHTTP_DISABLE_COOKIE_SAVE
                if (_isSupportCheckDone)
                    return _isSavingSupported;

                try
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                    _isSavingSupported = false;
#else
                    HttpManager.IOService.DirectoryExists(HttpManager.GetRootCacheFolder());
                    _isSavingSupported = true;
#endif
                }
                catch
                {
                    _isSavingSupported = false;

                    HttpManager.Logger.Warning("CookieJar", "Cookie saving and loading disabled!");
                }
                finally
                {
                    _isSupportCheckDone = true;
                }

                return _isSavingSupported;
#else
                    return false;
#endif
            }
        }

        #region Private Helper Functions

        /// <summary>
        /// 找到并返回一个Cookie及其在列表中的索引。
        /// </summary>
        private static Cookie Find(Cookie cookie, out int idx)
        {
            for (var i = 0; i < Cookies.Count; ++i)
            {
                var c = Cookies[i];

                if (!c.Equals(cookie)) continue;
                idx = i;
                return c;
            }

            idx = -1;
            return null;
        }

        #endregion

        #region Privates

        /// <summary>
        /// List of the Cookies
        /// </summary>
        private static readonly List<Cookie> Cookies = new List<Cookie>();

        private static string CookieFolder { get; set; }
        private static string LibraryPath { get; set; }

        /// <summary>
        /// Synchronization object for thread safety.
        /// </summary>
        private static readonly ReaderWriterLockSlim RwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

#if !BESTHTTP_DISABLE_COOKIE_SAVE
        private static bool _isSavingSupported;
        private static bool _isSupportCheckDone;
#endif

        private static bool _loaded;

        #endregion

        #region Internal Functions

        internal static void SetupFolder()
        {
#if !BESTHTTP_DISABLE_COOKIE_SAVE
            if (!CookieJar.IsSavingSupported)
                return;

            try
            {
                if (!string.IsNullOrEmpty(CookieFolder) && !string.IsNullOrEmpty(LibraryPath)) return;
                CookieFolder = System.IO.Path.Combine(HttpManager.GetRootCacheFolder(), "Cookies");
                LibraryPath = System.IO.Path.Combine(CookieFolder, "Library");
            }
            catch
            {
                // ignored
            }
#endif
        }

        /// <summary>
        /// 将设置或更新来自响应对象的所有cookie。
        /// </summary>
        internal static bool Set(HttpResponse response)
        {
            if (response == null)
                return false;

            var newCookies = new List<Cookie>();
            var setCookieHeaders = response.GetHeaderValues("set-cookie");

            // No cookies. :'(
            if (setCookieHeaders == null)
                return false;

            foreach (var cookie in setCookieHeaders
                         .Select(cookieHeader => Cookie.Parse(cookieHeader, response.baseRequest.CurrentUri,
                             response.baseRequest.Context)).Where(cookie => cookie != null))
            {
                RwLock.EnterWriteLock();
                try
                {
                    var old = Find(cookie, out var idx);

                    //如果cookie没有值或已经过期，服务器会要求我们删除该cookie
                    var expired = string.IsNullOrEmpty(cookie.Value) || !cookie.WillExpireInTheFuture();

                    if (!expired)
                    {
                        // 没有旧Cookie，直接添加到列表中
                        if (old == null)
                        {
                            Cookies.Add(cookie);

                            newCookies.Add(cookie);
                        }
                        else
                        {
                            // 更新新创建的cookie的创建时间，以匹配旧cookie的创建时间.
                            cookie.Date = old.Date;
                            Cookies[idx] = cookie;

                            newCookies.Add(cookie);
                        }
                    }
                    else if (idx != -1)
                    {
                        // 删除cookie
                        Cookies.RemoveAt(idx);
                    }
                }
                catch
                {
                    // Ignore cookie on error
                }
                finally
                {
                    RwLock.ExitWriteLock();
                }
            }

            response.Cookies = newCookies;

            PluginEventHelper.EnqueuePluginEvent(new PluginEventInfo(PluginEvents.SaveCookieLibrary));

            return true;
        }

        /// <summary>
        /// 删除所有过期或“旧”的cookie，并将cookie的总和大小保持在给定的大小以下。
        /// </summary>
        private static void Maintain(bool sendEvent)
        {
            // 这和rfc中的不一样:
            //  http://tools.ietf.org/html/rfc6265#section-5.3

            RwLock.EnterWriteLock();
            try
            {
                uint size = 0;

                for (var i = 0; i < Cookies.Count;)
                {
                    var cookie = Cookies[i];

                    // 删除过期或不使用的cookies
                    if (!cookie.WillExpireInTheFuture() || (cookie.LastAccess + AccessThreshold) < DateTime.UtcNow)
                    {
                        Cookies.RemoveAt(i);
                    }
                    else
                    {
                        if (!cookie.IsSession)
                        {
                            size += cookie.GuessSize();
                        }

                        i++;
                    }
                }

                if (size > HttpManager.CookieJarSize)
                {
                    Cookies.Sort();

                    while (size > HttpManager.CookieJarSize && Cookies.Count > 0)
                    {
                        var cookie = Cookies[0];
                        Cookies.RemoveAt(0);

                        size -= cookie.GuessSize();
                    }
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                RwLock.ExitWriteLock();
            }

            if (sendEvent)
                PluginEventHelper.EnqueuePluginEvent(new PluginEventInfo(PluginEvents.SaveCookieLibrary));
        }

        /// <summary>
        /// 将Cookie Jar保存到一个文件。
        /// </summary>
        /// <remarks>未在Unity WebPlayer下实现</remarks>
        internal static void Persist()
        {
#if !BESTHTTP_DISABLE_COOKIE_SAVE
            if (!IsSavingSupported)
                return;

            if (!_loaded)
                return;

            // Delete any expired cookie
            Maintain(false);

            RwLock.EnterWriteLock();
            try
            {
                if (!HttpManager.IOService.DirectoryExists(CookieFolder))
                    HttpManager.IOService.DirectoryCreate(CookieFolder);

                using var fs = HttpManager.IOService.CreateFileStream(LibraryPath, FileStreamModes.Create);
                using var bw = new System.IO.BinaryWriter(fs);
                bw.Write(Version);

                // 数一下我们有多少个非会话cookie
                var count = Cookies.Count(cookie => !cookie.IsSession);

                bw.Write(count);

                // 只保存可持久化的cookie
                foreach (var cookie in Cookies.Where(cookie => !cookie.IsSession))
                {
                    cookie.SaveTo(bw);
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                RwLock.ExitWriteLock();
            }
#endif
        }

        /// <summary>
        /// 从文件中加载以前持久化的cookie库。
        /// </summary>
        internal static void Load()
        {
#if !BESTHTTP_DISABLE_COOKIE_SAVE
            if (!IsSavingSupported)
                return;

            if (_loaded)
                return;

            SetupFolder();

            RwLock.EnterWriteLock();
            try
            {
                Cookies.Clear();

                if (!HttpManager.IOService.DirectoryExists(CookieFolder))
                    HttpManager.IOService.DirectoryCreate(CookieFolder);

                if (!HttpManager.IOService.FileExists(LibraryPath))
                    return;

                using var fs = HttpManager.IOService.CreateFileStream(LibraryPath, FileStreamModes.OpenRead);
                using var br = new System.IO.BinaryReader(fs);
                /*int version = */
                br.ReadInt32();
                var cookieCount = br.ReadInt32();

                for (var i = 0; i < cookieCount; ++i)
                {
                    var cookie = new Cookie();
                    cookie.LoadFrom(br);

                    if (cookie.WillExpireInTheFuture())
                        Cookies.Add(cookie);
                }
            }
            catch
            {
                Cookies.Clear();
            }
            finally
            {
                _loaded = true;
                RwLock.ExitWriteLock();
            }
#endif
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// 返回与给定Uri对应的所有cookie。
        /// </summary>
        public static List<Cookie> Get(Uri uri)
        {
            Load();

            RwLock.EnterReadLock();
            try
            {
                List<Cookie> result = null;

                foreach (var cookie in
                         Cookies.Where(cookie =>
                             cookie.WillExpireInTheFuture()
                             && (uri.Host.IndexOf(cookie.Domain, StringComparison.Ordinal) != -1 ||
                                 $"{uri.Host}:{uri.Port}".IndexOf(cookie.Domain, StringComparison.Ordinal) != -1) &&
                             uri.AbsolutePath.StartsWith(cookie.Path)))
                {
                    result ??= new List<Cookie>();

                    result.Add(cookie);
                }

                return result;
            }
            finally
            {
                RwLock.ExitReadLock();
            }
        }

        /// <summary>
        /// 将添加一个新的cookie，或覆盖已经存在的旧cookie。
        /// </summary>
        public static void Set(Uri uri, Cookie cookie)
        {
            Set(cookie);
        }

        /// <summary>
        /// 将添加一个新的cookie，或覆盖已经存在的旧cookie。
        /// </summary>
        private static void Set(Cookie cookie)
        {
            Load();

            RwLock.EnterWriteLock();
            try
            {
                Find(cookie, out var idx);

                if (idx >= 0)
                    Cookies[idx] = cookie;
                else
                    Cookies.Add(cookie);
            }
            finally
            {
                RwLock.ExitWriteLock();
            }

            PluginEventHelper.EnqueuePluginEvent(new PluginEventInfo(PluginEvents.SaveCookieLibrary));
        }

        public static List<Cookie> GetAll()
        {
            Load();

            return Cookies;
        }

        /// <summary>
        /// 删除Jar中的所有cookie。
        /// </summary>
        public static void Clear()
        {
            Load();

            RwLock.EnterWriteLock();
            try
            {
                Cookies.Clear();
            }
            finally
            {
                RwLock.ExitWriteLock();
            }

            Persist();
        }

        /// <summary>
        /// 删除比给定参数更老的cookie。
        /// </summary>
        public static void Clear(TimeSpan olderThan)
        {
            Load();

            RwLock.EnterWriteLock();
            try
            {
                for (var i = 0; i < Cookies.Count;)
                {
                    var cookie = Cookies[i];

                    // 删除过期或不使用的cookie
                    if (!cookie.WillExpireInTheFuture() || (cookie.Date + olderThan) < DateTime.UtcNow)
                    {
                        Cookies.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            finally
            {
                RwLock.ExitWriteLock();
            }

            Persist();
        }

        /// <summary>
        /// 删除与给定域匹配的cookie。
        /// </summary>
        public static void Clear(string domain)
        {
            Load();

            RwLock.EnterWriteLock();
            try
            {
                for (var i = 0; i < Cookies.Count;)
                {
                    var cookie = Cookies[i];

                    // 删除过期或不使用的cookie
                    if (!cookie.WillExpireInTheFuture() ||
                        cookie.Domain.IndexOf(domain, StringComparison.Ordinal) != -1)
                    {
                        Cookies.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            finally
            {
                RwLock.ExitWriteLock();
            }

            Persist();
        }

        public static void Remove(Uri uri, string name)
        {
            Load();

            RwLock.EnterWriteLock();
            try
            {
                for (var i = 0; i < Cookies.Count;)
                {
                    var cookie = Cookies[i];

                    if (cookie.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                        uri.Host.IndexOf(cookie.Domain, StringComparison.Ordinal) != -1)
                    {
                        Cookies.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            finally
            {
                RwLock.ExitWriteLock();
            }

            PluginEventHelper.EnqueuePluginEvent(new PluginEventInfo(PluginEvents.SaveCookieLibrary));
        }

        #endregion
    }
}

#endif