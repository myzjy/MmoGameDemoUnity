using System;

namespace GameTools.Singletons
{
    public abstract class SingletonMMO<T> where T : class, new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Activator.CreateInstance<T>();
                    if (instance != null)
                    {
                        (SingletonMMO<T>.instance as SingletonMMO<T>)?.Init();
                    }
                }

                return instance;
            }
        }

        public virtual void Init()
        {
        }
        
        public abstract void Dispose();

    }
}