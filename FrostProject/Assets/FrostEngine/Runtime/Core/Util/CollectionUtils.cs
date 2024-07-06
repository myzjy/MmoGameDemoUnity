using System.Collections.Generic;

namespace FrostEngine
{
    public abstract class CollectionUtils
    {
        public static readonly int[] EmptyINTArray = new int[] { };

        public static readonly byte[] EmptyByteArray = new byte[] { };
        
        public static List<T> EmptyList<T>()
        {
            return new List<T>(0);
        }
        
        public static HashSet<T> EmptyHashSet<T>()
        {
            return new HashSet<T>();
        }
        
        public static Dictionary<TK, TV> EmptyDictionary<TK, TV>()
        {
            return new Dictionary<TK, TV>(0);
        }
        
        public static bool IsEmpty(object[] array)
        {
            return array == null || array.Length == 0;
        }

        public static bool IsEmpty<T>(ICollection<T> collection)
        {
            return collection is not { Count: > 0 };
        }

        public static bool IsNotEmpty<T>(ICollection<T> collection)
        {
            return !IsEmpty(collection);
        }
    }
}