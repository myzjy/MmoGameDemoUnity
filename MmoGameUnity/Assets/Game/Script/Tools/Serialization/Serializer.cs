// ReSharper disable once CheckNamespace

using System;

namespace Serialization
{
    public class Serializer
    {
        public static readonly JSONSerializer jsonSerializer = new JSONSerializer();

    }

    // ReSharper disable once InconsistentNaming
    public class JSONSerializer
    {
        public T Deserialize<T>(string str, SerializationContext context = null)
        {
            return (T)Deserialize(typeof(T), str, context);
        }
        public object Deserialize(Type type, string str, SerializationContext context = null)
        {
            return ObjSerializer.Deserialize(type, MiniJSON.Json.Deserialize(str), context);
        }
        public string Serialize(object obj, SerializationContext context = null)
        {
            return MiniJSON.Json.Serialize(ObjSerializer.Serialize(obj, context));
        }
    }
}