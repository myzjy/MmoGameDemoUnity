/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

namespace XLuaBase
{

	using System;
	using System.Runtime.InteropServices;
	using System.Text;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || XLUA_GENERAL || (UNITY_WSA && !UNITY_EDITOR)
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int lua_CSFunction(IntPtr L);

#if GEN_CODE_MINIMIZE
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int CSharpWrapperCaller(IntPtr L, int funcidx, int top);
#endif
#else
    public delegate int lua_CSFunction(IntPtr L);

#if GEN_CODE_MINIMIZE
    public delegate int CSharpWrapperCaller(IntPtr L, int funcidx, int top);
#endif
#endif


	public partial class Lua
	{
#if (UNITY_IPHONE || UNITY_TVOS || UNITY_WEBGL || UNITY_SWITCH) && !UNITY_EDITOR
        const string LUADLL = "__Internal";
#else
		const string LUADLL = "xlua";
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern IntPtr cxx_lua_tothread(IntPtr L, int index);

		public static IntPtr lua_tothread(IntPtr L, int index)
		{
			return cxx_lua_tothread(L, index);
		}
#else
		public static extern IntPtr lua_tothread(IntPtr L, int index);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_get_lib_version();

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_gc(IntPtr L, int what, int data);

		public static int lua_gc(IntPtr L, int what, int data)
		{
			return cxx_lua_gc(L, what, data);
		}
#else
		public static extern int lua_gc(IntPtr L, int what, int data);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern IntPtr cxx_lua_getupvalue(IntPtr L, int funcindex, int n);

		public static IntPtr lua_getupvalue(IntPtr L, int funcindex, int n)
		{
			return cxx_lua_getupvalue(L, funcindex, n);
		}
#else
		public static extern IntPtr lua_getupvalue(IntPtr L, int funcindex, int n);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern IntPtr cxx_lua_setupvalue(IntPtr L, int funcindex, int n);

		public static IntPtr lua_setupvalue(IntPtr L, int funcindex, int n)
		{
			return cxx_lua_setupvalue(L, funcindex, n);
		}
#else
		public static extern IntPtr lua_setupvalue(IntPtr L, int funcindex, int n);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_pushthread(IntPtr L);

		public static int lua_pushthread(IntPtr L)
		{
			return cxx_lua_pushthread(L);
		}
#else
		public static extern int lua_pushthread(IntPtr L);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_setfenv(IntPtr L, int stackPos);

		public static int lua_setfenv(IntPtr L, int stackPos)
		{
			return cxx_lua_setfenv(L, stackPos);
		}
#else
		public static extern int lua_setfenv(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern IntPtr cxx_luaL_newstate();

		public static IntPtr luaL_newstate()
		{
			return cxx_luaL_newstate();
		}
#else
		public static extern IntPtr luaL_newstate();
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_lua_close(IntPtr L);

		public static void lua_close(IntPtr L)
		{
			cxx_lua_close(L);
		}
#else
		public static extern void lua_close(IntPtr L);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)] //[-0, +0, m]
		public static extern void luaopen_xlua(IntPtr L);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)] //[-0, +0, m]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_luaL_openlibs(IntPtr L);

		public static void luaL_openlibs(IntPtr L)
		{
			cxx_luaL_openlibs(L);
		}
#else
		public static extern void luaL_openlibs(IntPtr L);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint xlua_objlen(IntPtr L, int stackPos);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_lua_createtable(IntPtr L, int narr, int nrec);

		public static void lua_createtable(IntPtr L, int narr, int nrec)
		{
			cxx_lua_createtable(L, narr, nrec);
		}
#else
		public static extern void lua_createtable(IntPtr L, int narr, int nrec); //[-0, +0, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_settop(IntPtr L, int stackPos);

		public static int lua_settop(IntPtr L, int stackPos)
		{
			return cxx_lua_settop(L, stackPos);
		}
#else
		public static extern int lua_settop(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_insert(IntPtr L, int stackPos);

		public static int lua_insert(IntPtr L, int stackPos)
		{
			return cxx_lua_insert(L, stackPos);
		}
#else
		public static extern int lua_insert(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_remove(IntPtr L, int stackPos);

		public static int lua_remove(IntPtr L, int stackPos)
		{
			return cxx_lua_remove(L, stackPos);
		}
#else
		public static extern int lua_remove(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_rawget(IntPtr L, int stackPos);

		public static int lua_rawget(IntPtr L, int stackPos)
		{
			return cxx_lua_rawget(L, stackPos);
		}
#else
		public static extern int lua_rawget(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_rawset(IntPtr L, int stackPos);

		public static int lua_rawset(IntPtr L, int stackPos)
		{
			return cxx_lua_rawset(L, stackPos);
		}
#else
		public static extern int lua_rawset(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_setmetatable(IntPtr L, int stackPos);

		public static int lua_setmetatable(IntPtr L, int stackPos)
		{
			return cxx_lua_setmetatable(L, stackPos);
		}
#else
		public static extern int lua_setmetatable(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_rawequal(IntPtr L, int index1, int index2);

		public static int lua_rawequal(IntPtr L, int index1, int index2)
		{
			return cxx_lua_rawequal(L, index1, index2);
		}
#else
		public static extern int lua_rawequal(IntPtr L, int index1, int index2);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_lua_pushvalue(IntPtr L, int stackPos);

		public static void lua_pushvalue(IntPtr L, int stackPos)
		{
			cxx_lua_pushvalue(L, stackPos);
		}
#else
		public static extern void lua_pushvalue(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_lua_pushcclosure(IntPtr L, IntPtr fn, int n); //[-n, +1, m]

		public static void lua_pushcclosure(IntPtr L, IntPtr fn, int n)
		{
			cxx_lua_pushcclosure(L, fn, n);
		}
#else
		public static extern void lua_pushcclosure(IntPtr L, IntPtr fn, int n); //[-n, +1, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_lua_replace(IntPtr L, int stackPos); //[-n, +1, m]

		public static void lua_replace(IntPtr L, int stackPos)
		{
			cxx_lua_replace(L, stackPos);
		}
#else
		public static extern void lua_replace(IntPtr L, int stackPos); //[-n, +1, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_gettop(IntPtr L);

		public static int lua_gettop(IntPtr L)
		{
			return cxx_lua_gettop(L);
		}
#else
		public static extern int lua_gettop(IntPtr L);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_type(IntPtr L, int stackPos);

		public static int lua_type(IntPtr L, int stackPos)
		{
			return cxx_lua_type(L, stackPos);
		}
#else
		public static extern int lua_type(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern bool cxx_lua_isnumber(IntPtr L, int stackPos);

		public static bool lua_isnumber(IntPtr L, int stackPos)
		{
			return cxx_lua_isnumber(L, stackPos);
		}
#else
		public static extern bool lua_isnumber(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_luaL_ref(IntPtr L, int stackPos);

		public static int luaL_ref(IntPtr L, int stackPos)
		{
			return cxx_luaL_ref(L, stackPos);
		}
#else
		public static extern int luaL_ref(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_luaL_unref(IntPtr L, int stackPos, int r);

		public static int luaL_unref(IntPtr L, int stackPos, int r)
		{
			return cxx_luaL_unref(L, stackPos, r);
		}
#else
		public static extern int luaL_unref(IntPtr L, int stackPos, int r);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern bool cxx_lua_isstring(IntPtr L, int stackPos);

		public static bool lua_isstring(IntPtr L, int stackPos)
		{
			return cxx_lua_isstring(L, stackPos);
		}
#else
		public static extern bool lua_isstring(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern bool cxx_lua_isinteger(IntPtr L, int stackPos);

		public static bool lua_isinteger(IntPtr L, int stackPos)
		{
			return cxx_lua_isinteger(L, stackPos);
		}
#else
		public static extern bool lua_isinteger(IntPtr L, int stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_lua_pushnil(IntPtr L);

		public static void lua_pushnil(IntPtr L)
		{
			cxx_lua_pushnil(L);
		}
#else
		public static extern void lua_pushnil(IntPtr L);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern IntPtr cxx_lua_atpanic(IntPtr L, lua_CSFunction panicf);

		public static IntPtr lua_atpanic(IntPtr L, lua_CSFunction panicf)
		{
			return cxx_lua_atpanic(L, panicf);
		}
#else
		public static extern IntPtr lua_atpanic(IntPtr L, lua_CSFunction panicf);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_lua_pushnumber(IntPtr L, double stackPos);

		public static void lua_pushnumber(IntPtr L, double stackPos)
		{
			cxx_lua_pushnumber(L, stackPos);
		}
#else
		public static extern void lua_pushnumber(IntPtr L, double stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_lua_pushboolean(IntPtr L, bool stackPos);

		public static void lua_pushboolean(IntPtr L, bool stackPos)
		{
			cxx_lua_pushboolean(L, stackPos);
		}
#else
		public static extern void lua_pushboolean(IntPtr L, bool stackPos);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_luaL_newmetatable(IntPtr L, string meta);

		public static int luaL_newmetatable(IntPtr L, string meta)
		{
			return cxx_luaL_newmetatable(L, meta);
		}
#else
		public static extern int luaL_newmetatable(IntPtr L, string meta); //[-0, +1, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_pcall(IntPtr L, int nArgs, int nResults, int errfunc);

		public static int lua_pcall(IntPtr L, int nArgs, int nResults, int errfunc)
		{
			return cxx_lua_pcall(L, nArgs, nResults, errfunc);
		}
#else
		public static extern int lua_pcall(IntPtr L, int nArgs, int nResults, int errfunc);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern double cxx_lua_tonumber(IntPtr L, int index);

		public static double lua_tonumber(IntPtr L, int index)
		{
			return cxx_lua_tonumber(L, index);
		}
#else
		public static extern double lua_tonumber(IntPtr L, int index);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern bool cxx_lua_toboolean(IntPtr L, int index);

		public static bool lua_toboolean(IntPtr L, int index)
		{
			return cxx_lua_toboolean(L, index);
		}
#else
		public static extern bool lua_toboolean(IntPtr L, int index);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern IntPtr cxx_lua_topointer(IntPtr L, int index);

		public static IntPtr lua_topointer(IntPtr L, int index)
		{
			return cxx_lua_topointer(L, index);
		}
#else
		public static extern IntPtr lua_topointer(IntPtr L, int index);
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern IntPtr cxx_lua_tolstring(IntPtr L, int index, out IntPtr strLen);

		public static IntPtr lua_tolstring(IntPtr L, int index, out IntPtr strLen)
		{
			return cxx_lua_tolstring(L, index, out strLen);
		}
#else
		public static extern IntPtr lua_tolstring(IntPtr L, int index, out IntPtr strLen); //[-0, +0, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern bool cxx_lua_checkstack(IntPtr L, int index);

		public static bool lua_checkstack(IntPtr L, int index)
		{
			return cxx_lua_checkstack(L, index);
		}
#else
		public static extern bool lua_checkstack(IntPtr L, int index); //[-0, +0, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_lua_next(IntPtr L, int index);

		public static int lua_next(IntPtr L, int index)
		{
			return cxx_lua_next(L, index);
		}
#else
		public static extern int lua_next(IntPtr L, int index); //[-0, +0, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_lua_pushlightuserdata(IntPtr L, IntPtr index);

		public static void lua_pushlightuserdata(IntPtr L, IntPtr index)
		{
			cxx_lua_pushlightuserdata(L, index);
		}
#else
		public static extern void lua_pushlightuserdata(IntPtr L, IntPtr index); //[-0, +0, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_luaopen_i64lib(IntPtr L);

		public static int luaopen_i64lib(IntPtr L)
		{
			return cxx_luaopen_i64lib(L);
		}
#else
		public static extern int luaopen_i64lib(IntPtr L); //[-0, +0, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern int cxx_luaopen_socket_core(IntPtr L);

		public static int luaopen_socket_core(IntPtr L)
		{
			return cxx_luaopen_socket_core(L);
		}
#else
		public static extern int luaopen_socket_core(IntPtr L); //[,,m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_lua_pushint64(IntPtr L, long n);

		public static void lua_pushint64(IntPtr L, long n)
		{
			cxx_lua_pushint64(L, n);
		}
#else
		public static extern void lua_pushint64(IntPtr L, long n); //[,,m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_lua_pushuint64(IntPtr L, ulong n);

		public static void lua_pushuint64(IntPtr L, ulong n)
		{
			cxx_lua_pushuint64(L, n);
		}
#else
		public static extern void lua_pushuint64(IntPtr L, ulong n); //[,,m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern bool cxx_lua_isint64(IntPtr L, int index);

		public static bool lua_isint64(IntPtr L, int index)
		{
			return cxx_lua_isint64(L, index);
		}
#else
		public static extern bool lua_isint64(IntPtr L, int index); //[-0, +0, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern bool cxx_lua_isuint64(IntPtr L, int index);

		public static bool lua_isuint64(IntPtr L, int index)
		{
			return cxx_lua_isuint64(L, index);
		}
#else
		public static extern bool lua_isuint64(IntPtr L, int index); //[-0, +0, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern long cxx_lua_toint64(IntPtr L, int index);

		public static long lua_toint64(IntPtr L, int index)
		{
			return cxx_lua_toint64(L, index);
		}
#else
		public static extern long lua_toint64(IntPtr L, int index); //[-0, +0, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern ulong cxx_lua_touint64(IntPtr L, int index);

		public static ulong lua_touint64(IntPtr L, int index)
		{
			return cxx_lua_touint64(L, index);
		}
#else
		public static extern ulong lua_touint64(IntPtr L, int index); //[-0, +0, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern IntPtr cxx_lua_touserdata(IntPtr L, int index);

		public static IntPtr lua_touserdata(IntPtr L, int index)
		{
			return cxx_lua_touserdata(L, index);
		}
#else
		public static extern IntPtr lua_touserdata(IntPtr L, int index); //[-0, +0, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_luaL_where(IntPtr L, int level); //[-0, +1, m]

		public static void luaL_where(IntPtr L, int level)
		{
			cxx_luaL_where(L, level);
		}
#else
		public static extern void luaL_where(IntPtr L, int level); //[-0, +1, m]
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_getglobal(IntPtr L, string name); //[-1, +0, m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_setglobal(IntPtr L, string name); //[-1, +0, m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void xlua_getloaders(IntPtr L);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void xlua_rawgeti(IntPtr L, int tableIndex, long index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void xlua_rawseti(IntPtr L, int tableIndex, long index); //[-1, +0, m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int pcall_prepare(IntPtr L, int error_func_ref, int func_ref);
		
		public static void lua_pushstdcallcfunction(IntPtr L, lua_CSFunction function, int n = 0) //[-0, +1, m]
		{
#if XLUA_GENERAL || (UNITY_WSA && !UNITY_EDITOR)
            GCHandle.Alloc(function);
#endif
			IntPtr fn = Marshal.GetFunctionPointerForDelegate(function);
			xlua_push_csharp_function(L, fn, n);
		}
		
		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_upvalueindex(int n);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_tointeger(IntPtr L, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint xlua_touint(IntPtr L, int index);

	
		public static string lua_tostring(IntPtr L, int index)
		{
			IntPtr strlen;

			IntPtr str = lua_tolstring(L, index, out strlen);
			if (str != IntPtr.Zero)
			{
#if XLUA_GENERAL || (UNITY_WSA && !UNITY_EDITOR)
                int len = strlen.ToInt32();
                byte[] buffer = new byte[len];
                Marshal.Copy(str, buffer, 0, len);
                return Encoding.UTF8.GetString(buffer);
#else
				string ret = Marshal.PtrToStringAnsi(str, strlen.ToInt32());
				if (ret == null)
				{
					int len = strlen.ToInt32();
					byte[] buffer = new byte[len];
					Marshal.Copy(str, buffer, 0, len);
					return Encoding.UTF8.GetString(buffer);
				}

				return ret;
#endif
			}
			else
			{
				return null;
			}
		}
		
		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void xlua_pushinteger(IntPtr L, int value);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void xlua_pushuint(IntPtr L, uint value);
		
		
		public static byte[] lua_tobytes(IntPtr L, int index) //[-0, +0, m]
		{
			if (lua_type(L, index) == 4)
			{
				IntPtr strlen;
				IntPtr str = lua_tolstring(L, index, out strlen);
				if (str != IntPtr.Zero)
				{
					int buff_len = strlen.ToInt32();
					byte[] buffer = new byte[buff_len];
					Marshal.Copy(str, buffer, 0, buff_len);
					return buffer;
				}
			}

			return null;
		}

#if NATIVE_LUA_PUSHSTRING
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
#if UNITY_IPHONE && !UNITY_EDITOR
		public static extern void cxx_lua_pushstring(IntPtr L, string str);
        public static long lua_pushstring(IntPtr L, string str){cxx_lua_pushstring(L, str);}
#else
       public static extern void lua_pushstring(IntPtr L, string str);
#endif
#endif

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void xlua_pushlstring(IntPtr L, byte[] str, int size);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_pgettable(IntPtr L, int idx);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_psettable(IntPtr L, int idx);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xluaL_loadbuffer(IntPtr L, byte[] buff, int size, string name);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_tocsobj_safe(IntPtr L, int obj); //[-0, +0, m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_tocsobj_fast(IntPtr L, int obj);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr xlua_tag();

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_tryget_cachedud(IntPtr L, int key, int cache_ref);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void
			xlua_pushcsobj(IntPtr L, int key, int meta_ref, bool need_cache, int cache_ref); //[-0, +1, m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)] //[,,m]
		public static extern int gen_obj_indexer(IntPtr L);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)] //[,,m]
		public static extern int gen_obj_newindexer(IntPtr L);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)] //[,,m]
		public static extern int gen_cls_indexer(IntPtr L);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)] //[,,m]
		public static extern int gen_cls_newindexer(IntPtr L);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)] //[,,m]
		public static extern int get_error_func_ref(IntPtr L);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)] //[,,m]
		public static extern int load_error_func(IntPtr L, int Ref);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void xlua_push_csharp_function(IntPtr L, IntPtr fn, int n); //[-0,+1,m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_csharp_str_error(IntPtr L, string message); //[-0,+1,m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)] //[-0,+0,m]
		public static extern int xlua_csharp_error(IntPtr L);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_pack_int8_t(IntPtr buff, int offset, byte field);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_unpack_int8_t(IntPtr buff, int offset, out byte field);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_pack_int16_t(IntPtr buff, int offset, short field);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_unpack_int16_t(IntPtr buff, int offset, out short field);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_pack_int32_t(IntPtr buff, int offset, int field);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_unpack_int32_t(IntPtr buff, int offset, out int field);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_pack_int64_t(IntPtr buff, int offset, long field);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_unpack_int64_t(IntPtr buff, int offset, out long field);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_pack_float(IntPtr buff, int offset, float field);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_unpack_float(IntPtr buff, int offset, out float field);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_pack_double(IntPtr buff, int offset, double field);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_unpack_double(IntPtr buff, int offset, out double field);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr xlua_pushstruct(IntPtr L, uint size, int meta_ref);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void xlua_pushcstable(IntPtr L, uint field_count, int meta_ref);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_gettypeid(IntPtr L, int idx);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_get_registry_index();

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_pgettable_bypath(IntPtr L, int idx, string path);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int xlua_psettable_bypath(IntPtr L, int idx, string path);

		//对于Unity，仅浮点组成的struct较多，这几个api用于优化这类struct
		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_pack_float2(IntPtr buff, int offset, float f1, float f2);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_unpack_float2(IntPtr buff, int offset, out float f1, out float f2);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_pack_float3(IntPtr buff, int offset, float f1, float f2, float f3);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_unpack_float3(IntPtr buff, int offset, out float f1, out float f2, out float f3);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_pack_float4(IntPtr buff, int offset, float f1, float f2, float f3, float f4);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_unpack_float4(IntPtr buff, int offset, out float f1, out float f2, out float f3,
			out float f4);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_pack_float5(IntPtr buff, int offset, float f1, float f2, float f3, float f4,
			float f5);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_unpack_float5(IntPtr buff, int offset, out float f1, out float f2, out float f3,
			out float f4, out float f5);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_pack_float6(IntPtr buff, int offset, float f1, float f2, float f3, float f4,
			float f5, float f6);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_unpack_float6(IntPtr buff, int offset, out float f1, out float f2, out float f3,
			out float f4, out float f5, out float f6);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_pack_decimal(IntPtr buff, int offset, ref decimal dec);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_unpack_decimal(IntPtr buff, int offset, out byte scale, out byte sign,
			out int hi32, out ulong lo64);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool xlua_is_eq_str(IntPtr L, int index, string str, int str_len);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr xlua_gl(IntPtr L);

#if GEN_CODE_MINIMIZE
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void xlua_set_csharp_wrapper_caller(IntPtr wrapper);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void xlua_push_csharp_wrapper(IntPtr L, int wrapperID);
#endif
	}
}
