using FrostEngine;

namespace ZJYFrameWork
{
    public static class STuple
    {
        public static STuple<TFirst, TSecond> Create<TFirst, TSecond>(TFirst first, TSecond second)
        {
            Debug.Log($"first:{first},second:{second} ; FIRST Type:{typeof(TFirst)},SECOND Type:{typeof(TSecond)}");
            return new STuple<TFirst, TSecond>(first, second);
        }
    }
    public struct STuple<TFirst, TSecond>
    {
        public TFirst First;
        public TSecond Second;

        public STuple(TFirst first, TSecond second)
        {
            this.First = first;
            this.Second = second;
        }
    }
}