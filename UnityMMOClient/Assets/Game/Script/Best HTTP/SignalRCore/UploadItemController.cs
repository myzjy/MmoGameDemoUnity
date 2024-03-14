#if !BESTHTTP_DISABLE_SIGNALR_CORE
using System;
using BestHTTP.Futures;
using BestHTTP.SignalRCore.Messages;

namespace BestHTTP.SignalRCore
{
    public interface IUploadItemController : IDisposable
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
            {
                return;
            }

            this.IsCanceled = true;

            var message = new Message
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

    public sealed class UpStreamItemController<TResult> : IUploadItemController, IFuture<TResult>
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
            if (this._streams[paramIdx] is not UploadChannel<TResult, T> stream)
            {
                this._streams[paramIdx] = stream = new UploadChannel<TResult, T>(this, paramIdx);
            }

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
            if (this.IsFinished) return;
            this.IsFinished = true;

            foreach (var t in this.StreamingIds)
            {
                if (t == null) continue;
                var message = new Message
                {
                    type = MessageTypes.Completion,
                    invocationId = t
                };

                this._hubConnection.SendMessage(message);
            }
        }

        public void Cancel()
        {
            if (this.IsFinished || this.IsCanceled) return;
            this.IsCanceled = true;

            var message = new Message
            {
                type = MessageTypes.CancelInvocation,
                invocationId = this._invocationId.ToString(),
            };

            this._hubConnection.SendMessage(message);

            // 将流id归零，禁用任何未来的消息发送
            Array.Clear(
                array: this.StreamingIds,
                index: 0,
                length: this.StreamingIds.Length);

            // 如果是下游，就取消。
            if (this._future.value is
                StreamItemContainer<TResult> itemContainer)
            {
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
    /// ReSharper禁用一次CommentTypo
    ///一个上传通道，表示客户端可调用函数的一个参数。它实现了IDisposable
    ///从Dispose方法调用Finish。
    /// </summary>
    public sealed class UploadChannel<TResult, T> : IDisposable
    {
        private TResult _result;

        /// <summary>
        /// 相关的上传控制器
        /// </summary>
        private IUploadItemController Controller { get; set; }

        /// <summary>
        /// 参数被绑定到什么。
        /// </summary>
        private int ParamIdx { get; set; }

        /// <summary>
        /// 如果已经调用了Finish()或Cancel()，则返回true。
        /// </summary>
        private bool IsFinished
        {
            get => this.Controller.StreamingIDs[this.ParamIdx] == null;
            set
            {
                if (value)
                {
                    this.Controller.StreamingIDs[this.ParamIdx] = null;
                }
            }
        }

        /// <summary>
        /// 此参数channel的唯一生成id。
        /// </summary>
        private string StreamingId => this.Controller.StreamingIDs[this.ParamIdx];

        internal UploadChannel(IUploadItemController ctrl, int paramIdx)
        {
            this.Controller = ctrl;
            this.ParamIdx = paramIdx;
        }

        /// <summary>
        /// 向服务器上传参数值。
        /// </summary>
        public void Upload(T item)
        {
            string streamId = this.StreamingId;
            if (streamId != null)
            {
                this.Controller.UploadParam(streamId, item);
            }
        }

        /// <summary>
        /// 调用此函数将取消调用本身，而不仅仅是参数上传通道。
        /// </summary>
        public void Cancel()
        {
            if (!this.IsFinished)
            {
                // 取消所有上传流，取消也将流id设置为0。
                this.Controller.Cancel();
            }
        }

        /// <summary>
        ///通过告诉服务器没有更多的上传项来结束通道。
        /// </summary>
        private void Finish()
        {
            if (this.IsFinished) return;
            string streamId = this.StreamingId;
            if (streamId == null) return;
            // 这将把流id设置为0
            this.IsFinished = true;

            var message = new Message
            {
                type = MessageTypes.Completion,
                invocationId = streamId
            };

            this.Controller.Hub.SendMessage(message);
        }

        void IDisposable.Dispose()
        {
            if (!this.IsFinished)
            {
                Finish();
            }

            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            GC.SuppressFinalize(this);
        }
    }
}
#endif