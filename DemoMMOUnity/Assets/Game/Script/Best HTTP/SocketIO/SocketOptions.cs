#if !BESTHTTP_DISABLE_SOCKETIO

using System;
using System.Text;

using PlatformSupport.Collections.ObjectModel;

#if !NETFX_CORE
    using PlatformSupport.Collections.Specialized;
#else
    using System.Collections.Specialized;
#endif

namespace BestHTTP.SocketIO
{
    public delegate void HttpRequestCallbackDelegate(SocketManager manager, HttpRequest request);

    public enum SupportedSocketIOVersions
    {
        Unknown,
        // ReSharper disable once InconsistentNaming
        v2,
        // ReSharper disable once InconsistentNaming
        v3
    }

    public sealed class SocketOptions
    {
        /// <summary>
        /// SocketManager将尝试连接此传输。
        /// </summary>
        public Transports.TransportTypes ConnectWith { get; set; }

        /// <summary>
        /// 断开连接后是否自动重新连接(默认为true)
        /// </summary>
        public bool Reconnection { get; set; }

        /// <summary>
        /// 放弃前的尝试次数(默认Int.MaxValue)
        /// </summary>
        public int ReconnectionAttempts { get; set; }

        /// <summary>
        /// 尝试重新连接之前的初始等待时间(默认1000ms)。
        /// 受+/- RandomizationFactor影响，例如默认初始延迟将在500ms到1500ms之间。
        /// </summary>
        public TimeSpan ReconnectionDelay { get; set; }

        /// <summary>
        /// 重连接之间的最大等待时间(默认为5000ms)。
        /// 每次尝试都会增加重连接延迟以及如上所述的随机化。
        /// </summary>
        public TimeSpan ReconnectionDelayMax { get; set; }

        /// <summary>
        /// (default 0.5`), [0..1]
        /// </summary>
        public float RandomizationFactor { get => _randomizationFactor;
            private set => _randomizationFactor = Math.Min(1.0f, Math.Max(0.0f, value));
        }
        private float _randomizationFactor;

        /// <summary>
        /// 在触发connect_error和connect_timeout事件之前的连接超时(默认为20000ms)
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// 通过将此设置为false，您必须在您认为合适的时候调用SocketManager的Open()。
        /// </summary>
        public bool AutoConnect { get; set; }

        /// <summary>
        /// 将为访问的uri传递的其他查询参数。如果值为null或空字符串，它将不会被添加到查询中，只会被添加到键。
        /// <remarks>键和值必须正确转义，因为插件不会转义它们。 </remarks>
        /// </summary>
        public ObservableDictionary<string, string> AdditionalQueryParams
        {
            get => _additionalQueryParams;
            set
            {
                // Unsubscribe from previous dictionary's events
                if (_additionalQueryParams != null)
                    _additionalQueryParams.CollectionChanged -= AdditionalQueryParams_CollectionChanged;

                _additionalQueryParams = value;

                // Clear out the cached value
                _builtQueryParams = null;

                // Subscribe to the collection changed event
                if (value != null)
                    value.CollectionChanged += AdditionalQueryParams_CollectionChanged;
            }
        }
        private ObservableDictionary<string, string> _additionalQueryParams;

        /// <summary>
        /// 如果为false, AdditionalQueryParams中的参数将被传递给所有HTTP请求。默认值为true。
        /// </summary>
        public bool QueryParamsOnlyForHandshake { get; set; }

        /// <summary>
        /// 调用套接字的每个HTTPRequest的回调。IO协议发送出去。它可以用于进一步定制(例如添加额外的请求)请求。
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public HttpRequestCallbackDelegate httpRequestCustomizationCallback { get; set; }

        /// <summary>
        /// 套接字。服务器的IO协议版本。如果保留默认值(Unknown)，插件将尝试检测服务器版本。
        /// </summary>
        public SupportedSocketIOVersions ServerVersion { get; set; }

        /// <summary>
        /// 从Socket开始。IO v3，连接到一个命名空间，客户端可以发送有效负载数据。当设置了Auth回调函数时，插件将在连接到命名空间时调用它。它的返回值必须是json字符串!
        /// </summary>
        public readonly Func<SocketManager, Socket, string> Auth;


        /// <summary>
        /// BuildQueryParams()调用结果的缓存值。
        /// </summary>
        private string _builtQueryParams;

        /// <summary>
        /// 构造函数，设置默认选项值。
        /// </summary>
        public SocketOptions(Func<SocketManager, Socket, string> auth)
        {
            Auth = auth;
            ConnectWith = Transports.TransportTypes.Polling;
            Reconnection = true;
            ReconnectionAttempts = int.MaxValue;
            ReconnectionDelay = TimeSpan.FromMilliseconds(1000);
            ReconnectionDelayMax = TimeSpan.FromMilliseconds(5000);
            RandomizationFactor = 0.5f;
            Timeout = TimeSpan.FromMilliseconds(20000);
            AutoConnect = true;
            QueryParamsOnlyForHandshake = true;
        }
        /// <summary>
        /// 将AdditionalQueryParams中的键和值构建为键=值表单。如果AdditionalQueryParams为空或空，它将返回一个空字符串。
        /// </summary>
        internal string BuildQueryParams()
        {
            if (AdditionalQueryParams == null || AdditionalQueryParams.Count == 0)
                return string.Empty;

            if (!string.IsNullOrEmpty(_builtQueryParams))
                return _builtQueryParams;

            StringBuilder sb = new StringBuilder(AdditionalQueryParams.Count * 4);

            foreach(var kvp in AdditionalQueryParams)
            {
                sb.Append("&");
                sb.Append(kvp.Key);

                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    sb.Append("=");
                    sb.Append(kvp.Value);
                }
            }

            return _builtQueryParams = sb.ToString();
        }

        /// <summary>
        /// 当AdditionalQueryPrams字典改变时，此事件将被调用。我们必须重置缓存的值。
        /// </summary>
        private void AdditionalQueryParams_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _builtQueryParams = null;
        }
    }
}

#endif
