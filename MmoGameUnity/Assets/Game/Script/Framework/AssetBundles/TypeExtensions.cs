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
using System.Linq;
using System.Reflection;
using ZJYFrameWork.AssetBundles.Observables;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.AssetBundles
{
    /// <summary>
    /// 类型扩展器
    /// </summary>
    public static class TypeExtensions
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(TypeExtensions));

        /// <summary>
        /// 子类是泛型类型定义吗
        /// </summary>
        /// <param name="type">子类 继承泛型的类型</param>
        /// <param name="genericTypeDefinition">泛型类型定义</param>
        /// <returns>返回子类是否是泛型类型定义true 是 false 为不是</returns>
        public static bool IsSubclassOfGenericTypeDefinition(this Type type, Type genericTypeDefinition)
        {
#if NETFX_CORE
//泛型类型定义
            if (!genericTypeDefinition.GetTypeInfo().IsGenericTypeDefinition)
#else
            // 泛型类型定义
            if (!genericTypeDefinition.IsGenericTypeDefinition)
#endif
                return false;

#if NETFX_CORE
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals(genericTypeDefinition))
#else
            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
#endif
                return true;

#if NETFX_CORE
            var baseType = type.GetTypeInfo().BaseType;
#else
            var baseType = type.BaseType;
#endif
            if (baseType == null || baseType == typeof(object))
                return type.GetInterfaces().Any(t => IsSubclassOfGenericTypeDefinition(t, genericTypeDefinition));
            return IsSubclassOfGenericTypeDefinition(baseType, genericTypeDefinition) || type.GetInterfaces()
                .Any(t => IsSubclassOfGenericTypeDefinition(t, genericTypeDefinition));
        }

        /// <summary>
        /// 创建默认的类型object
        /// </summary>
        /// <param name="type">需要创建的类型</param>
        /// <returns>类</returns>
        public static object CreateDefault(this Type type)
        {
            if (type == null)
                return null;

            if (type == typeof(string))
                return "";
#if NETFX_CORE
            if (!type.GetTypeInfo().IsValueType)
#else
            if (!type.IsValueType)
#endif
                return null;

            return Nullable.GetUnderlyingType(type) != null ? null : Activator.CreateInstance(type);
        }

        /// <summary>
        /// 当前方法是否为静态方法
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool IsStatic(this MemberInfo info)
        {
            var fieldInfo = info as FieldInfo;
            if (fieldInfo != null)
                return fieldInfo.IsStatic;

            var propertyInfo = info as PropertyInfo;
            if (propertyInfo != null)
            {
                var method = propertyInfo.GetGetMethod();
                if (method != null)
                    return method.IsStatic;

                method = propertyInfo.GetSetMethod();
                if (method != null)
                    return method.IsStatic;
            }

            var methodInfo = info as MethodInfo;
            if (methodInfo != null)
                return methodInfo.IsStatic;

            var eventInfo = info as EventInfo;
            if (eventInfo == null) return false;
            {
                var method = eventInfo.GetAddMethod();
                return method.IsStatic;
            }
        }

        /// <summary>
        /// 类型当中的某个值是保护类型，对其进行赋值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToSafe(this Type type, object value)
        {
            if (value == null)
                return type.CreateDefault();

            var safeValue = value;
            try
            {
                if (!type.IsInstanceOfType(value))
                {
                    if (value is IObservableProperty property && type.IsAssignableFrom(property.Type))
                    {
                        safeValue = property.Value;
                    }
                    else if (type == typeof(string))
                    {
                        safeValue = value.ToString();
                    }
#if NETFX_CORE
                    else if (type.GetTypeInfo().IsEnum)
#else
                    else if (type.IsEnum)
#endif
                    {
                        safeValue = value is string s ? Enum.Parse(type, s, true) : Enum.ToObject(type, value);
                    }
#if NETFX_CORE
                    else if (type.GetTypeInfo().IsValueType)
#else
                    else if (type.IsValueType)
#endif
                    {
                        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
                        safeValue = underlyingType == typeof(bool)
                            ? ConvertToBoolean(value)
                            : ChangeType(value, underlyingType);
                    }
                    else
                    {
                        safeValue = ChangeType(value, type);
                    }
                }
            }
            catch (Exception)
            {
            }

            //返回更改完的Object
            return safeValue;
        }

        /// <summary>
        /// 转换为布尔值
        /// </summary>
        /// <param name="result"></param>
        /// <returns>返回转换之后的布尔值</returns>
        private static bool ConvertToBoolean(object result)
        {
            switch (result)
            {
                case null:
                    return false;
                case string s:
                    return s.ToLower().Equals("true");
                case bool b:
                    return b;
            }

            var resultType = result.GetType();
#if NETFX_CORE
            if (resultType.GetTypeInfo().IsValueType)
#else
            // ReSharper disable once InvertIf
            if (resultType.IsValueType)
#endif
            {
                var underlyingType = Nullable.GetUnderlyingType(resultType) ?? resultType;
                return !result.Equals(underlyingType.CreateDefault());
            }

            return true;
        }

        /// <summary>
        /// 更改type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object ChangeType(object value, Type type)
        {
            try
            {
                return Convert.ChangeType(value, type);
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}