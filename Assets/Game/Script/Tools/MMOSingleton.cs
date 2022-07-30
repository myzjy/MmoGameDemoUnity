using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameTools.Singletons
{
    /// <summary>
    /// 可能继承ui组件管理上面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MMOSingleton<T> : MonoBehaviour where T : MMOSingleton<T>
    {
        private static T instance = null;

        public static T Instance
        {
            get { return instance; }
        }


        private void Awake()
        {
            OnAwake();
        }

        public virtual void OnAwake()
        {
            instance = this as T;
        }

        private void Reset()
        {
            OnReset();
        }

        public virtual void OnReset()
        {
        }

        private void Start()
        {
            OnStart();
        }

        public virtual void OnStart()
        {
        }

        public void OnEnable()
        {
            Enable();
        }

        public virtual void Enable()
        {
        }


        public virtual void OnDisable()
        {
            Disable();
        }

        public virtual void Disable()
        {
        }

        private void OnDestroy()
        {
            Destroy();
        }

        public void DestroySelf()
        {
            Dispose();
            MMOSingleton<T>.instance = null;
            UnityEngine.Object.Destroy(gameObject);
        }

        public virtual void Destroy()
        {
        }

        public virtual void Dispose()
        {
        }

        public static bool IsValue()
        {
            return instance != null;
        }
    }
}