using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace ZJYFrameWork.Spring.Utils
{
    public abstract class AssemblyUtils
    {
        /**
        * 获取所有程序集，这里过滤掉不需要程序集
        * 
        */
        private static readonly Assembly[] allAssemblys = AppDomain.CurrentDomain.GetAssemblies().Where(a =>
            a.Equals(typeof(AssemblyUtils).Assembly) || 
            a.FullName.StartsWith("Assembly-CSharp-Editor")||
            a.FullName.StartsWith("Assembly-CSharp")||
            a.FullName.StartsWith("Main")||
            a.FullName.StartsWith("ZJYFrameWork.AssetBundles.Bundles")||
            a.FullName.StartsWith("ZJYFrameWork.Log")
        ).ToArray();

        private static readonly Dictionary<string, Type> cachedTypes = new Dictionary<string, Type>();

        /// <summary>
        /// 获取Type里面所有属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldInfo[] GetFieldTypes(Type type)
        {
            return type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
                                  | BindingFlags.Instance | BindingFlags.Default);
        }

        public static MethodInfo[] GetMethodsTypes(Type type)
        {
            return type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
                                   | BindingFlags.Instance | BindingFlags.Default);
        }

        public static FieldInfo[] GetFieldsByAnnoInPOJOClass(Type type, Type attribute)
        {
            var list = new List<FieldInfo>();
            var fields = GetFieldTypes(type);
            foreach (var field in fields)
            {
                if (field.IsDefined(attribute, false))
                {
                    list.Add(field);
                }
            }

            return list.ToArray();
        }

        public static MethodInfo[] GetMethodsByAnnoInPOJOClass(Type type, Type attribute)
        {
            var list = new List<MethodInfo>();
            MethodInfo[] methods = GetMethodsTypes(type);
            foreach (var method in methods)
            {
                if (method.IsDefined(attribute, false))
                {
                    list.Add(method);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取已加载的程序集中的所有类型。
        /// </summary>
        /// <returns>已加载的程序集中的所有类型。</returns>
        public static List<Type> GetAllClassTypes()
        {
            var results = new List<Type>();
            foreach (var assembly in allAssemblys)
            {
                results.AddRange(assembly.GetTypes());
            }

            return results;
        }

        /// <summary>
        /// 获取已加载的程序集中的指定类型。
        /// </summary>
        /// <param name="typeName">要获取的类型名。</param>
        /// <returns>已加载的程序集中的指定类型。</returns>
        public static Type GetTypeByName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new Exception("Type name is invalid.");
            }

            if (cachedTypes.TryGetValue(typeName, out var type))
            {
                return type;
            }

            type = Type.GetType(typeName);
            if (type != null)
            {
                cachedTypes.Add(typeName, type);
                return type;
            }

            foreach (var assembly in allAssemblys)
            {
                type = Type.GetType(StringUtils.Format("{}, {}", typeName, assembly.FullName));
                if (type == null) continue;
                cachedTypes.Add(typeName, type);
                return type;
            }

            return null;
        }

        /// <summary>
        /// 获取指定基类的所有子类的名称。
        /// </summary>
        /// <param name="typeBase">基类类型。</param>
        /// <returns>指定基类的所有子类的名称。</returns>
        public static string[] GetAllSubClassNames(Type typeBase)
        {
            return GetAllSubClassType(typeBase).Select(it => it.FullName).ToArray();
        }

        public static List<Type> GetAllSubClassType(Type typeBase)
        {
            var allClassTypes = GetAllClassTypes();
            return allClassTypes.Where(type => type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                .ToList();
        }

        /**
         * 获取所有被T注解修饰的Field
         */
        public static List<FieldInfo> GetAllFieldsByAttribute<T>() where T : Attribute
        {
            var allClassTypes = GetAllClassTypes();
            var result = new List<FieldInfo>();

            foreach (var fields in allClassTypes.Select(GetFieldTypes))
            {
                result.AddRange(fields.Where(fieldInfo => fieldInfo.IsDefined(typeof(T), false)));
            }

            return result;
        }

        public static object GetField(FieldInfo field, object target)
        {
            try
            {
                return field.GetValue(target);
            }
            catch (Exception e)
            {
                throw new Exception(StringUtils.Format("Unexpected reflection exception - [{}]", e.GetType().Name), e);
            }
        }

        public static void SetField(FieldInfo field, Object target, Object value)
        {
            try
            {
                field.SetValue(target, value);
            }
            catch (Exception e)
            {
                throw new Exception(StringUtils.Format("Unexpected reflection exception - [{}]", e.GetType().Name, e));
            }
        }
    }
}