using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
// using UniRx;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools.Util
{
    public static class Util
    {
        public static void PerformOnBgThread<T>(Func<T> _func, Action<T> action)
        {
            action?.Execute(_func.Invoke());
        }

        public static void Execute(this System.Action _action)
        {
            _action?.Invoke();
        }

        public static void Execute<T>(this System.Action<T> _action, T m_t)
        {
            _action?.Invoke(m_t);
        }

        public static void Execute<T, Ty>(this System.Action<T, Ty> _action, T m_t, Ty m_ty)
        {
            _action?.Invoke(m_t, m_ty);
        }

        public static void Execute<T, Ty, TB>(this System.Action<T, Ty, TB> _action, T m_t, Ty m_ty, TB _tl)
        {
            _action?.Invoke(m_t, m_ty, _tl);
        }

        public static void Execute<T, Ty, TB, TY>(this System.Action<T, Ty, TB, TY> _action, T m_t, Ty m_ty, TB _tl,
            TY m_ty_1)
        {
            _action?.Invoke(m_t, m_ty, _tl, m_ty_1);
        }

        public static T GetComponentUtil<T>(this Transform _transform)
        {
            var com = _transform.gameObject.GetComponent<T>();
            return com;
        }

        public static bool IsDictNull<T, Values>(this Dictionary<T, Values> _dictionary)
        {
            return _dictionary == null;
        }

        /// <summary>
        /// 进行复制完全内容，避免引用时被修改
        /// </summary>
        /// <param name="V"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CloneByObject<T>(this T V)
        {
            T temp;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream _stream = new MemoryStream();
            try
            {
                bf.Serialize(_stream, V);
                _stream.Position = 0L;
                temp = (T) bf.Deserialize(_stream);
            }
            finally
            {
                _stream.Close();
            }

            return temp;
        }

        public static void InstanceObj(GameObject _gameObject)
        {
            Object.Instantiate(_gameObject);
        }

        public static T InstantiateGetComponent<T>(GameObject m_obj, Transform m_tr = null, bool isActive = true)
            where T : Component
        {
            GameObject go = InstantiateGameObject(m_obj, m_tr, isActive);
            return go.GetComponent<T>();
        }

        public static GameObject InstantiateGameObject(GameObject m_obj, Transform m_tr = null, bool isActive = true)
        {
            GameObject go = Object.Instantiate(m_obj) as GameObject;
            if (m_tr != null)
            {
                go.transform.SetParent(m_tr);
            }

            go.name = m_obj.name;
            if (!isActive)
            {
                go.SetActive(false);
            }

            return go;
        }


        public static void Executes<T>(this Action<T> _action, Func<T> mT)
        {
            _action?.Execute<T>(mT());
        }
        /// <summary>
        /// 执行一个线程
        /// </summary>
        /// <param name="onComplte"></param>
        /// <param name="finish"></param>
        public static void PerformOnBgThread(Action onComplte, Action finish)
        {
            // Observable.Start(onComplte).ObserveOnMainThread().Subscribe(res => finish.Execute());
        }

        public static void PerformOnBgThread<T>(Action<T> finish, Func<T> onComplte)
        {
            // Observable.Start(onComplte).ObserveOnMainThread().Subscribe(finish);
        }
    
    }
}