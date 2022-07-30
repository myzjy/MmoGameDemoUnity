using UnityEngine;

namespace GameTools.Singletons
{
    /// <summary>
    /// 继承与一些不能被销毁的管理类上面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MMOSingletonDontDestroy<T> : MonoBehaviour where T : MMOSingletonDontDestroy<T>
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<T>();
                    if (instance == null)
                    {
                        var obj = new GameObject(typeof(T).Name);
                        instance = obj.AddComponent<T>();
                        var parent = GameObject.Find("Boot");
                        if (parent == null)
                        {
                            parent = new GameObject("Boot");
                            DontDestroyOnLoad(parent);
                        }

                        if (parent != null)
                        {
                            obj.transform.parent = parent.transform;
                        }
                    }
                }

                return instance;
            }
        }

        private void Awake()
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
            OnAwake();
        }

        public virtual void OnAwake()
        {
            
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


        public virtual void Disable()
        {
            
        }

        private void OnDestroy()
        {
            Destroy();
        }

        public virtual void Destroy()
        {
        }

        public static bool IsValue()
        {
            return instance != null;
        }

        public void DestroySelf()
        {
            Dispose();
            instance = null;
            Destroy(gameObject);
        }
        public virtual void Dispose()
        {

        }
    }
}