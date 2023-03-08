#if !BESTHTTP_DISABLE_CACHING

using System;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Caching
{
    public abstract class HttpCacheMaintenanceParams
    {
        public HttpCacheMaintenanceParams(TimeSpan deleteOlder, ulong maxCacheSize)
        {
            this.DeleteOlder = deleteOlder;
            this.MaxCacheSize = maxCacheSize;
        }

        /// <summary>
        /// 删除访问时间早于此值的缓存项。如果TimeSpan.FromSeconds(0)被使用，那么所有的缓存条目都将被删除。使用TimeSpan.FromDays(2)，超过两天的条目将被删除。
        /// </summary>
        public TimeSpan DeleteOlder { get; private set; }

        /// <summary>
        /// 如果缓存大于第一个维护步骤之后的MaxCacheSize，那么维护作业将强制删除最后一次访问的最老的缓存项。
        /// </summary>
        public ulong MaxCacheSize { get; private set; }
    }
}

#endif