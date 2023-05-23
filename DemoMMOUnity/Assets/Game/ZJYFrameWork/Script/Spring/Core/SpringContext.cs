using System;
using System.Collections.Generic;
using System.Linq;
using ZJYFrameWork.Contexts;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Spring.Core
{
    /// <summary>
    /// SpringContext 扫描类
    /// </summary>
    public sealed class SpringContext
    {
        private static ApplicationContext _context;

        /// <summary>
        /// bean标签 容器
        /// </summary>
        private static readonly Dictionary<Type, object> beanMap = new Dictionary<Type, object>();

        /// <summary>
        /// 缓存的bean标签 dict字典
        /// </summary>
        private static readonly Dictionary<Type, object> cachedBeanMap = new Dictionary<Type, object>();

        public static readonly List<string> scanPaths = new List<string>();

        /**
         * 扫描状态，true标识已经扫描过，false标识没有扫描过
         */
        private static bool scanFlag;

        private SpringContext()
        {
            _context = Context.GetApplicationContext();
        }

        public static ApplicationContext GetApplicationContext()
        {
            if (_context == null)
            {
                _context = Context.GetApplicationContext();
            }

            return _context;
        }

        public static bool GetScanFlag()
        {
            return scanFlag;
        }

        /// <summary>
        /// 添加路径
        /// </summary>
        /// <param name="includePathList"></param>
        public static void AddScanPath(List<string> includePathList)
        {
            scanPaths.AddRange(includePathList);
        }

        /// <summary>
        /// 通过类型 注册bean注解
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RegisterBean<T>()
        {
            return (T)RegisterBean(typeof(T));
        }

        /// <summary>
        /// 注册Bean注解
        /// </summary>
        /// <param name="bean"></param>
        public static void RegisterBean(object bean)
        {
            checkBean(bean);
            var type = bean.GetType();
            beanMap.Add(type, bean);
        }

        /// <summary>
        /// 注册Bean注解
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static object RegisterBean(Type type)
        {
            //包含用于在本地或远程创建对象类型或获取对现有远程对象的引用的方法
            var bean = Activator.CreateInstance(type);
            // Debug.Log(bean);
            if (bean == null)
            {
                throw new Exception($"无法通过类型[{type}]创建实例");
            }

            RegisterBean(bean);
            return bean;
        }

        // public static object TryGetBean(Type type)
        // {
        //     
        // }


        /// <summary>
        /// 扫描
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void Scan()
        {
            if (scanFlag)
            {
                throw new Exception("SpringContext已经扫描，不需要重复扫描");
            }

            //获取所有类
            var types = AssemblyUtils.GetAllClassTypes();

            //获取所有被Ben注解标注的类
            foreach (var type in types.Where(type => type.IsDefined(typeof(BeanAttribute), false))
                         .Where(type =>
                         {
                             return scanPaths.Any(it => type.FullName != null && type.FullName.StartsWith(it));
                         }))
            {
                RegisterBean(type);
            }

            // 注入被Autowired注解标注的属性
            foreach (var item in beanMap)
            {
                //类型
                var type = item.Key;
                //bean
                var beanObj = item.Value;
                //获取Bean注解标识的类里面所有属性
                var fields = AssemblyUtils.GetFieldTypes(type);
                foreach (var fieldInfo in fields)
                {
                    if (!fieldInfo.IsDefined(typeof(AutowiredAttribute), false)) continue;
                    var fieldType = fieldInfo.FieldType;
                    try
                    {
                        var autowiredFieldBean = GetBean(fieldType);
                        fieldInfo.SetValue(beanObj, autowiredFieldBean);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(StringUtils.Format("在类[{}]中注入[{}]类型异常"
                            , beanObj.GetType().Name, fieldType.Name), e);
                    }
                }
            }

            // 调用被BeforePostConstruct注解标注的方法
            foreach (var pair in beanMap)
            {
                var type = pair.Key;
                var beanObj = pair.Value;
                var methods = AssemblyUtils.GetMethodsByAnnoInPOJOClass(type, typeof(BeforePostConstructAttribute));
                foreach (var method in methods)
                {
                    method.Invoke(beanObj, null);
                }
            }

            // 调用被PostConstruct注解标注的方法
            foreach (var pair in beanMap)
            {
                var type = pair.Key;
                var bean = pair.Value;
                var methods = AssemblyUtils.GetMethodsByAnnoInPOJOClass(type, typeof(PostConstructAttribute));
                foreach (var method in methods)
                {
                    method.Invoke(bean, null);
                }
            }

            // 调用被AfterPostConstruct注解标注的方法
            foreach (var pair in beanMap)
            {
                var type = pair.Key;
                var bean = pair.Value;
                var methods = AssemblyUtils.GetMethodsByAnnoInPOJOClass(type, typeof(AfterPostConstructAttribute));
                foreach (var method in methods)
                {
                    method.Invoke(bean, null);
                }
            }

            scanFlag = true;
        }

        public static T GetBean<T>()
        {
            return (T)GetBean(typeof(T));
        }

        public static List<object> GetBeans(Type type)
        {
            return beanMap
                .Where(it => type.IsAssignableFrom(it.Key))
                .Select(it => it.Value)
                .ToList();
        }

        public static object GetBean(string beanString)
        {
            Debug.Log(beanString);
            object bean = null;
            foreach (var (key, value) in cachedBeanMap)
            {
                var typeFull = key.FullName;
                if (typeFull != null)
                {
                    var fullSplit = typeFull.Split('.');
                    var beanSplit = fullSplit[^1];
                    if (beanSplit == beanString)
                    {
                        bean = value;
                    }
                }
            }

            return bean;
        }

        public static object GetBean(Type type)
        {
            Debug.Log(type);
            object bean = null;

            // 先从缓存获取
            cachedBeanMap.TryGetValue(type, out bean);
            if (bean != null)
            {
                return bean;
            }

            // 再从全部的组件获取
            beanMap.TryGetValue(type, out bean);
            if (bean != null)
            {
                return bean;
            }

            // 如果是接口类型，没有办法直接获取bean，这里遍历全部的bean
            var findList = beanMap.Keys.Where(type.IsAssignableFrom).ToList();
            if (CollectionUtils.IsEmpty(findList))
            {
                throw new Exception(StringUtils.Format("无法找到类型[{}]的实例", type.Name));
            }

            if (findList.Count > 1)
            {
                throw new Exception(StringUtils.Format("类型[{}]存在多个[{}][{}]实例"
                    , type.Name, findList.First().Name, findList.Skip(1).First().Name));
            }

            //获取注册在BeanMap里面的bean
            bean = beanMap[findList.First()];

            // 缓存结果
            cachedBeanMap[type] = bean;

            return bean;
        }

        public static object TryGetBean(Type type)
        {
            Debug.Log(type);
            object bean = null;

            // 先从缓存获取
            cachedBeanMap.TryGetValue(type, out bean);
            if (bean != null)
            {
                return bean;
            }

            // 再从全部的组件获取
            beanMap.TryGetValue(type, out bean);
            if (bean != null)
            {
                return bean;
            }

            // 如果是接口类型，没有办法直接获取bean，这里遍历全部的bean
            var findList = beanMap.Keys.Where(type.IsAssignableFrom).ToList();
            if (CollectionUtils.IsEmpty(findList))
            {
                throw new Exception(StringUtils.Format("无法找到类型[{}]的实例", type.Name));
            }

            if (findList.Count > 1)
            {
                throw new Exception(StringUtils.Format("类型[{}]存在多个[{}][{}]实例"
                    , type.Name, findList.First().Name, findList.Skip(1).First().Name));
            }

            //获取注册在BeanMap里面的bean
            bean = beanMap[findList.First()];

            // 缓存结果
            cachedBeanMap[type] = bean;

            return bean;
        }

        public static List<object> GetAllBeans()
        {
            var list = new List<object>();
            foreach (var pair in beanMap)
            {
                list.Add(pair.Value);
            }

            return list;
        }

        public static void Shutdown()
        {
            scanFlag = false;
            scanPaths.Clear();
            beanMap.Clear();
            cachedBeanMap.Clear();
        }

        public static void checkBean(object bean)
        {
            if (bean == null)
            {
                throw new Exception(StringUtils.Format("实例[{}]为空，无法创建", null));
            }

            var type = bean.GetType();
            if (beanMap.ContainsKey(type))
            {
                throw new Exception(StringUtils.Format("类型[{}]重复创建实例", type.Name));
            }
        }

        public static void UnBean(object bean)
        {
            var type = bean.GetType();
            if (beanMap.ContainsKey(type))
            {
                Debug.Log("已经保存，需要清除");
                beanMap.Remove(type);
            }
        }
    }
}