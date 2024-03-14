#if !BESTHTTP_DISABLE_SIGNALR

using System;
using System.Collections.Generic;

using BestHTTP.SignalR.Messages;
using BestHTTP.SignalR.JsonEncoders;

namespace BestHTTP.SignalR.Transports
{
    public delegate void OnTransportStateChangedDelegate(TransportBase transport, TransportStates oldState, TransportStates newState);

    public abstract class TransportBase
    {
        private const int MaxRetryCount = 5;

        #region Public Properties

        /// <summary>
        /// Name of the transport.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// True if the manager has to check the last message received time and reconnect if too much time passes.
        /// </summary>
        public abstract bool SupportsKeepAlive { get; }

        /// <summary>
        /// Type of the transport. Used mainly by the manager in its BuildUri function.
        /// </summary>
        public abstract TransportTypes Type { get; }

        /// <summary>
        /// Reference to the manager.
        /// </summary>
        public IConnection Connection { get; protected set; }

        /// <summary>
        /// The current state of the transport.
        /// </summary>
        public TransportStates State
        {
            get { return _state; }
            protected set
            {
                TransportStates old = _state;
                _state = value;

                if (OnStateChanged != null)
                    OnStateChanged(this, old, _state);
            }
        }
        public TransportStates _state;

        /// <summary>
        /// Thi event called when the transport's State set to a new value.
        /// </summary>
        public event OnTransportStateChangedDelegate OnStateChanged;

        #endregion

        public TransportBase(string name, Connection connection)
        {
            this.Name = name;
            this.Connection = connection;
            this.State = TransportStates.Initial;
        }

        #region Abstract functions

        /// <summary>
        /// Start to connect to the server
        /// </summary>
        public abstract void Connect();

        /// <summary>
        /// Stop the connection
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// The transport specific implementation to send the given json string to the server.
        /// </summary>
        protected abstract void SendImpl(string json);

        /// <summary>
        /// Called when the Start request finished successfully, or after a reconnect.
        /// Manager.TransportOpened(); called from the TransportBase after this call
        /// </summary>
        protected abstract void Started();

        /// <summary>
        /// Called when the abort request finished successfully.
        /// </summary>
        protected abstract void Aborted();

        #endregion

        /// <summary>
        /// Called after a succesful connect/reconnect. The transport implementations have to call this function.
        /// </summary>
        protected void OnConnected()
        {
            if (this.State != TransportStates.Reconnecting)
            {
                // Send the Start request
                Start();
            }
            else
            {
                Connection.TransportReconnected();

                Started();

                this.State = TransportStates.Started;
            }
        }

        #region Start Request Sending

        /// <summary>
        /// Sends out the /start request to the server.
        /// </summary>
        protected void Start()
        {
            HttpManager.Logger.Information("Transport - " + this.Name, "Sending Start Request");

            this.State = TransportStates.Starting;

            if (this.Connection.Protocol > ProtocolVersions.Protocol_2_0)
            {
                var request = new HttpRequest(Connection.BuildUri(RequestTypes.Start, this), HttpMethods.Get, true, true, OnStartRequestFinished);

                request.Tag = 0;
                request.MaxRetries = 0;

                request.Timeout = Connection.NegotiationResult.ConnectionTimeout + TimeSpan.FromSeconds(10);

                Connection.PrepareRequest(request, RequestTypes.Start);

                request.Send();
            }
            else
            {
                // The transport and the signalr protocol now started
                this.State = TransportStates.Started;

                Started();

                Connection.TransportStarted();
            }
        }

        private void OnStartRequestFinished(HttpRequest req, HttpResponse resp)
        {
            switch (req.State)
            {
                case HttpRequestStates.Finished:
                    if (resp.IsSuccess)
                    {
                        HttpManager.Logger.Information("Transport - " + this.Name, "Start - Returned: " + resp.DataAsText);

                        string response = Connection.ParseResponse(resp.DataAsText);

                        if (response != "started")
                        {
                            Connection.Error(string.Format("Expected 'started' response, but '{0}' found!", response));
                            return;
                        }

                        // The transport and the signalr protocol now started
                        this.State = TransportStates.Started;

                        Started();

                        Connection.TransportStarted();

                        return;
                    }
                    else
                        HttpManager.Logger.Warning("Transport - " + this.Name, string.Format("Start - request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}",
                                                                    resp.StatusCode,
                                                                    resp.Message,
                                                                    resp.DataAsText,
                                                                    req.CurrentUri));
                    goto default;

                default:
                    HttpManager.Logger.Information("Transport - " + this.Name, "Start request state: " + req.State.ToString());

                    // The request may not reached the server. Try it again.
                    int retryCount = (int)req.Tag;
                    if (retryCount++ < MaxRetryCount)
                    {
                        req.Tag = retryCount;
                        req.Send();
                    }
                    else
                        Connection.Error("Failed to send Start request.");

                    break;
            }
        }

        #endregion

        #region Abort Implementation

        /// <summary>
        /// Will abort the transport. In SignalR 'Abort'ing is a graceful process, while 'Close'ing is a hard-abortion...
        /// </summary>
        public virtual void Abort()
        {
            if (this.State != TransportStates.Started)
                return;

            this.State = TransportStates.Closing;

            var request = new HttpRequest(Connection.BuildUri(RequestTypes.Abort, this), HttpMethods.Get, true, true, OnAbortRequestFinished);

            // Retry counter
            request.Tag = 0;
            request.MaxRetries = 0;

            Connection.PrepareRequest(request, RequestTypes.Abort);

            request.Send();
        }

        protected void AbortFinished()
        {
            this.State = TransportStates.Closed;

            Connection.TransportAborted();

            this.Aborted();
        }

        private void OnAbortRequestFinished(HttpRequest req, HttpResponse resp)
        {
            switch (req.State)
            {
                case HttpRequestStates.Finished:
                    if (resp.IsSuccess)
                    {
                        HttpManager.Logger.Information("Transport - " + this.Name, "Abort - Returned: " + resp.DataAsText);

                        if (this.State == TransportStates.Closing)
                            AbortFinished();
                    }
                    else
                    {
                        HttpManager.Logger.Warning("Transport - " + this.Name, string.Format("Abort - Handshake request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}",
                                                                    resp.StatusCode,
                                                                    resp.Message,
                                                                    resp.DataAsText,
                                                                    req.CurrentUri));

                        // try again
                        goto default;
                    }
                    break;
                default:
                    HttpManager.Logger.Information("Transport - " + this.Name, "Abort request state: " + req.State.ToString());

                    // The request may not reached the server. Try it again.
                    int retryCount = (int)req.Tag;
                    if (retryCount++ < MaxRetryCount)
                    {
                        req.Tag = retryCount;                        
                        req.Send();
                    }
                    else
                        Connection.Error("Failed to send Abort request!");

                    break;
            }
        }

        #endregion

        #region Send Implementation

        /// <summary>
        /// Sends the given json string to the wire.
        /// </summary>
        /// <param name="jsonStr"></param>
        public void Send(string jsonStr)
        {
            try
            {
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
                HttpManager.Logger.Information($"Transport - {Name}", $"Sending: {jsonStr}");
#endif
                SendImpl(jsonStr);
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                // ignored
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
                HttpManager.Logger.Exception($"Transport - {Name}", "Send", ex);
#endif
            }
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// 启动重新连接进程
        /// </summary>
        public void Reconnect()
        {
            HttpManager.Logger.Information("Transport - " + this.Name, "Reconnecting");

            Stop();

            this.State = TransportStates.Reconnecting;

            Connect();
        }

        /// <summary>
        /// When the json string is successfully parsed will return with an IServerMessage implementation.
        /// </summary>
        public static IServerMessage Parse(IJsonEncoder encoder, string json)
        {
            // Nothing to parse?
            if (string.IsNullOrEmpty(json))
            {
                HttpManager.Logger.Error("MessageFactory", "Parse - called with empty or null string!");
                return null;
            }

            // We don't have to do further decoding, if it's an empty json object, then it's a KeepAlive message from the server
            if (json.Length == 2 && json == "{}")
                return new KeepAliveMessage();

            IDictionary<string, object> msg = null;

            try
            {
                // try to decode the json message with the encoder
                msg = encoder.DecodeMessage(json);
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch(Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
                HttpManager.Logger.Exception("MessageFactory", "Parse - encoder.DecodeMessage", ex);
#endif
                return null;
            }

            if (msg == null)
            {
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
                HttpManager.Logger.Error("MessageFactory", $"Parse - Json Decode failed for json string: {json}");
#endif
                return null;
            }

            // "C" is for message id
            IServerMessage result = null;
            if (!msg.ContainsKey("C"))
            {
                // If there are no ErrorMessage in the object, then it was a success
                if (!msg.ContainsKey("E"))
                    result = new ResultMessage();
                else
                    result = new FailureMessage();
            }
            else
              result = new MultiMessage();

            try
            {
                result.Parse(msg);
            }
            catch
            {
                HttpManager.Logger.Error("MessageFactory", "Can't parse msg: " + json);
                throw;
            }

            return result;
        }

        #endregion
    }
}

#endif