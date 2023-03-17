using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.PlatformSupport.Threading;

namespace BestHTTP.Authentication
{
    /// <summary>
    /// 存储和管理已经收到的摘要信息。
    /// </summary>
    public static class DigestStore
    {
        private
            static
            readonly
            Dictionary<string, Digest> Digests = new Dictionary<string, Digest>();

        private
            static
            readonly
            System.Threading.ReaderWriterLockSlim RwLock =
                new System.Threading.ReaderWriterLockSlim(System.Threading.LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// 插件支持的算法数组。按优先级排序(第一个优先级最高)。
        /// </summary>
        private
            static
            readonly
            string[] SupportedAlgorithms = new string[] { "digest", "basic" };

        public
            static
            Digest Get(Uri uri)
        {
            using (new ReadLock(RwLock))
            {
                if (!Digests.TryGetValue(uri.Host, out var digest)) return null;
                var digestData = !digest.IsUriProtected(uri) ? null : digest;
                return digestData;
            }
        }

        /// <summary>
        /// 它将为给定的Uri检索或创建一个新的Digest。
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public 
            static 
            Digest GetOrCreate(Uri uri)
        {
            using (new WriteLock(RwLock))
            {
                if (!Digests.TryGetValue(uri.Host, out var digest))
                {
                    Digests.Add(uri.Host, digest = new Digest(uri));
                }

                return digest;
            }
        }

        public 
            static 
            void Remove(Uri uri)
        {
            using (new WriteLock(RwLock))
            {
                Digests.Remove(uri.Host);
            }
        }

        public 
            static 
            string FindBest(List<string> authHeaders)
        {
            if (authHeaders == null || authHeaders.Count == 0)
            {
                return string.Empty;
            }

            var headers = new List<string>(authHeaders.Count);
            headers.AddRange(authHeaders.Select(t => t.ToLower()));

            foreach (var t in SupportedAlgorithms)
            {
                var idx = headers.FindIndex(
                    header =>
                    header.StartsWith(t));
                if (idx != -1)
                {
                    return authHeaders[idx];
                }
            }

            return string.Empty;
        }
    }
}