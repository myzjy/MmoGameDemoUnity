namespace ZJYFrameWork
{
    public static class STuple
    {
        public static STuple<FIRST, SECOND> Create<FIRST, SECOND>(FIRST first, SECOND second)
        {
            return new STuple<FIRST, SECOND>(first, second);
        }
    }
    public struct STuple<FIRST, SECOND>
    {
        public FIRST first;
        public SECOND second;

        public STuple(FIRST first, SECOND second)
        {
            this.first = first;
            this.second = second;
        }
    }
}