using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;

#if UNITY_EDITOR

namespace FrostEngine.Editor
{
    /// <summary>
    /// 启动自动运行编译脚本
    /// </summary>
    [InitializeOnLoad]
    public class EditorExecutors
    {
        private static bool _running = false;
        private static readonly List<Task> Pending = new List<Task>();
        private static readonly List<IEnumerator> Routines = new List<IEnumerator>();

        static EditorExecutors()
        {
        }

        public static void RunOnCoroutine(Task routine)
        {
            if (routine == null)
                return;

            Pending.RemoveAll(r => r.ID == routine.ID);
            Pending.Add(routine);

            RegisterUpdate();
        }

        public static void RunOnCoroutine(IEnumerator routine)
        {
            if (routine == null)
                return;

            Routines.Add(routine);

            RegisterUpdate();
        }

        private static void RegisterUpdate()
        {
            if (_running)
                return;

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.update += OnUpdate;
                _running = true;
            }
        }

        private static void UnregisterUpdate()
        {
            if (!_running)
                return;

            EditorApplication.update -= OnUpdate;
            _running = false;
        }

        private static void OnUpdate()
        {
            if (Routines.Count <= 0 && Pending.Count <= 0)
            {
                UnregisterUpdate();
                return;
            }

            for (int i = Routines.Count - 1; i >= 0; i--)
            {
                try
                {
                    var routine = Routines[i];
                    if (!routine.MoveNext())
                        Routines.RemoveAt(i);
                }
                catch (Exception e)
                {
                    Routines.RemoveAt(i);
                    UnityEngine.Debug.LogError(e);
                }
            }

            for (int i = Pending.Count - 1; i >= 0; i--)
            {
                var routine = Pending[i];
                if (routine == null)
                    continue;

                if (routine.CanExecute())
                {
                    Routines.Add(routine);
                    Pending.RemoveAt(i);
                }
            }
        }

        private static void DoRunAsync(Action action)
        {
            ThreadPool.QueueUserWorkItem((_) => { action(); });
        }

        public static void RunAsyncNoReturn(Action action)
        {
            DoRunAsync(action);
        }

        public static void RunAsyncNoReturn<T>(Action<T> action, T t)
        {
            DoRunAsync(() => { action(t); });
        }

        public static IAsyncResult RunAsync(Action action)
        {
            AsyncResult result = new AsyncResult(true);
            DoRunAsync(() =>
            {
                try
                {
                    action();
                    result.SetResult();
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return result;
        }

        public static IAsyncResult<TResult> RunAsync<TResult>(Func<TResult> func)
        {
            AsyncResult<TResult> result = new AsyncResult<TResult>(true);
            DoRunAsync(() =>
            {
                try
                {
                    TResult tr = func();
                    result.SetResult(tr);
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return result;
        }

        public static IAsyncResult RunAsync(Action<IPromise> action)
        {
            AsyncResult result = new AsyncResult(true);
            DoRunAsync(() =>
            {
                try
                {
                    action(result);
                    if (!result.IsDone)
                        result.SetResult();
                }
                catch (Exception e)
                {
                    if (!result.IsDone)
                        result.SetException(e);
                }
            });
            return result;
        }

        public static IProgressResult<TProgress> RunAsync<TProgress>(Action<IProgressPromise<TProgress>> action)
        {
            ProgressResult<TProgress> result = new ProgressResult<TProgress>(true);
            DoRunAsync(() =>
            {
                try
                {
                    action(result);
                    if (!result.IsDone)
                        result.SetResult();
                }
                catch (Exception e)
                {
                    if (!result.IsDone)
                        result.SetException(e);
                }
            });
            return result;
        }

        public static IAsyncResult<TResult> RunAsync<TResult>(Action<IPromise<TResult>> action)
        {
            AsyncResult<TResult> result = new AsyncResult<TResult>(true);
            DoRunAsync(() =>
            {
                try
                {
                    action(result);
                    if (!result.IsDone)
                        result.SetResult();
                }
                catch (Exception e)
                {
                    if (!result.IsDone)
                        result.SetException(e);
                }
            });
            return result;
        }

        public static IProgressResult<TProgress, TResult> RunAsync<TProgress, TResult>(
            Action<IProgressPromise<TProgress, TResult>> action)
        {
            ProgressResult<TProgress, TResult> result = new ProgressResult<TProgress, TResult>(true);
            DoRunAsync(() =>
            {
                try
                {
                    action(result);
                    if (!result.IsDone)
                        result.SetResult();
                }
                catch (Exception e)
                {
                    if (!result.IsDone)
                        result.SetException(e);
                }
            });
            return result;
        }
    }

    public class Task : IEnumerator
    {
        private int id;
        private int delay;
        private IEnumerator routine;

        private long startTime;

        public Task(int id, IEnumerator routine) : this(id, 0, routine)
        {
        }

        public Task(int id, int delay, IEnumerator routine)
        {
            this.id = id;
            this.delay = delay;
            this.routine = routine;
            this.startTime = System.DateTime.Now.Ticks / 10000;
        }

        public int ID
        {
            get { return this.id; }
        }

        public int Delay
        {
            get { return this.delay; }
        }

        public bool CanExecute()
        {
            return System.DateTime.Now.Ticks / 10000 - startTime > delay;
        }

        public object Current
        {
            get { return routine.Current; }
        }

        public bool MoveNext()
        {
            return routine.MoveNext();
        }

        public void Reset()
        {
            routine.Reset();
        }
    }
}
#endif
