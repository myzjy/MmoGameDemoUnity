using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZJYFrameWork.Common.Utility
{
    public static class TypeExtensions
    {
        private static Dictionary<object, object[]> s_attrTable = new Dictionary<object, object[]>();

        public static T GetAttribute<T>(this Type t)
        {
            return (T)GetAttributes(t).FirstOrDefault(x => x is T);
        }

        static object[] GetAttributes(object o)
        {
            object[] ret = null;
            if (s_attrTable.TryGetValue(o, out ret))
            {
                return ret;
            }
            else
            {
                if (o is FieldInfo)
                {
                    ret = (o as FieldInfo).GetCustomAttributes(true);
                }
                else if (o is MemberInfo)
                {
                    ret = (o as MemberInfo).GetCustomAttributes(true);
                }
                else if (o is Type)
                {
                    ret = (o as Type).GetCustomAttributes(true);
                }

                s_attrTable[o] = ret;
                return ret;
            }
        }
        public static T GetAttribute<T>(this FieldInfo f)
        {
            return (T)GetAttributes(f).FirstOrDefault(x => x is T);
        }
        
        public static System.Type GetType(string typeName)
        {
            System.Type type = System.Type.GetType(typeName);

            if (type == null)
            {
                var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies()
                    from assemblyType in assembly.GetTypes()
                    where assemblyType.FullName == typeName
                    select assemblyType;

                type = types.FirstOrDefault();
            }

            return type;
        }
    }
}