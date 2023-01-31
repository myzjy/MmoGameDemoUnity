#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2

using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Connections.HTTP2
{
    // https://httpwg.org/specs/rfc7540.html#iana-settings
    public enum Http2Settings : ushort
    {
        /// <summary>
        /// <code>
        /// 允许发送端通知远程端点用于解码报头块的报头压缩表的最大大小(以字节为单位)。
        /// 编码器可以通过在报头块中使用特定于报头压缩格式的信号来选择等于或小于此值的任何大小(参见[compression])。
        /// 初始值是4096个字节.
        ///  </code>
        /// </summary>
        HeaderTableSize = 0x01,

        /// <summary>
        /// <code>
        /// 此设置可用于禁用服务器推送(Section 8.2)。
        /// 如果端点接收到PUSH_PROMISE参数设置为0，则绝对不能发送PUSH_PROMISE帧。
        /// 终端必须将接收到的PUSH_PROMISE帧作为类型为协议错误的连接错误(Section5.4.1)处理。
        /// 
        /// 初始值为1，表示允许服务器推送。
        /// 除0和1以外的任何值必须作为类型为协议错误的连接错误(章节5.4.1)处理。
        ///  </code>
        /// </summary>
        EnablePush = 0x02,

        /// <summary>
        /// <code>
        /// 指示发送方允许的并发流的最大数量。这个极限是定向的:
        /// 它适用于发送方允许接收方创建的流的数量。
        /// 最初，这个值没有限制。建议该值不小于100。
        /// 这样就不会不必要地限制并行性。
        /// 
        /// SETTINGS_MAX_CONCURRENT_STREAMS的0值不应该被端点视为特殊值。
        /// 零值确实会阻止新流的创建;
        /// 然而，对于活动流耗尽的任何限制也可能发生这种情况。
        /// 服务器应该只在短时间内设置零值;如果服务器不希望接受请求，
        /// 关闭连接更合适。
        ///  </code>
        /// </summary>
        MaxConcurrentStreams = 0x03,

        /// <summary>
        /// <code>
        /// 指示发送端用于流级流控制的初始窗口大小(以字节为单位)。
        /// 初始值是2^16-1(65,535)个字节。
        ///
        /// 这个设置会影响所有流的窗口大小(参见 Section 6.9.2)。
        ///
        /// 超过最大流量控制窗口大小2^31-1的值必须作为类型为FLOW_CONTROL_ERROR的连接错误(章节5.4.1)处理。
        ///  </code>
        /// </summary>
        InitialWindowSize = 0x04,

        /// <summary>
        /// <code>
        /// 指示发送方愿意接收的最大帧有效负载的大小，以八字节为单位。
        ///
        /// 初始值是2^14(16,384)个字节。
        /// 端点发布的值必须在这个初始值和允许的最大帧大小(2^24-1或16,777,215字节)之间，
        /// 包括超出这个范围的值必须作为类型为协议错误的连接错误(章节5.4.1)处理。
        ///  </code>
        /// </summary>
        MaxFrameSize = 0x05,

        /// <summary>
        /// <code>
        /// 此通知设置以字节为单位通知对等体发送方准备接受的报头列表的最大大小。
        /// 该值基于头字段的未压缩大小，
        /// 包括以字节为单位的名称和值的长度，加上每个报头字段32个字节的开销。
        ///
        /// 对于任何给定的请求，可能会强制执行比所宣传的更低的限制。此设置的初始值是无限的。
        /// </code>
        /// </summary>
        MaxHeaderListSize = 0x06,

        Reserved = 0x07,

        /// <summary>
        /// <code>
        /// https://tools.ietf.org/html/rfc8441
        ///  当接收到值为1的SETTINGS_ENABLE_CONNECT_PROTOCOL时，客户端可以在创建新流时使用本文档中定义的扩展连接。
        ///  服务器接收此参数不会产生任何影响。
        ///  
        ///  发送方绝对不能在之前发送值为1后，再发送值为0的SETTINGS_ENABLE_CONNECT_PROTOCOL参数。
        /// </code>
        /// </summary>
        EnableConnectProtocol = 0x08
    }

    public sealed class Http2SettingsRegistry
    {
        private readonly bool[] _changeFlags;
        private readonly Http2SettingsManager _parent;

        private UInt32[] _values;
        public Action<Http2SettingsRegistry, Http2Settings, UInt32, UInt32> OnSettingChangedEvent;

        public Http2SettingsRegistry(Http2SettingsManager parent, bool readOnly, bool treatItAsAlreadyChanged)
        {
            this._parent = parent;

            this._values = new UInt32[Http2SettingsManager.SettingsCount];

            this.IsReadOnly = readOnly;
            if (!this.IsReadOnly)
                this._changeFlags = new bool[Http2SettingsManager.SettingsCount];

            // Set default values (https://httpwg.org/specs/rfc7540.html#iana-settings)
            this._values[(UInt16)Http2Settings.HeaderTableSize] = 4096;
            this._values[(UInt16)Http2Settings.EnablePush] = 1;
            this._values[(UInt16)Http2Settings.MaxConcurrentStreams] = 128;
            this._values[(UInt16)Http2Settings.InitialWindowSize] = 65535;
            this._values[(UInt16)Http2Settings.MaxFrameSize] = 16384;
            this._values[(UInt16)Http2Settings.MaxHeaderListSize] = UInt32.MaxValue; // infinite

            if (this.IsChanged == treatItAsAlreadyChanged)
            {
                var changeFlags = this._changeFlags;
                if (changeFlags != null) changeFlags[(UInt16)Http2Settings.MaxConcurrentStreams] = true;
            }
        }

        private bool IsReadOnly { get; set; }

        public UInt32 this[Http2Settings setting]
        {
            get => this._values[(ushort)setting];

            set
            {
                if (this.IsReadOnly)
                    throw new NotSupportedException("It's a read-only one!");

                ushort idx = (ushort)setting;

                // https://httpwg.org/specs/rfc7540.html#SettingValues
                // An endpoint that receives a SETTINGS frame with any unknown or unsupported identifier MUST ignore that setting.
                if (idx == 0 || idx >= this._values.Length)
                    return;

                UInt32 oldValue = this._values[idx];
                if (oldValue != value)
                {
                    this._values[idx] = value;
                    this._changeFlags[idx] = true;
                    IsChanged = true;

                    if (this.OnSettingChangedEvent != null)
                        this.OnSettingChangedEvent(this, setting, oldValue, value);
                }
            }
        }

        public bool IsChanged { get; private set; }

        public void Merge(List<KeyValuePair<Http2Settings, UInt32>> settings)
        {
            if (settings == null)
                return;

            foreach (var t in settings)
            {
                Http2Settings setting = t.Key;
                UInt16 key = (UInt16)setting;
                UInt32 value = t.Value;

                if (key > 0 && key <= Http2SettingsManager.SettingsCount)
                {
                    UInt32 oldValue = this._values[key];
                    this._values[key] = value;

                    if (oldValue != value && this.OnSettingChangedEvent != null)
                    {
                        this.OnSettingChangedEvent(this, setting, oldValue, value);
                    }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.Log(
                        $"[HTTP2SettingsRegistry] [method:Merge(List<KeyValuePair<Http2Settings, UInt32>> settings)] Merge {setting}({key}) = {value}");
#endif
                }
            }
        }

        public void Merge(Http2SettingsRegistry from)
        {
            if (this._values != null)
                this._values = new uint[from._values.Length];

            var values = this._values;
            if (values != null)
            {
                for (int i = 0; i < values.Length; ++i)
                {
                    values[i] = from._values[i];
                }
            }
        }

        internal Http2FrameHeaderAndPayload CreateFrame()
        {
            List<KeyValuePair<Http2Settings, UInt32>> keyValuePairs =
                new List<KeyValuePair<Http2Settings, uint>>(Http2SettingsManager.SettingsCount);

            for (int i = 1; i < Http2SettingsManager.SettingsCount; ++i)
            {
                if (!this._changeFlags[i]) continue;
                keyValuePairs.Add(new KeyValuePair<Http2Settings, uint>((Http2Settings)i, this[(Http2Settings)i]));
                this._changeFlags[i] = false;
            }

            this.IsChanged = false;

            return Http2FrameHelper.CreateSettingsFrame(keyValuePairs);
        }
    }

    public sealed class Http2SettingsManager
    {
        public static readonly int SettingsCount = Enum.GetNames(typeof(Http2Settings)).Length + 1;

        public Http2SettingsManager(Http2Handler parentHandler)
        {
            this.Parent = parentHandler;

            this.MySettings = new Http2SettingsRegistry(this, readOnly: true, treatItAsAlreadyChanged: false);
            this.InitiatedMySettings = new Http2SettingsRegistry(this, readOnly: false, treatItAsAlreadyChanged: true);
            this.RemoteSettings = new Http2SettingsRegistry(this, readOnly: true, treatItAsAlreadyChanged: false);
            this.SettingsChangesSentAt = DateTime.MinValue;
        }

        /// <summary>
        /// 这是我们发送给服务器的ACKd或默认设置。
        /// </summary>
        public Http2SettingsRegistry MySettings { get; private set; }

        /// <summary>
        /// 这是可以更改的设置。会尽快发送到服务器，ACKd时复制到MySettings。
        /// </summary>
        public Http2SettingsRegistry InitiatedMySettings { get; private set; }

        /// <summary>
        /// 远端对等体设置
        /// </summary>
        public Http2SettingsRegistry RemoteSettings { get; private set; }

        private DateTime SettingsChangesSentAt { get; set; }

        public Http2Handler Parent { get; private set; }

        internal void Process(Http2FrameHeaderAndPayload frame, List<Http2FrameHeaderAndPayload> outgoingFrames)
        {
            if (frame.Type != Http2FrameTypes.Settings)
                return;

            var settingsFrame = Http2FrameHelper.ReadSettings(frame);

            if (HttpManager.Logger.Level <= Logger.Loglevels.Information)
            {
                HttpManager.Logger.Information("HTTP2SettingsManager",
                    "Processing Settings frame: " + settingsFrame.ToString(), this.Parent.Context);
            }

            if ((settingsFrame.Flags & Http2SettingsFlags.Ack) == Http2SettingsFlags.Ack)
            {
                this.MySettings.Merge(this.InitiatedMySettings);
                this.SettingsChangesSentAt = DateTime.MinValue;
            }
            else
            {
                this.RemoteSettings.Merge(settingsFrame.Settings);
                outgoingFrames.Add(Http2FrameHelper.CreateAckSettingsFrame());
            }
        }

        internal void SendChanges(List<Http2FrameHeaderAndPayload> outgoingFrames)
        {
            if (this.SettingsChangesSentAt != DateTime.MinValue &&
                DateTime.UtcNow - this.SettingsChangesSentAt >= TimeSpan.FromSeconds(10))
            {
                HttpManager.Logger.Error("HTTP2SettingsManager", "No ACK received for settings frame!",
                    this.Parent.Context);
                this.SettingsChangesSentAt = DateTime.MinValue;
            }

            //  当接收到设置了ACK标志的设置帧时，更改参数的发送方可以依赖已应用的设置。
            if (!this.InitiatedMySettings.IsChanged)
                return;

            outgoingFrames.Add(this.InitiatedMySettings.CreateFrame());
            this.SettingsChangesSentAt = DateTime.UtcNow;
        }
    }
}

#endif