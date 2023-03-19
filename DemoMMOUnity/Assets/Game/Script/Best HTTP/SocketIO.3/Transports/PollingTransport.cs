#if !BESTHTTP_DISABLE_SOCKETIO

using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;

namespace BestHTTP.SocketIO3.Transports
{
    public sealed class PollingTransport : ITransport
    {
        public TransportTypes Type => TransportTypes.Polling;

        public TransportStates State { get; private set; }
        public SocketManager Manager { get; private set; }

        public bool IsRequestInProgress => _lastRequest != null;

        public bool IsPollingInProgress => _pollRequest != null;

        /// <summary>
        /// 我们发送给服务器的最后一个POST请求。
        /// </summary>
        private HttpRequest _lastRequest;

        /// <summary>
        /// 我们发送给服务器的最后一个GET请求。
        /// </summary>
        private HttpRequest _pollRequest;

        public PollingTransport(SocketManager manager)
        {
            Manager = manager;
        }

        public void Open()
        {
            var format = "{0}?EIO={1}&transport=polling&t={2}-{3}{5}";
            if (Manager.Handshake != null)
                format += "&sid={4}";

            var sendAdditionalQueryParams = !Manager.Options.QueryParamsOnlyForHandshake ||
                                            (Manager.Options.QueryParamsOnlyForHandshake && Manager.Handshake == null);

            var request = new HttpRequest(new Uri(string.Format(format,
                    Manager.Uri,
                    Manager.ProtocolVersion,
                    SocketManager.Timestamp.ToString(),
                    Manager.RequestCounter++.ToString(),
                    Manager.Handshake != null ? Manager.Handshake.Sid : string.Empty,
                    sendAdditionalQueryParams ? Manager.Options.BuildQueryParams() : string.Empty)),
                OnRequestFinished);

#if !BESTHTTP_DISABLE_CACHING
            // Don't even try to cache it
            request.DisableCache = true;
#endif

            request.MaxRetries = 0;

            if (this.Manager.Options.httpRequestCustomizationCallback != null)
            {
                this.Manager.Options.httpRequestCustomizationCallback(this.Manager, request);
            }

            request.Send();

            State = TransportStates.Opening;
        }

        /// <summary>
        /// 关闭传输并清理资源。
        /// </summary>
        public void Close()
        {
            if (State == TransportStates.Closed)
            {
                return;
            }

            State = TransportStates.Closed;
        }

        #region Packet Sending Implementation

        private readonly
            List<OutgoingPacket> _lonelyPacketList =
                new System.Collections.Generic.List<OutgoingPacket>(1);

        public void Send(OutgoingPacket packet)
        {
            try
            {
                _lonelyPacketList.Add(packet);
                Send(_lonelyPacketList);
            }
            finally
            {
                _lonelyPacketList.Clear();
            }
        }

        public void Send(List<OutgoingPacket> packets)
        {
            if (State != TransportStates.Opening && State != TransportStates.Open)
            {
                return;
            }

            if (IsRequestInProgress)
            {
                throw new Exception("数据包仍在发送中!");
            }

            StringBuilder sb = new StringBuilder(10);
            sb.Append($"{Manager.Uri}");
            sb.Append($"?EIO={Manager.ProtocolVersion}&");
            sb.Append($"transport=polling&t={SocketManager.Timestamp.ToString()}");
            sb.Append($"-{Manager.RequestCounter++.ToString()}&");
            sb.Append($"sid={Manager.Handshake.Sid}");
            var buildQuery = Manager.Options.BuildQueryParams();
            sb.Append($"{(!Manager.Options.QueryParamsOnlyForHandshake ? buildQuery : string.Empty)}");

            _lastRequest = new HttpRequest(
                uri: new Uri(sb.ToString()),
                methodType: HttpMethods.Post,
                callback: OnRequestFinished);


#if !BESTHTTP_DISABLE_CACHING
            // 甚至不要尝试缓存它
            _lastRequest.DisableCache = true;
#endif
            EncodePackets(
                packets: packets,
                request: _lastRequest);

            this.Manager.Options.httpRequestCustomizationCallback?.Invoke(this.Manager, _lastRequest);

            _lastRequest.Send();
        }

        private
            readonly
            StringBuilder _sendBuilder = new StringBuilder();

        private void EncodePackets(IReadOnlyList<OutgoingPacket> packets, HttpRequest request)
        {
            _sendBuilder.Length = 0;

            for (var i = 0; i < packets.Count; ++i)
            {
                var packet = packets[i];

                if (packet.IsBinary)
                {
                    _sendBuilder.Append('b');
                    var baseString = Convert.ToBase64String(
                        inArray: packet.PayloadData.Data,
                        offset: packet.PayloadData.Offset,
                        length: packet.PayloadData.Count);
                    _sendBuilder.Append(baseString);
                }
                else
                {
                    _sendBuilder.Append(packet.Payload);
                }

                if (packet.Attachements != null)
                {
                    foreach (var t in packet.Attachements)
                    {
                        _sendBuilder.Append((char)0x1E);
                        _sendBuilder.Append('b');
                        var baseString = Convert.ToBase64String(t);
                        _sendBuilder.Append(baseString);
                    }
                }

                if (i < packets.Count - 1)
                {
                    _sendBuilder.Append((char)0x1E);
                }
            }

            var result = _sendBuilder.ToString();
            var length = Encoding.UTF8.GetByteCount(result);
            var buffer = BufferPool.Get(length, true);

            Encoding.UTF8.GetBytes(result, 0, result.Length, buffer, 0);

            var stream = new BufferSegmentStream();

            stream.Write(new BufferSegment(buffer, 0, length));

            request.UploadStream = stream;
            request.SetHeader("Content-Type", "text/plain; charset=UTF-8");
        }

        private void OnRequestFinished(HttpRequest req, HttpResponse resp)
        {
            // 清除LastRequest变量，这样我们就可以开始发送新的包了
            _lastRequest = null;

            if (State == TransportStates.Closed)
            {
                return;
            }

            string errorString = null;

            switch (req.State)
            {
                // 请求顺利完成。
                case HttpRequestStates.Finished:
                {
                    if (HttpManager.Logger.Level <= Logger.Loglevels.All)
                    {
                        HttpManager.Logger.Verbose("PollingTransport", "OnRequestFinished: " + resp.DataAsText,
                            this.Manager.Context);
                    }

                    if (resp.IsSuccess)
                    {
                        // 当我们发送数据时，响应是一个'ok'字符串
                        if (req.MethodType != HttpMethods.Post)
                        {
                            ParseResponse(resp);
                        }
                    }
                    else
                    {
                        errorString =
                            $"轮询-请求成功完成，但服务器发送了一个错误。状态码:{resp.StatusCode}-{resp.Message}报文:{resp.DataAsText}Uri: {req.CurrentUri}";
                    }
                }
                    break;

                // 请求结束时出现意外错误。请求的Exception属性可能包含有关错误的更多信息。
                case HttpRequestStates.Error:
                {
                    errorString = (req.Exception != null
                        ? $"{req.Exception.Message}\n{req.Exception.StackTrace}"
                        : "No Exception");
                }
                    break;

                // 由用户发起的请求中止。
                case HttpRequestStates.Aborted:
                {
                    errorString = $"Polling - Request({req.CurrentUri}) Aborted!";
                }
                    break;

                // 连接服务器超时。处理步骤
                case HttpRequestStates.ConnectionTimedOut:
                    errorString = $"Polling - Connection Timed Out! Uri: {req.CurrentUri}";
                    break;

                // 请求没有在规定的时间内完成。
                case HttpRequestStates.TimedOut:
                {
                    errorString = $"Polling - Processing the request({req.CurrentUri}) Timed Out!";
                }
                    break;
                case HttpRequestStates.Initial:
                    break;
                case HttpRequestStates.Queued:
                    break;
                case HttpRequestStates.Processing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!string.IsNullOrEmpty(errorString))
                (Manager as IManager).OnTransportError(this, errorString);
        }

        #endregion

        #region Polling Implementation

        public void Poll()
        {
            if (_pollRequest != null || State == TransportStates.Paused)
                return;
            StringBuilder sb = new StringBuilder(10);
            sb.Append($"{Manager.Uri}");
            sb.Append($"?EIO={Manager.ProtocolVersion}&");
            sb.Append($"transport=polling&t={SocketManager.Timestamp.ToString()}");
            sb.Append($"-{Manager.RequestCounter++.ToString()}&");
            sb.Append($"sid={Manager.Handshake.Sid}");
            var buildQuery = Manager.Options.BuildQueryParams();
            sb.Append($"{(!Manager.Options.QueryParamsOnlyForHandshake ? buildQuery : string.Empty)}");
            _pollRequest = new HttpRequest(
                uri: new Uri(sb.ToString()),
                methodType: HttpMethods.Get,
                callback: OnPollRequestFinished);

#if !BESTHTTP_DISABLE_CACHING
            // Don't even try to cache it
            _pollRequest.DisableCache = true;
#endif

            _pollRequest.MaxRetries = 0;

            this.Manager.Options.httpRequestCustomizationCallback?.Invoke(this.Manager, _pollRequest);

            _pollRequest.Send();
        }

        private void OnPollRequestFinished(HttpRequest req, HttpResponse resp)
        {
            /*
             * 清除PollRequest变量，这样我们就可以开始一个新的poll。
             */
            // Clear the PollRequest variable, so we can start a new poll.
            _pollRequest = null;

            if (State == TransportStates.Closed)
            {
                return;
            }

            string errorString = null;

            switch (req.State)
            {
                // 请求顺利完成。
                case HttpRequestStates.Finished:
                {
                    if (HttpManager.Logger.Level <= Logger.Loglevels.All)
                    {
                        HttpManager.Logger.Verbose("PollingTransport", "OnPollRequestFinished: " + resp.DataAsText,
                            this.Manager.Context);
                    }

                    if (resp.IsSuccess)
                    {
                        ParseResponse(resp);
                    }
                    else
                    {
                        errorString =
                            $"轮询-请求成功完成，但服务器发送了一个错误。状态码:{resp.StatusCode}-{resp.Message}报文:{resp.DataAsText}Uri: {req.CurrentUri}";
                    }
                }
                    break;

                // 请求结束时出现意外错误。请求的Exception属性可能包含有关错误的更多信息。
                case HttpRequestStates.Error:
                {
                    errorString = req.Exception != null
                        ? $"{req.Exception.Message}\n{req.Exception.StackTrace}"
                        : "No Exception";
                }
                    break;

                // 由用户发起的请求中止。
                case HttpRequestStates.Aborted:
                {
                    errorString = $"Polling - Request({req.CurrentUri}) Aborted!";
                }
                    break;

                // 连接服务器超时。处理步骤
                case HttpRequestStates.ConnectionTimedOut:
                    errorString = $"Polling - Connection Timed Out! Uri: {req.CurrentUri}";
                    break;

                // 请求没有在规定的时间内完成。
                case HttpRequestStates.TimedOut:
                {
                    errorString = $"轮询——处理请求({req.CurrentUri}) Timed Out!";
                }
                    break;
            }

            if (!string.IsNullOrEmpty(errorString))
            {
                (Manager as IManager).OnTransportError(this, errorString);
            }
        }

        /// <summary>
        /// 预处理数据包并将数据包发送给管理器。
        /// </summary>
        private void OnPacket(IncomingPacket packet)
        {
            switch (packet.TransportEvent)
            {
                case TransportEventTypes.Open:
                {
                    if (this.State != TransportStates.Opening)
                    {
                        HttpManager.Logger.Warning("PollingTransport",
                            $"当状态为{State.ToString()}时收到“打开”数据包", this.Manager.Context);
                        
                    }
                    else
                    {
                        State = TransportStates.Open;
                    }

                    goto default;
                }

                case TransportEventTypes.Message:
                {
                    if (packet.SocketIOEvent == SocketIOEventTypes.Connect) //2:40
                    {
                        this.State = TransportStates.Open;
                    }

                    goto default;
                }

                default:
                {
                    (Manager as IManager).OnPacket(packet);
                }
                    break;
            }
        }

        private void ParseResponse(HttpResponse resp)
        {
            try
            {
                if (resp?.Data == null || resp.Data.Length < 1)
                {
                    return;
                }

                int idx = 0;
                while (idx < resp.Data.Length)
                {
                    int endIdx = FindNextRecordSeparator(resp.Data, idx);
                    int length = endIdx - idx;

                    if (length <= 0)
                    {
                        break;
                    }

                    var packet = IncomingPacket.Empty;

                    if (resp.Data[idx] == 'b')
                    {
                        // 第一个字节是二进制指示符('b')。我们必须跳过它，所以我们提前idx，同时也要减少长度
                        idx++;
                        length--;
                        var base64Encoded = Encoding.UTF8.GetString(resp.Data, idx, length);
                        var byteData = Convert.FromBase64String(base64Encoded);
                        var bufferData = new BufferSegment(
                            data: byteData,
                            offset: 0,
                            count: byteData.Length);
                        packet = this.Manager.Parser.Parse(
                            manager: this.Manager,
                            data: bufferData);
                    }
                    else
                    {
                        // It's the handshake data?
                        if (this.State == TransportStates.Opening)
                        {
                            var transportEvent = (TransportEventTypes)(resp.Data[idx] - '0');
                            if (transportEvent == TransportEventTypes.Open)
                            {
                                var utfGetString = Encoding.UTF8.GetString(
                                    bytes: resp.Data,
                                    index: idx + 1,
                                    count: length - 1);
                                var handshake = JSON.LitJson.JsonMapper.ToObject<HandshakeData>(utfGetString);
                                packet = new IncomingPacket(
                                    transportEvent: TransportEventTypes.Open,
                                    packetType: SocketIOEventTypes.Unknown,
                                    nsp: "/",
                                    id: -1)
                                {
                                    DecodedArg = handshake
                                };
                            }
                            else
                            {
                                // TODO: error?
                            }
                        }
                        else
                        {
                            var utfGetString = Encoding.UTF8.GetString(
                                bytes: resp.Data,
                                index: idx,
                                count: length);
                            packet = this.Manager.Parser.Parse(
                                manager: this.Manager,
                                data: utfGetString);
                        }
                    }

                    if (!packet.Equals(IncomingPacket.Empty))
                    {
                        try
                        {
                            OnPacket(packet);
                        }
                        catch (Exception ex)
                        {
                            HttpManager.Logger.Exception("PollingTransport", "ParseResponse - OnPacket", ex,
                                this.Manager.Context);
                            (Manager as IManager).EmitError(ex.Message + " " + ex.StackTrace);
                        }
                    }

                    idx = endIdx + 1;
                }
            }
            catch (Exception ex)
            {
                (Manager as IManager).EmitError(ex.Message + " " + ex.StackTrace);

                HttpManager.Logger.Exception("PollingTransport", "ParseResponse", ex, this.Manager.Context);
            }
        }

        private int FindNextRecordSeparator(IReadOnlyList<byte> data, int startIdx)
        {
            for (var i = startIdx; i < data.Count; ++i)
            {
                if (data[i] == 0x1E)
                {
                    return i;
                }
            }

            return data.Count;
        }

        #endregion
    }
}

#endif