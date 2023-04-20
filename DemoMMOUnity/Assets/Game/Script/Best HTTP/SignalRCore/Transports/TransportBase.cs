#if !BESTHTTP_DISABLE_SIGNALR_CORE
using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.Memory;

namespace BestHTTP.SignalRCore.Transports
{
    internal abstract class TransportBase : ITransport
    {
        public abstract TransportTypes TransportType { get; }

        public TransferModes TransferMode => TransferModes.Binary;

        /// <summary>
        /// 传输的当前状态。所有更改都将通过onstatechange事件传播到HubConnection。
        /// </summary>
        public TransportStates State
        {
            get => this._transportStates;
            protected set
            {
                if (this._transportStates != value)
                {
                    TransportStates oldState = this._transportStates;
                    this._transportStates = value;

                    if (this.OnStateChanged != null)
                        this.OnStateChanged(oldState, this._transportStates);
                }
            }
        }

        private TransportStates _transportStates;

        /// <summary>
        /// 这将存储失败的原因，以便HubConnection可以将其包含在其OnError事件中。
        /// </summary>
        public string ErrorReason { get; protected set; }

        /// <summary>
        /// 每次传输状态改变的时候都会调用
        /// </summary>
        public event Action<TransportStates, TransportStates> OnStateChanged;

        protected LoggingContext Context { get; set; }

        /// <summary>
        /// 已解析消息的缓存列表。
        /// </summary>
        protected List<Messages.Message> Messages = new List<Messages.Message>();

        /// <summary>
        /// 父HubConnection实例。
        /// </summary>
        protected readonly HubConnection Connection;

        internal TransportBase(HubConnection con)
        {
            this.Connection = con;
            this.Context = new LoggingContext(this);
            this.Context.Add("Hub", this.Connection.Context);
            this.State = TransportStates.Initial;
        }

        /// <summary>
        /// ITransport.StartConnect
        /// </summary>
        public abstract void StartConnect();

        /// <summary>
        /// ITransport.Send
        /// </summary>
        /// <param name="msg"></param>
        public abstract void Send(BufferSegment msg);

        /// <summary>
        /// ITransport.StartClose
        /// </summary>
        public abstract void StartClose();

        private string ParseHandshakeResponse(string data)
        {
            // The handshake response is
            //  -an empty json object ('{}') if the handshake process is successFull
            //  -otherwise it has one 'error' field

            Dictionary<string, object> response = JSON.Json.Decode(data) as Dictionary<string, object>;

            if (response == null)
            {
                return $"Couldn't parse json data: {data}";
            }

            return response.TryGetValue("error", out var error) ? error.ToString() : null;
        }

        protected void HandleHandshakeResponse(string data)
        {
            this.ErrorReason = ParseHandshakeResponse(data);

            this.State = string.IsNullOrEmpty(this.ErrorReason) ? TransportStates.Connected : TransportStates.Failed;
        }

        readonly StringBuilder _queryBuilder = new StringBuilder(3);

        protected Uri BuildUri(Uri baseUri)
        {
            if (this.Connection.NegotiationResult == null)
                return baseUri;

            UriBuilder builder = new UriBuilder(baseUri);

            _queryBuilder.Length = 0;

            _queryBuilder.Append(baseUri.Query);
            if (!string.IsNullOrEmpty(this.Connection.NegotiationResult.ConnectionToken))
            {
                _queryBuilder.Append("&id=").Append(this.Connection.NegotiationResult.ConnectionToken);
            }
            else if (!string.IsNullOrEmpty(this.Connection.NegotiationResult.ConnectionId))
            {
                _queryBuilder.Append("&id=").Append(this.Connection.NegotiationResult.ConnectionId);
            }

            builder.Query = _queryBuilder.ToString();

            if (builder.Query.StartsWith("??"))
            {
                builder.Query = builder.Query.Substring(2);
            }

            return builder.Uri;
        }
    }
}
#endif