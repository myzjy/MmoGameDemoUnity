/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace FrostEngine
{
    public class CoroutineScheduledExecutor : AbstractExecutor, IScheduledExecutor
    {
        private readonly ComparerImpl<IDelayTask> _comparer = new ComparerImpl<IDelayTask>();
        private readonly List<IDelayTask> _queue = new List<IDelayTask>();
        private bool _running;

        // ReSharper disable once EmptyConstructor
        public CoroutineScheduledExecutor()
        {
        }

        public void Start()
        {
            if (this._running)
                return;

            this._running = true;

            InterceptableEnumerator ie = new InterceptableEnumerator(DoStart());
            ie.RegisterCatchBlock(e => { this._running = false; });
            Executors.RunOnCoroutineNoReturn(ie);
        }

        public void Stop()
        {
            if (!this._running)
                return;

            this._running = false;
            List<IDelayTask> list = new List<IDelayTask>(_queue);
            foreach (var task in list.Where(task => task is { IsDone: false }))
            {
                task.Cancel();
            }

            this._queue.Clear();
        }

        public virtual IAsyncResult Schedule(Action command, long delay)
        {
            return this.Schedule(command, new TimeSpan(delay * TimeSpan.TicksPerMillisecond));
        }

        public virtual IAsyncResult Schedule(Action command, TimeSpan delay)
        {
            this.Check();
            return new OneTimeDelayTask(this, command, delay);
        }

        public virtual IAsyncResult<TResult> Schedule<TResult>(Func<TResult> command, long delay)
        {
            return this.Schedule(command, new TimeSpan(delay * TimeSpan.TicksPerMillisecond));
        }

        public virtual IAsyncResult<TResult> Schedule<TResult>(Func<TResult> command, TimeSpan delay)
        {
            this.Check();
            return new OneTimeDelayTask<TResult>(this, command, delay);
        }

        public virtual IAsyncResult ScheduleAtFixedRate(Action command, long initialDelay, long period)
        {
            return this.ScheduleAtFixedRate(command, new TimeSpan(initialDelay * TimeSpan.TicksPerMillisecond),
                new TimeSpan(period * TimeSpan.TicksPerMillisecond));
        }

        public virtual IAsyncResult ScheduleAtFixedRate(Action command, TimeSpan initialDelay,
            TimeSpan period)
        {
            this.Check();
            return new FixedRateDelayTask(this, command, initialDelay, period);
        }

        public virtual IAsyncResult ScheduleWithFixedDelay(Action command, long initialDelay, long delay)
        {
            return this.ScheduleWithFixedDelay(command, new TimeSpan(initialDelay * TimeSpan.TicksPerMillisecond),
                new TimeSpan(delay * TimeSpan.TicksPerMillisecond));
        }

        public virtual IAsyncResult ScheduleWithFixedDelay(Action command, TimeSpan initialDelay,
            TimeSpan delay)
        {
            this.Check();
            return new FixedDelayDelayTask(this, command, initialDelay, delay);
        }

        public virtual void Dispose()
        {
            this.Stop();
        }

        private void Add(IDelayTask task)
        {
            _queue.Add(task);
            _queue.Sort(_comparer);
        }

        private bool Remove(IDelayTask task)
        {
            if (_queue.Remove(task))
            {
                _queue.Sort(_comparer);
                return true;
            }

            return false;
        }

        protected virtual IEnumerator DoStart()
        {
            while (_running)
            {
                while (_running && (_queue.Count <= 0 || _queue[0].Delay.Ticks > 0))
                {
                    yield return null;
                }

                if (!_running)
                    yield break;

                IDelayTask task = _queue[0];
                _queue.RemoveAt(0);
                task.Run();
            }
        }

        protected virtual void Check()
        {
            if (!this._running)
                throw new RejectedExecutionException("The ScheduledExecutor isn't started.");
        }

        interface IDelayTask : IAsyncResult
        {
            TimeSpan Delay { get; }

            void Run();
        }

        private sealed class OneTimeDelayTask : AsyncResult, IDelayTask
        {
            private readonly Action _command;
            private readonly CoroutineScheduledExecutor _executor;
            private readonly long _startTime;
            private TimeSpan _delay;

            public OneTimeDelayTask(CoroutineScheduledExecutor executor, Action command, TimeSpan delay)
            {
                this._startTime = (long)(Time.fixedTime * TimeSpan.TicksPerSecond);
                this._delay = delay;
                this._executor = executor;
                this._command = command;
                this._executor.Add(this);
            }

            public TimeSpan Delay =>
                new TimeSpan(_startTime + _delay.Ticks - (long)(Time.fixedTime * TimeSpan.TicksPerSecond));

            public override bool Cancel()
            {
                if (this.IsDone)
                    return false;

                if (!this._executor.Remove(this))
                    return false;

                this.cancellationRequested = true;
                this.SetCancelled();
                return true;
            }

            public void Run()
            {
                try
                {
                    if (this.IsDone)
                        return;

                    if (this.IsCancellationRequested)
                    {
                        this.SetCancelled();
                    }
                    else
                    {
                        _command();
                        this.SetResult();
                    }
                }
                catch (Exception e)
                {
                    this.SetException(e);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError(e);
#endif
                }
            }
        }

        sealed class OneTimeDelayTask<TResult> : AsyncResult<TResult>, IDelayTask
        {
            private readonly Func<TResult> _command;
            private readonly CoroutineScheduledExecutor _executor;
            private readonly long _startTime;
            private TimeSpan _delay;

            public OneTimeDelayTask(CoroutineScheduledExecutor executor, Func<TResult> command, TimeSpan delay)
            {
                this._startTime = (long)(Time.fixedTime * TimeSpan.TicksPerSecond);
                this._delay = delay;
                this._executor = executor;
                this._command = command;
                this._executor.Add(this);
            }

            public TimeSpan Delay =>
                new TimeSpan(_startTime + _delay.Ticks - (long)(Time.fixedTime * TimeSpan.TicksPerSecond));

            public override bool Cancel()
            {
                if (this.IsDone)
                    return false;

                if (!this._executor.Remove(this))
                    return false;

                this.cancellationRequested = true;
                this.SetCancelled();
                return true;
            }

            public void Run()
            {
                try
                {
                    if (this.IsDone)
                        return;

                    if (this.IsCancellationRequested)
                    {
                        this.SetCancelled();
                    }
                    else
                    {
                        this.SetResult(_command());
                    }
                }
                catch (Exception e)
                {
                    this.SetException(e);
                    Debug.LogError(e);
                }
            }
        }

        private sealed class FixedRateDelayTask : AsyncResult, IDelayTask
        {
            private readonly Action _command;
            private readonly CoroutineScheduledExecutor _executor;
            private readonly long _startTime;
            private int _count;
            private TimeSpan _initialDelay;
            private TimeSpan _period;

            public FixedRateDelayTask(CoroutineScheduledExecutor executor, Action command, TimeSpan initialDelay,
                TimeSpan period)
            {
                this._startTime = (long)(Time.fixedTime * TimeSpan.TicksPerSecond);
                this._initialDelay = initialDelay;
                this._period = period;
                this._executor = executor;
                this._command = command;
                this._executor.Add(this);
            }

            public TimeSpan Delay =>
                new TimeSpan(_startTime + _initialDelay.Ticks + _period.Ticks * _count -
                             (long)(Time.fixedTime * TimeSpan.TicksPerSecond));

            public override bool Cancel()
            {
                if (this.IsDone)
                    return false;

                this._executor.Remove(this);
                this.cancellationRequested = true;
                this.SetCancelled();
                return true;
            }

            public void Run()
            {
                try
                {
                    if (this.IsDone)
                        return;

                    if (this.IsCancellationRequested)
                    {
                        this.SetCancelled();
                    }
                    else
                    {
                        Interlocked.Increment(ref _count);
                        this._executor.Add(this);
                        _command();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        private sealed class FixedDelayDelayTask : AsyncResult, IDelayTask
        {
            private readonly Action _command;
            private readonly CoroutineScheduledExecutor _executor;
            private TimeSpan _delay;
            private long _nextTime;

            public FixedDelayDelayTask(CoroutineScheduledExecutor executor, Action command, TimeSpan initialDelay,
                TimeSpan delay)
            {
                this._delay = delay;
                this._executor = executor;
                this._command = command;
                this._nextTime = (long)(Time.fixedTime * TimeSpan.TicksPerSecond + initialDelay.Ticks);
                this._executor.Add(this);
            }

            public TimeSpan Delay => new TimeSpan(_nextTime - (long)(Time.fixedTime * TimeSpan.TicksPerSecond));

            public override bool Cancel()
            {
                if (this.IsDone)
                    return false;

                this._executor.Remove(this);
                this.cancellationRequested = true;
                this.SetCancelled();
                return true;
            }

            public void Run()
            {
                try
                {
                    if (this.IsDone)
                        return;

                    if (this.IsCancellationRequested)
                    {
                        this.SetCancelled();
                    }
                    else
                    {
                        _command();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                finally
                {
                    if (this.IsCancellationRequested)
                    {
                        this.SetCancelled();
                    }
                    else
                    {
                        this._nextTime = (long)(Time.fixedTime * TimeSpan.TicksPerSecond + this._delay.Ticks);
                        this._executor.Add(this);
                    }
                }
            }
        }

        class ComparerImpl<T> : IComparer<T> where T : IDelayTask
        {
            public int Compare(T x, T y)
            {
                if (y != null && x != null && x.Delay.Ticks == y.Delay.Ticks)
                {
                    return 0;
                }

                return y != null && x != null && x.Delay.Ticks > y.Delay.Ticks ? 1 : -1;
            }
        }
    }
}