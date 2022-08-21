using System;
using System.Collections.Generic;
using System.Linq;
using Serialization;
using UnityEditor;
using ZJYFrameWork.AttributeCustom;

namespace ZJYFrameWork.Common.Utility
{
    public class SubClassTable
    {
        public static Type GetClass(ObjSerializer.SuperClassAttribute attr, object label)
        {
            try
            {
                return GetClass(attr.targetEnum, label);
            }
            catch(Exception e)
            {
                Debug.LogError(string.Format("label: {0} not found on {1},{2}", label, attr.targetEnum,e));
                return null;
            }
        }
        public static Type GetClass(Type targetEnum, object label)
        {
            table ??= GetGenerateSubClass();

            return table[label];
        }
        // ReSharper disable once InconsistentNaming
        private static Dictionary<object, Type> table = null;

        public static IEnumerable<Type> GetClassIE(Type targetEnum)
        {
            table ??= GetGenerateSubClass();

            foreach (var item in table)
            {
                if(item.Key.GetType() == targetEnum)
                {
                    yield return item.Value;
                }
            }
        }

        public static Dictionary<object, Type> GetGenerateSubClass()
        {
            var ret = new Dictionary<object, Type>();
            var typeDict = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(item => item.IsDefined(typeof(SubClassAttribute), false)).Select(a => a);

            foreach (var item in typeDict)
            {
                var attribute =
                    (SubClassAttribute)item.GetCustomAttributes(typeof(SubClassAttribute), false).FirstOrDefault();
                if (attribute != null)
                {
                    ret[attribute.label] = item;
                }
            }

            return ret;
        }

       
    }
}