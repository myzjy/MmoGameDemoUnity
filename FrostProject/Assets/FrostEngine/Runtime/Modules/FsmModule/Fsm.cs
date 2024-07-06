using System;
using System.Collections.Generic;

namespace FrostEngine
{
    public sealed class Fsm<T> : FsmBase, IMemory, IFsm<T> where T : class
    {
        /// <summary>
        /// 所属者
        /// </summary>
        private T owner;

        private readonly Dictionary<Type, FsmState<T>> _fsmStates;
        private Dictionary<string, object> _datas;
        private FsmState<T> _currentState;
        private float _currentStateTime;
        private bool _isDestroyed;

        /// <summary>
        /// 初始化有限状态机的新实例。
        /// </summary>
        public Fsm()
        {
            owner = null;
            _fsmStates = new Dictionary<Type, FsmState<T>>();
            _datas = new Dictionary<string, object>(StringComparer.Ordinal);
            _currentState = null;
            _currentStateTime = 0f;
            _isDestroyed = true;
        }


        /// <summary>
        /// 获取有限状态机持有者。
        /// </summary>
        public T Owner => owner;

        /// <summary>
        /// 获取有限状态机持有者类型。
        /// </summary>s
        public override Type OwnerType => typeof(T);
        
        /// <summary>
        /// 获取有限状态机中状态的数量。
        /// </summary>
        public override int FsmStateCount => _fsmStates.Count;

        /// <summary>
        /// 获取有限状态机是否正在运行。
        /// </summary>
        public override bool IsRunning => _currentState != null;


        /// <summary>
        /// 获取有限状态机是否被销毁。
        /// </summary>
        public override bool IsDestroyed => _isDestroyed;

        /// <summary>
        /// 获取当前有限状态机状态。
        /// </summary>
        public FsmState<T> CurrentState => _currentState;

        /// <summary>
        /// 获取当前有限状态机状态名称。
        /// </summary>
        public override string CurrentStateName => _currentState?.GetType().FullName;

        /// <summary>
        /// 获取当前有限状态机状态持续时间。
        /// </summary>
        public override float CurrentStateTime => _currentStateTime;

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>创建的有限状态机。</returns>
        public static Fsm<T> Create(string name, T owner, params FsmState<T>[] states)
        {
            if (owner == null)
            {
                throw new Exception("FSM owner is invalid.");
            }

            if (CollectionUtils.IsEmpty((object[])states))
            {
                throw new Exception("FSM states is invalid.");
            }

            var abstractFsm = MemoryPool.Acquire<Fsm<T>>();
            abstractFsm.Name = name;
            abstractFsm.owner = owner;
            abstractFsm._isDestroyed = false;
            foreach (var state in states)
            {
                if (state == null)
                {
                    throw new Exception("FSM states is invalid.");
                }

                var stateType = state.GetType();
                if (!abstractFsm._fsmStates.TryAdd(stateType, state))
                {
                    throw new Exception(StringUtils.Format("FSM '{}' state '{}' is already exist.",
                        new TypeNamePair(typeof(T), name), stateType));
                }

                state.OnInit(abstractFsm);
            }

            return abstractFsm;
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>创建的有限状态机。</returns>
        public static Fsm<T> Create(string name, T owner, List<FsmState<T>> states)
        {
            if (owner == null)
            {
                throw new GameFrameworkException("FSM owner is invalid.");
            }

            if (states == null || states.Count < 1)
            {
                throw new GameFrameworkException("FSM states is invalid.");
            }

            Fsm<T> fsm = MemoryPool.Acquire<Fsm<T>>();
            fsm.Name = name;
            fsm.owner = owner;
            fsm._isDestroyed = false;
            foreach (FsmState<T> state in states)
            {
                if (state == null)
                {
                    throw new GameFrameworkException("FSM states is invalid.");
                }

                Type stateType = state.GetType();
                if (!fsm._fsmStates.TryAdd(stateType, state))
                {
                    throw new GameFrameworkException(StringUtils.Format("FSM '{}' state '{}' is already exist.",
                        new TypeNamePair(typeof(T), name), stateType.FullName));
                }

                state.OnInit(fsm);
            }

            return fsm;
        }


        /// <summary>
        /// 清理有限状态机。
        /// </summary>
        public void Clear()
        {
            if (_currentState != null)
            {
                _currentState.OnLeave(this, true);
            }

            foreach (KeyValuePair<Type, FsmState<T>> state in _fsmStates)
            {
                state.Value.OnDestroy(this);
            }

            owner = null;
            _fsmStates.Clear();
            _datas.Clear();
            _currentState = null;
            _currentStateTime = 0f;
            _isDestroyed = true;
        }

        /// <summary>
        /// 开始有限状态机。
        /// </summary>
        /// <typeparam name="TState">要开始的有限状态机状态类型。</typeparam>
        public void Start<TState>() where TState : FsmState<T>
        {
            if (IsRunning)
            {
                throw new Exception("FSM is running, can not start again.");
            }

            FsmState<T> state = GetState<TState>();
            if (state == null)
            {
                throw new Exception(StringUtils.Format(
                    "FSM '{}' can not start state '{}' which is not exist.", new TypeNamePair(typeof(T), Name),
                    typeof(TState).FullName));
            }

            _currentStateTime = 0f;
            _currentState = state;
            _currentState.OnEnter(this);
        }

        /// <summary>
        /// 开始有限状态机。
        /// </summary>
        /// <param name="stateType">要开始的有限状态机状态类型。</param>
        public void Start(Type stateType)
        {
            if (IsRunning)
            {
                throw new Exception("FSM is running, can not start again.");
            }

            if (stateType == null)
            {
                throw new Exception("State type is invalid.");
            }

            if (!typeof(FsmState<T>).IsAssignableFrom(stateType))
            {
                throw new Exception(
                    StringUtils.Format("State type '{}' is invalid.", stateType.FullName));
            }

            FsmState<T> state = GetState(stateType);

            _currentStateTime = 0f;
            _currentState = state ?? throw new Exception(StringUtils.Format(
                "FSM '{}' can not start state '{}' which is not exist.", new TypeNamePair(typeof(T), Name),
                stateType.FullName));
            _currentState.OnEnter(this);
        }

        /// <summary>
        /// 是否存在有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要检查的有限状态机状态类型。</typeparam>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState<TState>() where TState : FsmState<T>
        {
            return _fsmStates.ContainsKey(typeof(TState));
        }

        /// <summary>
        /// 是否存在有限状态机状态。
        /// </summary>
        /// <param name="stateType">要检查的有限状态机状态类型。</param>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState(Type stateType)
        {
            if (stateType == null)
            {
                throw new Exception("State type is invalid.");
            }

            if (!typeof(FsmState<T>).IsAssignableFrom(stateType))
            {
                throw new Exception(
                    StringUtils.Format("State type '{}' is invalid.", stateType.FullName));
            }

            return _fsmStates.ContainsKey(stateType);
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要获取的有限状态机状态类型。</typeparam>
        /// <returns>要获取的有限状态机状态。</returns>
        public TState GetState<TState>() where TState : FsmState<T>
        {
            FsmState<T> state = null;
            if (_fsmStates.TryGetValue(typeof(TState), out state))
            {
                return (TState)state;
            }

            return null;
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <param name="stateType">要获取的有限状态机状态类型。</param>
        /// <returns>要获取的有限状态机状态。</returns>
        public FsmState<T> GetState(Type stateType)
        {
            if (stateType == null)
            {
                throw new Exception("State type is invalid.");
            }

            if (!typeof(FsmState<T>).IsAssignableFrom(stateType))
            {
                throw new Exception(
                    StringUtils.Format("State type '{}' is invalid.", stateType.FullName));
            }

            FsmState<T> state = null;
            if (_fsmStates.TryGetValue(stateType, out state))
            {
                return state;
            }

            return null;
        }

        /// <summary>
        /// 获取有限状态机的所有状态。
        /// </summary>
        /// <returns>有限状态机的所有状态。</returns>
        public FsmState<T>[] GetAllStates()
        {
            int index = 0;
            FsmState<T>[] results = new FsmState<T>[_fsmStates.Count];
            foreach (KeyValuePair<Type, FsmState<T>> state in _fsmStates)
            {
                results[index++] = state.Value;
            }

            return results;
        }

        /// <summary>
        /// 获取有限状态机的所有状态。
        /// </summary>
        /// <param name="results">有限状态机的所有状态。</param>
        public void GetAllStates(List<FsmState<T>> results)
        {
            results.Clear();
            foreach (KeyValuePair<Type, FsmState<T>> state in _fsmStates)
            {
                results.Add(state.Value);
            }
        }

        /// <summary>
        /// 是否存在有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>有限状态机数据是否存在。</returns>
        public bool HasData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data name is invalid.");
            }

            return _datas.ContainsKey(name);
        }

        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要获取的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        public TData GetData<TData>(string name)
        {
            return (TData)GetData(name);
        }

        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        public object GetData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data name is invalid.");
            }

            return _datas.GetValueOrDefault(name);
        }

        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要设置的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetData<TData>(string name, TData data)
        {
            SetData(name, (object)data);
        }

        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要设置的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetData(string name, object data)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data name is invalid.");
            }

            _datas ??= new Dictionary<string, object>(StringComparer.Ordinal);

            _datas[name] = data;
        }


        public void ClearData()
        {
            _datas.Clear();
        }

        /// <summary>
        /// 移除有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>是否移除有限状态机数据成功。</returns>
        public bool RemoveData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data name is invalid.");
            }
            
            if (_datas == null)
            {
                return false;
            }
            
            return _datas.Remove(name);
        }

        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (_currentState == null)
            {
                return;
            }

            _currentStateTime += elapseSeconds;
            _currentState.OnUpdate(this, elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 关闭并清理有限状态机。
        /// </summary>
        internal override void Shutdown()
        {
            MemoryPool.Release(this);
        }

        public void ChangeState<TNextState>() where TNextState : FsmState<T>
        {
            var stateType = typeof(TNextState);
            ChangeState(stateType);
        }

        public void ChangeState(Type stateType)
        {
            if (_currentState == null)
            {
                throw new Exception("Current state is invalid.");
            }

            FsmState<T> state = GetState(stateType);
            if (state == null)
            {
                throw new Exception(StringUtils.Format("FSM '{}' can not change state to '{}' which is not exist.",
                    typeof(T).Name, stateType.FullName));
            }

            _currentState.OnLeave(this, false);
            _currentStateTime = 0f;
            _currentState = state;
            _currentState.OnEnter(this);
        }
    }
}