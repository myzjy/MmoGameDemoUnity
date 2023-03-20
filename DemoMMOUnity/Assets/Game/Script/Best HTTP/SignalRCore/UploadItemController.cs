#if !BESTHTTP_DISABLE_SIGNALR_CORE
using System;
using BestHTTP.Futures;
using BestHTTP.SignalRCore.Messages;

namespace BestHTTP.SignalRCore
{
    // ReSharper disable once IdentifierTypo
    // ReSharper disable once UnusedTypeParameter
    public interface IUPoladItemController<TResult> : IDisposable
    {
        string[] StreamingIDs { get; }
        HubConnection Hub { get; }

        void UploadParam<T>(string streamId, T item);
        void Cancel();
    }

    public sealed class DownStreamItemController<TResult> : IFuture<TResult>, IDisposable
    {
        private readonly long _invocationId;
        private readonly HubConnection _hubConnection;
        private readonly IFuture<TResult> _future;

        public FutureState state => this._future.state;
        public TResult value => this._future.value;
        public Exception error => this._future.error;

        public bool IsCanceled { get; private set; }

        public DownStreamItemController(HubConnection hub, long iId, IFuture<TResult> future)
        {
            this._hubConnection = hub;
            this._invocationId = iId;
            this._future = future;
        }

        private void Cancel()
        {
            if (this.IsCanceled)
                return;

            this.IsCanceled = true;

            Message message = new Message
            {
                type = MessageTypes.CancelInvocation,
                invocationId = this._invocationId.ToString()
            };

            this._hubConnection.SendMessage(message);
        }

        public void Dispose()
        {
            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            GC.SuppressFinalize(this);
            Cancel();
        }

        public IFuture<TResult> OnItem(FutureValueCallback<TResult> callback)
        {
            return this._future.OnItem(callback);
        }

        public IFuture<TResult> OnSuccess(FutureValueCallback<TResult> callback)
        {
            return this._future.OnSuccess(callback);
        }

        public IFuture<TResult> OnError(FutureErrorCallback callback)
        {
            return this._future.OnError(callback);
        }

        public IFuture<TResult> OnComplete(FutureCallback<TResult> callback)
        {
            return this._future.OnComplete(callback);
        }
    }

    public sealed class UpStreamItemController<TResult> : IUPoladItemController<TResult>, IFuture<TResult>
    {
        private readonly long _invocationId;
        public readonly string[] StreamingIds;
        private readonly HubConnection _hubConnection;
        private readonly IFuture<TResult> _future;

        public string[] StreamingIDs => this.StreamingIds;

        public HubConnection Hub => this._hubConnection;

        public FutureState state => this._future.state;
        public TResult value => this._future.value;
        public Exception error => this._future.error;

        private bool IsFinished { get; set; }

        public bool IsCanceled { get; private set; }

        private readonly object[] _streams;

        public UpStreamItemController(HubConnection hub, long iId, string[] sIds, IFuture<TResult> future)
        {
            this._hubConnection = hub;
            this._invocationId = iId;
            this.StreamingIds = sIds;
            this._streams = new object[this.StreamingIds.Length];
            this._future = future;
        }

        public UploadChannel<TResult, T> GetUploadChannel<T>(int paramIdx)
        {
            var stream = this._streams[paramIdx] as UploadChannel<TResult, T>;
            if (stream == null)
                this._streams[paramIdx] = stream = new UploadChannel<TResult, T>(this, paramIdx);

            return stream;
        }

        public void UploadParam<T>(string streamId, T item)
        {
            if (streamId == null)
                return;

            var message = new Message
            {
                type = MessageTypes.StreamItem,
                invocationId = streamId,
                item = item,
            };

            this._hubConnection.SendMessage(message);
        }

        private void Finish()
        {
            if (!this.IsFinished)
            {
                this.IsFinished = true;

                foreach (var t in this.StreamingIds)
                {
                    if (t != null)
                    {
                        var message = new Message
                        {
                            type = MessageTypes.Completion,
                            invocationId = t
                        };

                        this._hubConnection.SendMessage(message);
                    }
                }
            }
        }

        public void Cancel()
        {
            if (!this.IsFinished && !this.IsCanceled)
            {
                this.IsCanceled = true;

                var message = new Message
                {
                    type = MessageTypes.CancelInvocation,
                    invocationId = this._invocationId.ToString(),
                };

                this._hubConnection.SendMessage(message);

                // Zero out the streaming ids, disabling any future message sending
                Array.Clear(this.StreamingIds, 0, this.StreamingIds.Length);

                // If it's also a down-stream, set it canceled.
                if (this._future.value is StreamItemContainer<TResult> itemContainer)
                    itemContainer.IsCanceled = true;
            }
        }

        void IDisposable.Dispose()
        {
            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            GC.SuppressFinalize(this);

            Finish();
        }

        public IFuture<TResult> OnItem(FutureValueCallback<TResult> callback)
        {
            return this._future.OnItem(callback);
        }

        public IFuture<TResult> OnSuccess(FutureValueCallback<TResult> callback)
        {
            return this._future.OnSuccess(callback);
        }

        public IFuture<TResult> OnError(FutureErrorCallback callback)
        {
            return this._future.OnError(callback);
        }

        public IFuture<TResult> OnComplete(FutureCallback<TResult> callback)
        {
            return this._future.OnComplete(callback);
        }
    }

    /// <summary>
    /// ReSharper disable once CommentTypo
    /// An upload channel that represents one prameter of a client callable function. It implements the IDisposable
    /// interface and calls Finish from the Dispose method.
    /// </summary>
    public sealed class UploadChannel<TResult, T> : IDisposable
    {
        /// <summary>
        /// The associated upload controller
        /// </summary>
        private IUPoladItemController<TResult> Controller { get; set; }

        /// <summary>
        /// What parameter is bound to.
        /// </summary>
        private int ParamIdx { get; set; }

        /// <summary>
        /// Returns true if Finish() or Cancel() is already called.
        /// </summary>
        private bool IsFinished
        {
            get => this.Controller.StreamingIDs[this.ParamIdx] == null;
            set
            {
                if (value)
                    this.Controller.StreamingIDs[this.ParamIdx] = null;
            }
        }

        /// <summary>
        /// The unique generated id of this parameter channel.
        /// </summary>
        private string StreamingId => this.Controller.StreamingIDs[this.ParamIdx];

        internal UploadChannel(IUPoladItemController<TResult> ctrl, int paramIdx)
        {
            this.Controller = ctrl;
            this.ParamIdx = paramIdx;
        }

        /// <summary>
        /// Uploads a parameter value to the server.
        /// </summary>
        public void Upload(T item)
        {
            string streamId = this.StreamingId;
            if (streamId != null)
                this.Controller.UploadParam(streamId, item);
        }

        /// <summary>
        /// Calling this function cancels the call itself, not just a parameter upload channel.
        /// </summary>
        public void Cancel()
        {
            if (!this.IsFinished)
            {
                // Cancel all upload stream, cancel will also set streaming ids to 0.
                this.Controller.Cancel();
            }
        }

        /// <summary>
        /// ReSharper disable once CommentTypo
        /// Finishes the channel by telling the server that no more uplode items will follow.
        /// </summary>
        private void Finish()
        {
            if (!this.IsFinished)
            {
                string streamId = this.StreamingId;
                if (streamId != null)
                {
                    // this will set the streaming id to 0
                    this.IsFinished = true;

                    var message = new Message
                    {
                        type = MessageTypes.Completion,
                        invocationId = streamId
                    };

                    this.Controller.Hub.SendMessage(message);
                }
            }
        }

        void IDisposable.Dispose()
        {
            if (!this.IsFinished)
                Finish();
            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            GC.SuppressFinalize(this);
        }
    }
}
#endif