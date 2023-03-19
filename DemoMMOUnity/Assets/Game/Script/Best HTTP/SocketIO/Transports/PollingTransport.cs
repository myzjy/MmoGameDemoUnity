#if !BESTHTTP_DISABLE_SOCKETIO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BestHTTP.Extensions;

namespace BestHTTP.SocketIO.Transports
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

        /// <summary>
        /// 带有预期二进制附件的最后一个包
        /// </summary>
        private Packet _packetWithAttachment;

        private enum PayloadTypes : byte
        {
            Text,
            Binary
        }

        public PollingTransport(SocketManager manager)
        {
            Manager = manager;
        }

        public void Open()
        {
            string format = "{0}?EIO={1}&transport=polling&t={2}-{3}{5}";
            if (Manager.Handshake != null)
            {
                format += "&sid={4}";
            }

            bool sendAdditionalQueryParams = !Manager.Options.QueryParamsOnlyForHandshake ||
                                             (Manager.Options.QueryParamsOnlyForHandshake &&
                                              Manager.Handshake == null);

            HttpRequest request = new HttpRequest(new Uri(string.Format(format,
                    Manager.Uri,
                    Manager.ProtocolVersion,
                    Manager.Timestamp.ToString(),
                    Manager.RequestCounter++.ToString(),
                    Manager.Handshake != null ? Manager.Handshake.Sid : string.Empty,
                    sendAdditionalQueryParams ? Manager.Options.BuildQueryParams() : string.Empty)),
                OnRequestFinished);

#if !BESTHTTP_DISABLE_CACHING
            // Don't even try to cache it
            request.DisableCache = true;
#endif

            request.MaxRetries = 0;

            this.Manager.Options.HTTPRequestCustomizationCallback?.Invoke(
                manager: this.Manager,
                request: request);

            request.Send();

            State = TransportStates.Opening;
        }

        /// <summary>
        /// 关闭传输并清理资源。
        /// </summary>
        public void Close()
        {
            if (State == TransportStates.Closed)
                return;

            State = TransportStates.Closed;

            /*
            if (LastRequest != null)
                LastRequest.Abort();

            if (PollRequest != null)
                PollRequest.Abort();*/
        }

        private readonly List<Packet> _lonelyPacketList =
            new System.Collections.Generic.List<Packet>(1);

        public void Send(Packet packet)
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

        public void Send(List<Packet> packets)
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
            sb.Append($"transport=polling&t={Manager.Timestamp.ToString()}");
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

            if (this.Manager.Options.ServerVersion == SupportedSocketIOVersions.v2)
            {
                SendV2(packets, _lastRequest);
            }
            else
            {
                SendV3(packets, _lastRequest);
            }

            if (this.Manager.Options.HTTPRequestCustomizationCallback != null)
            {
                this.Manager.Options.HTTPRequestCustomizationCallback(
                    manager: this.Manager,
                    request: _lastRequest);
            }

            _lastRequest.Send();
        }

        private
            readonly
            StringBuilder _sendBuilder = new StringBuilder();

        private void SendV3(IList<Packet> packets, HttpRequest request)
        {
            _sendBuilder.Length = 0;

            try
            {
                for (var i = 0; i < packets.Count; ++i)
                {
                    var packet = packets[i];

                    if (i > 0)
                        _sendBuilder.Append((char)0x1E);
                    _sendBuilder.Append(packet.Encode());

                    if (packet.Attachments is not { Count: > 0 }) continue;
                    foreach (var _ in packet.Attachments)
                    {
                        _sendBuilder.Append((char)0x1E);
                        _sendBuilder.Append('b');
                        _sendBuilder.Append(Convert.ToBase64String(packet.Attachments[i]));
                    }
                }

                packets.Clear();
            }
            catch (Exception ex)
            {
                (Manager as IManager).EmitError(SocketIOErrors.Internal, $"{ex.Message} {ex.StackTrace}");
                return;
            }

            var str = _sendBuilder.ToString();
            request.RawData = Encoding.UTF8.GetBytes(str);
            request.SetHeader("Content-Type", "text/plain; charset=UTF-8");
        }

        private void SendV2(IList<Packet> packets, HttpRequest request)
        {
            byte[] buffer;

            try
            {
                buffer = packets[0].EncodeBinary();

                for (int i = 1; i < packets.Count; ++i)
                {
                    byte[] tmpBuffer = packets[i].EncodeBinary();

                    Array.Resize(ref buffer, buffer.Length + tmpBuffer.Length);

                    Array.Copy(tmpBuffer, 0, buffer, buffer.Length - tmpBuffer.Length, tmpBuffer.Length);
                }

                packets.Clear();
            }
            catch (Exception ex)
            {
                (Manager as IManager).EmitError(SocketIOErrors.Internal, $"{ex.Message} {ex.StackTrace}");
                return;
            }

            request.SetHeader("Content-Type", "application/octet-stream");
            request.RawData = buffer;
        }

        private void OnRequestFinished(HttpRequest req, HttpResponse resp)
        {
            // 清除LastRequest变量，这样我们就可以开始发送新的包了
            _lastRequest = null;

            if (State == TransportStates.Closed)
                return;

            string errorString = null;

            switch (req.State)
            {
                // 请求顺利完成。
                case HttpRequestStates.Finished:
                {
                    if (HttpManager.Logger.Level <= Logger.Loglevels.All)
                        HttpManager.Logger.Verbose("PollingTransport", "OnRequestFinished: " + resp.DataAsText);

                    if (resp.IsSuccess)
                    {
                        // When we are sending data, the response is an 'ok' string
                        if (req.MethodType != HttpMethods.Post)
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

                //  连接服务器超时。处理步骤
                case HttpRequestStates.ConnectionTimedOut:
                    errorString = $"Polling - Connection Timed Out! Uri: {req.CurrentUri}";
                    break;

                // 请求没有在规定的时间内完成。
                case HttpRequestStates.TimedOut:
                    errorString = $"Polling - Processing the request({req.CurrentUri}) Timed Out!";
                    break;
            }

            if (!string.IsNullOrEmpty(errorString))
                (Manager as IManager).OnTransportError(this, errorString);
        }

        public void Poll()
        {
            if (_pollRequest != null || State == TransportStates.Paused)
                return;

            StringBuilder sb = new StringBuilder(10);
            sb.Append($"{Manager.Uri}");
            sb.Append($"?EIO={Manager.ProtocolVersion}&");
            sb.Append($"transport=polling&t={Manager.Timestamp.ToString()}");
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

            if (this.Manager.Options.HTTPRequestCustomizationCallback != null)
                this.Manager.Options.HTTPRequestCustomizationCallback(this.Manager, _pollRequest);

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
                        HttpManager.Logger.Verbose("PollingTransport", "OnPollRequestFinished: " + resp.DataAsText);
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
                    errorString = $"Polling - Request({req.CurrentUri}) Aborted!";
                    break;

                // 连接服务器超时。处理步骤
                case HttpRequestStates.ConnectionTimedOut:
                    errorString = $"Polling - Connection Timed Out! Uri: {req.CurrentUri}";
                    break;

                // 请求没有在规定的时间内完成。
                case HttpRequestStates.TimedOut:
                    errorString = $"Polling - Processing the request({req.CurrentUri}) Timed Out!";
                    break;
            }

            if (!string.IsNullOrEmpty(errorString))
                (Manager as IManager).OnTransportError(this, errorString);
        }


        /// <summary>
        /// 预处理数据包并将数据包发送给管理器。
        /// </summary>
        private void OnPacket(Packet packet)
        {
            if (packet.AttachmentCount != 0 && !packet.HasAllAttachment)
            {
                _packetWithAttachment = packet;
                return;
            }

            switch (packet.TransportEvent)
            {
                case TransportEventTypes.Open:
                {
                    if (this.State != TransportStates.Opening)
                    {
                        HttpManager.Logger.Warning("PollingTransport",
                            $"当状态为{State.ToString()}时收到“打开”数据包");
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

                case TransportEventTypes.Unknown:
                case TransportEventTypes.Close:
                case TransportEventTypes.Ping:
                case TransportEventTypes.Pong:
                case TransportEventTypes.Upgrade:
                case TransportEventTypes.Noop:
                default:
                {
                    (Manager as IManager).OnPacket(packet);
                }
                    break;
            }
        }

        private SupportedSocketIOVersions GetServerVersion(HttpResponse resp)
        {
            string contentTypeValue = resp.GetFirstHeaderValue("content-type");
            if (string.IsNullOrEmpty(contentTypeValue))
            {
                return SupportedSocketIOVersions.v2;
            }

            HeaderParser contentType = new HeaderParser(contentTypeValue);
            var type = contentType.Values.FirstOrDefault()?.Key == "text/plain"
                ? PayloadTypes.Text
                : PayloadTypes.Binary;

            if (type != PayloadTypes.Text)
                return SupportedSocketIOVersions.v2;

            // https://github.com/socketio/engine.io-protocol/issues/35
            // ReSharper disable once CommentTypo
            // v3: 96:0{ "sid":"lv_VI97HAXpY6yYWAAAC","upgrades":["websocket"],"pingInterval":25000,"pingTimeout":5000}
            // ReSharper disable once CommentTypo
            // v4:    0{ "sid":"lv_VI97HAXpY6yYWAAAC","upgrades":["websocket"],"pingInterval":25000,"pingTimeout":5000}
            foreach (var t in resp.Data)
            {
                if (t == ':')
                {
                    return SupportedSocketIOVersions.v2;
                }

                if (t == '{')
                {
                    return SupportedSocketIOVersions.v3;
                }
            }

            return SupportedSocketIOVersions.Unknown;
        }

        private void ParseResponse(HttpResponse resp)
        {
            if (this.Manager.Options.ServerVersion == SupportedSocketIOVersions.Unknown)
            {
                this.Manager.Options.ServerVersion = GetServerVersion(resp);
            }

            if (this.Manager.Options.ServerVersion == SupportedSocketIOVersions.v2)
            {
                this.ParseResponseV2(resp);
            }
            else
            {
                this.ParseResponseV3(resp);
            }
        }

        private void ParseResponseV3(HttpResponse resp)
        {
            try
            {
                if (resp?.Data == null || resp.Data.Length < 1)
                {
                    return;
                }

                //HeaderParser contentType = new HeaderParser(resp.GetFirstHeaderValue("content-type"));
                //PayloadTypes type = contentType.Values.FirstOrDefault().Key == "text/plain" ? PayloadTypes.Text : PayloadTypes.Binary;

                int idx = 0;
                while (idx < resp.Data.Length)
                {
                    int endIdx = FindNextRecordSeparator(resp.Data, idx);
                    int length = endIdx - idx;

                    if (length <= 0)
                        break;

                    Packet packet = null;

                    if (resp.Data[idx] == 'b')
                    {
                        if (_packetWithAttachment != null)
                        {
                            // 第一个字节是二进制指示符('b')。我们必须跳过它，所以我们提前idx，同时也要减少长度
                            idx++;
                            length--;

                            var base64Encoded = Encoding.UTF8.GetString(
                                bytes: resp.Data,
                                index: idx,
                                count: length);
                            var fromBaseString = Convert.FromBase64String(base64Encoded);
                            _packetWithAttachment.AddAttachmentFromServer(fromBaseString, true);

                            if (_packetWithAttachment.HasAllAttachment)
                            {
                                packet = _packetWithAttachment;
                                _packetWithAttachment = null;
                            }
                        }
                        else
                            HttpManager.Logger.Warning("PollingTransport",
                                "已收到二进制文件，但没有包附加!");
                    }
                    else
                    {
                        packet = new Packet(Encoding.UTF8.GetString(resp.Data, idx, length));
                    }

                    if (packet != null)
                    {
                        try
                        {
                            OnPacket(packet);
                        }
                        catch (Exception ex)
                        {
                            HttpManager.Logger.Exception("PollingTransport", "ParseResponseV3 - OnPacket", ex);
                            (Manager as IManager).EmitError(
                                errCode: SocketIOErrors.Internal,
                                msg: $"{ex.Message} {ex.StackTrace}");
                        }
                    }

                    idx = endIdx + 1;
                }
            }
            catch (Exception ex)
            {
                (Manager as IManager).EmitError(
                    errCode: SocketIOErrors.Internal,
                    msg: $"{ex.Message} {ex.StackTrace}");

                HttpManager.Logger.Exception("PollingTransport", "ParseResponseV3", ex);
            }
        }

        private int FindNextRecordSeparator(byte[] data, int startIdx)
        {
            for (int i = startIdx; i < data.Length; ++i)
            {
                if (data[i] == 0x1E)
                    return i;
            }

            return data.Length;
        }

        /// <summary>
        /// 将解析响应，并发送解析后的包。
        /// </summary>
        private void ParseResponseV2(HttpResponse resp)
        {
            try
            {
                if (resp is { Data: { Length: >= 1 } })
                {
                    int idx = 0;

                    while (idx < resp.Data.Length)
                    {
                        PayloadTypes type = PayloadTypes.Text;
                        int length = 0;

                        if (resp.Data[idx] < '0')
                        {
                            type = (PayloadTypes)resp.Data[idx++];

                            byte num = resp.Data[idx++];
                            while (num != 0xFF)
                            {
                                length = (length * 10) + num;
                                num = resp.Data[idx++];
                            }
                        }
                        else
                        {
                            byte next = resp.Data[idx++];
                            while (next != ':')
                            {
                                length = (length * 10) + (next - '0');
                                next = resp.Data[idx++];
                            }

                            // 因为长度可以不同于字节长度，所以我们必须做一些后期处理来支持unicode字符。
                            int brackets = 0;
                            int tmpIdx = idx;
                            while (tmpIdx < idx + length)
                            {
                                if (resp.Data[tmpIdx] == '[')
                                {
                                    brackets++;
                                }
                                else if (resp.Data[tmpIdx] == ']')
                                {
                                    brackets--;
                                }

                                tmpIdx++;
                            }

                            if (brackets > 0)
                            {
                                while (brackets > 0)
                                {
                                    if (resp.Data[tmpIdx] == '[')
                                    {
                                        brackets++;
                                    }
                                    else if (resp.Data[tmpIdx] == ']')
                                    {
                                        brackets--;
                                    }

                                    tmpIdx++;
                                }

                                length = tmpIdx - idx;
                            }
                        }

                        Packet packet = null;
                        switch (type)
                        {
                            case PayloadTypes.Text:
                            {
                                var utfString = Encoding.UTF8.GetString(
                                    bytes: resp.Data,
                                    index: idx,
                                    count: length);
                                packet = new Packet(utfString);
                            }
                                break;
                            case PayloadTypes.Binary:
                            {
                                if (_packetWithAttachment != null)
                                {
                                    // 第一个字节是数据包类型。我们可以跳过它，所以我们提前idx我们也要减少长度
                                    idx++;
                                    length--;

                                    byte[] buffer = new byte[length];
                                    Array.Copy(
                                        sourceArray: resp.Data,
                                        sourceIndex: idx,
                                        destinationArray: buffer,
                                        destinationIndex: 0,
                                        length: length);

                                    _packetWithAttachment.AddAttachmentFromServer(buffer, true);

                                    if (_packetWithAttachment.HasAllAttachment)
                                    {
                                        packet = _packetWithAttachment;
                                        _packetWithAttachment = null;
                                    }
                                }
                            }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        } // switch

                        if (packet != null)
                        {
                            try
                            {
                                OnPacket(packet);
                            }
                            catch (Exception ex)
                            {
                                HttpManager.Logger.Exception("PollingTransport", "ParseResponseV2 - OnPacket", ex);
                                (Manager as IManager).EmitError(
                                    errCode: SocketIOErrors.Internal,
                                    msg: $"{ex.Message} {ex.StackTrace}");
                            }
                        }

                        idx += length;
                    } // while
                }
            }
            catch (Exception ex)
            {
                (Manager as IManager).EmitError(
                    errCode: SocketIOErrors.Internal,
                    msg: $"{ex.Message} {ex.StackTrace}");

                HttpManager.Logger.Exception("PollingTransport", "ParseResponseV2", ex);
            }
        }
    }
}

#endif