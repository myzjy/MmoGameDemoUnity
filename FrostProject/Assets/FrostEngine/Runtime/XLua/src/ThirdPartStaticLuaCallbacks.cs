using System.Runtime.InteropServices;

namespace XLuaBase
{
    public partial class Lua
    {
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
        public static extern int cxx_luaopen_rapidjson(System.IntPtr L);

        public static int luaopen_rapidjson(System.IntPtr L)
        {
            return cxx_luaopen_rapidjson(L);
        }
#else
        public static extern int luaopen_rapidjson(System.IntPtr L);
#endif
        
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
        public static extern int cxx_luaopen_md5(System.IntPtr L);

        public static int luaopen_md5(System.IntPtr L)
        {
            return cxx_luaopen_md5(L);
        }
#else
        public static extern int luaopen_md5(System.IntPtr L);
#endif
    }
}