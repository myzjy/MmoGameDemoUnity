using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serialize;
using UnityEngine;
using ZJYFrameWork.AttributeCustom;
using ZJYFrameWork.Common.Utility;
using ZJYFrameWork.AttributeCustom;
using ZJYFrameWork.UISerializable.Manager;
using PureObject = System.Object;

namespace Serialization
{
    public interface IObjSerializable
    {
        object Serialize(SerializationContext ctx);
    }

    public interface IObjDeserializable
    {
        void Deserialize(SerializationContext ctx, object o);
    }

    public interface IDeserializationHandler
    {
        object OnDeserialize(Dictionary<string, object> dict);
    }

    public class ObjSerializer
    {
#if PROFILE_DYNAMIC_DESERIALIZATION
		static Dictionary<Type, int> s_dynamicDeserializationCount = new Dictionary<Type, int>();
#endif
        // リフレクションの結果キャッシュ
        static Dictionary<Type, List<FieldEntry>> typeCache = new Dictionary<Type, List<FieldEntry>>();

        static object syncRoot = new object();

        public class FieldEntry
        {
            public FieldInfo field;
            public string name;
            public OptionalAttribute optional;
            public NotEmptyAttribute notEmpty;
            public bool dontSerialize;
            public bool dontSerializeNull;
            public Func<SerializationContext, Type, PureObject, object> deserializer;

            public bool permitNull
            {
                get { return true; /*optional != null && !optional.notNull;*/ }
            }

            public bool permitNoField
            {
                get { return true; /*optional != null;*/ }
            }

            public bool generateDefault
            {
                get { return false; /*optional != null && optional.generateDefault;*/ }
            }
        }

        public static PureObject Serialize(object o, SerializationContext context = null)
        {
            return SerializeImpl(context == null ? new SerializationContext() : context, o);
        }

        public static T Deserialize<T>(PureObject o, SerializationContext context = null)
        {
            return (T)DeserializeImpl(context == null ? new SerializationContext() : context, typeof(T), o);
        }

        public static object Deserialize(Type t, PureObject o, SerializationContext context = null)
        {
            return DeserializeImpl(context == null ? new SerializationContext() : context, t, o);
        }

        // オブジェクトを単純オブジェクトに変換
        static PureObject SerializeImpl(SerializationContext context, object o, Type typeHint = null)
        {
            if (o == null)
            {
                return null;
            }
            else if (o.GetType().IsEnum)
            {
                return o.ToString();
            }
            else
                switch (o)
                {
                    case DateTime time when !time.IsValid():
                        return 0;
                    case DateTime time:
                        return (int)(time.ToUnixTimeStamp());
                    case TimeSpan span:
                        return (int)(span.TotalSeconds);
                    case Vector2 vector2:
                        return new List<object>()
                        {
                            SerializeImpl(context, vector2.x, typeof(float)),
                            SerializeImpl(context, vector2.y, typeof(float))
                        };
                    case Vector3 vec:
                        return new List<object>()
                        {
                            SerializeImpl(context, vec.x, typeof(float)),
                            SerializeImpl(context, vec.y, typeof(float)),
                            SerializeImpl(context, vec.z, typeof(float))
                        };
                    default:
                    {
                        if (o.GetType().IsValueType)
                        {
                            if (o is IObjSerializable)
                            {
                                return (o as IObjSerializable).Serialize(context);
                            }
                            else
                            {
                                return o;
                            }
                        }
                        else if (o is string)
                        {
                            return o;
                        }
                        else if (o.GetType().IsArray || o is IList)
                        {
                            var elementType = o.GetType().IsArray
                                ? o.GetType().GetElementType()
                                : o.GetType().GetGenericArguments()[0];

                            return (from object obj in (IEnumerable)o select SerializeImpl(context, obj, elementType))
                                .ToList();
                        }
                        else if (o is IObjSerializable serializable)
                        {
                            var ret = serializable.Serialize(context);
                            //				if(ret is IObjSerializable) Logging.Warning("serializer", "invalid value");
                            return ret;
                        }
                        else
                        {
                            return SerializeObject(context, o);
                        }
                    }
                }
        }

        public static Dictionary<string, object> SerializeObject(SerializationContext context, object o)
        {
            List<FieldEntry> fieldList = GetSerializeFields(o.GetType());
            int fieldCount = fieldList.Count;
            Dictionary<string, object> dic = new Dictionary<string, object>(fieldCount);
            bool isFull = context.isFull;
            for (int i = 0; i < fieldCount; i++)
            {
                FieldEntry f = fieldList[i];
                if (isFull || !f.dontSerialize)
                {
                    var obj = f.field.GetValue(o);
                    if (!f.dontSerializeNull || obj != null)
                    {
                        dic[f.name] = SerializeImpl(context, obj, f.field.FieldType);
                    }
                }
            }

            var subClass = o.GetType().GetAttribute<SubClassAttribute>();
            var superClass = o.GetType().GetAttribute<SuperClassAttribute>();
            if (subClass != null && superClass != null)
            {
                dic[superClass.field] = SerializeImpl(context, superClass.ConvertLabel(subClass.label));
            }

            return dic;
        }

        public static bool IsNaturalValueType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Boolean:
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
                default:
                    return false;
            }
        }

        public static object DeserializeEnum(SerializationContext context, Type t, PureObject o)
        {
            return o is string s ? EnumGetValueOrFirst(t, s) : Enum.ToObject(t, o);
        }

        public static object DeserializeString(SerializationContext context, Type t, PureObject o)
        {
            return o;
        }

        public static object DeserializeDateTime(SerializationContext context, Type t, PureObject o)
        {
            o = o ?? 0;
            return DateTimeUtil.FromUnixTimeStamp((double)Convert.ChangeType(o, typeof(System.Double)));
        }

        public static object DeserializeTimeSpan(SerializationContext context, Type t, PureObject o)
        {
            o = o ?? 0;
            return TimeSpan.FromSeconds((double)Convert.ChangeType(o, typeof(System.Double)));
        }

        public static object DeserializeVector2(SerializationContext context, Type t, PureObject o)
        {
            var et = typeof(float);
            var elementDeserializer = GetDeserializer(et);
            var arr = (float[])DeserializeArray(context, t, o, et, elementDeserializer);
            return new Vector2(arr[0], arr[1]);
        }

        public static object DeserializeVector3(SerializationContext context, Type t, PureObject o)
        {
            var et = typeof(float);
            var elementDeserializer = GetDeserializer(et);
            var arr = (float[])DeserializeArray(context, t, o, et, elementDeserializer);
            return new Vector3(arr[0], arr[1], arr[2]);
        }

        public static object DeserializeNaturalValue(SerializationContext context, Type t, PureObject o)
        {
            return Convert.ChangeType(o, t);
        }

        public static object DeserializeArray(SerializationContext context, Type t, PureObject o, Type et,
            Func<SerializationContext, Type, PureObject, object> elementDeserializer)
        {
            if (o == null) return null;
            context.Push(t);
            var originalArr = o as IList;
            var cnt = originalArr.Count;
            var arr = Array.CreateInstance(et, cnt);
            for (int i = 0; i < originalArr.Count; i++)
            {
                arr.SetValue(elementDeserializer(context, et, originalArr[i]), i);
            }

            context.Pop(t);
            return arr;
        }

        public static object DeserializeIObjSerializable(SerializationContext context, Type t, PureObject o)
        {
            if (o == null) return null;
            context.Push(t);
            IObjDeserializable
                ret = /*StaticObjSerializer.enabled ? StaticObjSerializer.CreateInstance(t) as IObjDeserializable :*/
                    Activator.CreateInstance(t) as IObjDeserializable;
            ret.Deserialize(context, o);
            if (ret is IDeserializationHandler)
            {
                ret = (ret as IDeserializationHandler).OnDeserialize(o as Dictionary<string, object>) as
                    IObjDeserializable;
            }

            context.Push(ret);
            context.Pop(ret);
            context.Pop(t);
            return ret;
        }

        public static object DeserializeList(SerializationContext context, Type t, PureObject o, Type et,
            Func<SerializationContext, Type, PureObject, object> elementDeserializer)
        {
            if (o == null) return null;
            context.Push(t);
            var originalArr = o as IList;
            var
                arr = /*StaticObjSerializer.enabled ? StaticObjSerializer.CreateListInstance(t, originalArr.Count) as IList :*/
                    Activator.CreateInstance(t) as IList;
            for (int i = 0; i < originalArr.Count; i++)
            {
                arr.Add(elementDeserializer(context, et, originalArr[i]));
            }

            context.Pop(t);
            return arr;
        }

        public static Func<SerializationContext, Type, PureObject, object> GetDeserializer(Type t)
        {
            if (t.IsEnum)
            {
                return DeserializeEnum;
            }
            else if (t == typeof(string))
            {
                return DeserializeString;
            }
            else if (t == typeof(DateTime))
            {
                return DeserializeDateTime;
            }
            else if (t == typeof(TimeSpan))
            {
                return DeserializeTimeSpan;
            }
            else if (t == typeof(Vector2))
            {
                return DeserializeVector2;
            }
            else if (t == typeof(Vector3))
            {
                return DeserializeVector3;
            }
            else if (IsNaturalValueType(t))
            {
                return DeserializeNaturalValue;
            }
            else if (t.IsArray)
            {
                var et = t.GetElementType();
                var elementDeserializer = GetDeserializer(et);
                return (ctx, tt, oo) => DeserializeArray(ctx, tt, oo, et, elementDeserializer);
            }
            else if (typeof(IList).IsAssignableFrom(t))
            {
                var et = t.GetGenericArguments()[0];
                var elementDeserializer = GetDeserializer(et);
                return (ctx, tt, oo) => DeserializeList(ctx, tt, oo, et, elementDeserializer);
            }
            else if (typeof(IObjDeserializable).IsAssignableFrom(t))
            {
                return DeserializeIObjSerializable;
            }
            else
            {
                return DeserializeObject;
            }
        }

        public static object DeserializeImpl(SerializationContext context, Type t, PureObject o)
        {
            lock (syncRoot)
            {
                // try
                // {
                    return GetDeserializer(t)(context, t, o);
                // }
                // catch (Exception ex)
                // {
                //     throw ex;
                // }
            }
        }

        public static Type GetSubClass(SerializationContext context, Type t, Dictionary<string, object> dic)
        {
            object label = null;
            var attr = t.GetAttribute<SuperClassAttribute>();
            if (attr != null)
            {
                try
                {
                    label = GetLabelFromDic(attr, dic);
                    return SubClassTable.GetClass(attr, label);
                }
                catch (Exception)
                {
                    return SubClassTable.GetClass(attr, Enum.GetValues(attr.targetEnum).GetValue(0));
                }
            }
            else return t;
        }

        static object GetLabelFromDic(SuperClassAttribute superClass, Dictionary<string, object> dic)
        {
            Type enumType = superClass.targetEnum;
            var val = dic[superClass.field];
            if (val is string)
            {
                return EnumGetValueOrFirst(enumType, (string)val);
            }
            else
            {
                return Enum.ToObject(enumType, val);
            }
        }

        public static void Dump()
        {
#if PROFILE_DYNAMIC_DESERIALIZATION
			foreach(var k in s_dynamicDeserializationCount.Keys.ToArray().OrderBy(x => s_dynamicDeserializationCount[x]))
			{
			UnityEngine.Debug.Log(string.Format("{0} : {1}", k, s_dynamicDeserializationCount[k]));
			}
			s_dynamicDeserializationCount.Clear();
#endif
        }

        public static object DeserializeObject(SerializationContext context, Type t, object o)
        {
            var dic = o as Dictionary<string, object>;
            if (dic == null) return null;
            context.Push(t);

            object ret;
            Type actualClass;
            actualClass = GetSubClass(context, t, dic);
            ret = Activator.CreateInstance(actualClass);

            if (ret is IDeserializationHandler)
            {
                ret = (ret as IDeserializationHandler).OnDeserialize(o as Dictionary<string, object>);
                context.Pop(t);
                return ret;
            }

#if PROFILE_DYNAMIC_DESERIALIZATION
			int cnt = 0;
			s_dynamicDeserializationCount.TryGetValue(ret.GetType(), out cnt);
			s_dynamicDeserializationCount[ret.GetType()] = ++cnt;
#endif
            context.Push(ret);
            Exception lastException = null;

            foreach (var f in GetSerializeFields(actualClass))
            {
                object setVal = null;
                object obj = null;

                // try
                // {
                    if (dic.TryGetValue(f.name, out obj))
                    {
                        if (obj == null && !f.permitNull)
                        {
                            Debug.Log(string.Format("{0}没有设置{1}", t,
                                f.name));
                            throw new Exception(string.Format("{0}没有设置{1}", t,
                                f.name)); // FieldNotFoundException(context, t, f.name);
                        }
                        else
                        {
                            f.field.SetValue(ret, setVal = f.deserializer(context, f.field.FieldType, obj));
                        }
                    }
                    else
                    {
                        if (!f.permitNoField)
                        {
                            Debug.Log(string.Format("{0}没有设置{1}", t,
                                f.name));
                            throw new Exception(string.Format("{0}没有设置{1}", t,
                                f.name)); // FieldNotFoundException(context, t, f.name);
                        }

                        if (f.generateDefault)
                        {
                            f.field.SetValue(ret, setVal = Activator.CreateInstance(f.field.FieldType));
                        }
                    }

                    if (f.notEmpty != null && setVal != null)
                    {
                        f.notEmpty.Validate(setVal);
                    }
                // }
                // catch (Exception e)
                // {
                //     lastException = e;
                // }
            }

            // if (lastException != null)
            // {
            //     throw lastException;
            // }

            context.Pop(ret);
            context.Pop(t);

            return ret;
        }

        public static List<FieldEntry> GetSerializeFields(Type t)
        {
            List<FieldEntry> ret = null;
            if (typeCache.TryGetValue(t, out ret))
            {
                return ret;
            }
            else
            {
                var fields = _GetSerializeFields(t);
                typeCache[t] = fields;
                return fields;
            }
        }

        private static List<FieldEntry> _GetSerializeFields(Type t)
        {
            var ret = t.GetFields( /*BindingFlags.NonPublic |*/
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Select(f => new FieldEntry() { field = f, name = f.Name, deserializer = GetDeserializer(f.FieldType) })
                .ToList();

            //在public field中，我想删除这些评论，
            // serializable fieldattribute
            // 因为必须追加，所以要另转
            ret.AddRange(from f in t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            let serializeField = f.GetAttribute<SerializableFieldAttribute>()
            where serializeField != null
            let optional = f.GetAttribute<OptionalAttribute>()
            let notEmpty = f.GetAttribute<NotEmptyAttribute>()
            let dontSerialize = f.GetAttribute<DontSerializeAttribute>()
            let dontSerializeNull = f.GetAttribute<DontSerializeNullAttribute>()
            select new FieldEntry()
            {
                field = f,
                name = serializeField.GetFieldName(t),
                optional = optional,
                notEmpty = notEmpty,
                dontSerialize = dontSerialize != null,
                dontSerializeNull = dontSerializeNull != null,
                deserializer = GetDeserializer(f.FieldType)
            });

            return ret;
        }

        static object EnumGetValueOrFirst(Type enumType, string name)
        {
            if (Enum.IsDefined(enumType, name))
            {
                return Enum.Parse(enumType, name);
            }
            else
            {
                //				Logging.Debug("deserializer", string.Format("'{0}' is not defined in {1}!!", name, enumType.FullName));
                return Enum.GetValues(enumType).GetValue(0);
            }
        }

#if UNITY_EDITOR
        class TestObject
        {
            [SerializableField("hoge")] public int hoge = 0;

            [SerializableField("fuga")] [Optional] public TestObject fuga = null;
        }

        [SuperClass(typeof(Super.Type))]
        class Super
        {
            public enum Type
            {
                A,
                B
            }

            [SerializableField("superField")] public string superField = null;
        }

        [SubClass(Super.Type.A)]
        class SubA : Super
        {
            [SerializableField("subField")] public string subField = null;
        }

        [SubClass(Super.Type.B)]
        class SubB : Super
        {
        }

        class ClassHaveDateTime
        {
            [SerializableField("time")] [Optional] public DateTime time = default;
        }

        class ClassHaveEnum
        {
            [SerializableField("type")] public Super.Type type = default;
        }

#endif
        public class SuperClassAttribute : Attribute
        {
            public enum Type
            {
                STRING,
                INT
            }

            public System.Type targetEnum { get; private set; }
            public string field { get; private set; }
            public Type type { get; private set; }

            public SuperClassAttribute(System.Type targetEnum, string field = "type", Type type = Type.STRING)
            {
                this.targetEnum = targetEnum;
                this.field = field;
                this.type = type;
            }

            public object ConvertLabel(object label)
            {
                if (type == Type.STRING)
                {
                    return label.ToString();
                }
                else
                {
                    return (int)label;
                }
            }
        }
    }
}